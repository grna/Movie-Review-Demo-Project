using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;

namespace SciFiReviewsApi.Services
{
    public class SqlReviewData : IReviewData
    {
        private SciFiReviewsDbContext _dbContext;
        private IPosterData _posterData;

        public SqlReviewData(SciFiReviewsDbContext context, IPosterData posterData)
        {
            _dbContext = context;
            _posterData = posterData;
        }

        public async Task<List<Review>> GetReviews(ReviewsResourceParameters reviewsResourceParameters)
        {
            var reviews = _dbContext.Reviews
                .Include(r => r.Movie)
                .Include(r => r.Reviewer)
                .AsQueryable();

            // Filter by MovieId
            if (reviewsResourceParameters.MovieId != null)
                reviews = reviews.Where(r => r.MovieId == reviewsResourceParameters.MovieId);

            // Filter by ReviewerId
            if (reviewsResourceParameters.ReviewerId != null)
                reviews = reviews.Where(r => r.ReviewerId == reviewsResourceParameters.ReviewerId);

            // Filter by MovieGenre
            if (!string.IsNullOrEmpty(reviewsResourceParameters.MovieGenre))
            {
                reviews = reviews.Where(
                    r => r.Movie.Genre == reviewsResourceParameters.MovieGenre.Trim());
            }
            
            // Apply sorting
            reviews = await ApplySorting(reviews, reviewsResourceParameters.SortBy);

            return await reviews.ToListAsync();
        }
        

        public async Task<Review> GetReview(int id)
        {
            var review = await _dbContext.Reviews
                    .Include(r => r.Movie)
                    .Include(r => r.Reviewer)
                    .Include(r => r.Comments)
                        .ThenInclude(c => c.Reviewer)
                    .FirstOrDefaultAsync(r => r.Id == id);

            return review;
        }

        public async Task<Review> CreateReview(ReviewCreate creationModel)
        {
            Review review;
            Movie movie = await _dbContext.Movies
                .FirstOrDefaultAsync(m => m.Title.ToLower() == creationModel.Movie.Title.ToLower()
                    && m.ReleaseYear == creationModel.Movie.ReleaseYear);
            Reviewer reviewer = await _dbContext.Reviewers
                .FirstOrDefaultAsync(r => r.Username == creationModel.ReviewerName);
            
            // Create review model for an existing movie
            if (movie != null)
            {
                review = new Review
                {
                    Title = creationModel.Title,
                    MovieId = movie.Id,
                    Body = creationModel.Body,
                    Rating = creationModel.Rating,
                    SubmitTime = creationModel.SubmitTime,
                    ReviewerId = reviewer.Id
                };

                // If movie exists, update it's AverageRating
                await UpdateAverageRating(movie.Id, creationModel.Rating);
            }
            // Create Review model for a new movie
            else
            {
                review = new Review
                {
                    Title = creationModel.Title,
                    Movie = new Movie
                    {
                        Title = creationModel.Movie.Title,
                        Author = creationModel.Movie.Author,
                        AverageRating = creationModel.Rating,
                        ImagePath = await _posterData.GetMoviePoster(creationModel.Movie.ReleaseYear, 
                            creationModel.Movie.Title),
                        ReleaseYear = creationModel.Movie.ReleaseYear
                    },
                    Body = creationModel.Body,
                    Rating = creationModel.Rating,
                    SubmitTime = creationModel.SubmitTime,
                    ReviewerId = reviewer.Id
                };
            }

            await _dbContext.AddAsync(review);
            reviewer.NumberOfReviewsSubmitted++;
            _dbContext.Reviewers.Update(reviewer);
            await _dbContext.SaveChangesAsync();

            return review;
        }

        public async Task<bool> EditReview(ReviewEdit editModel)
        {
            Review review = await _dbContext.Reviews.FindAsync(editModel.Id);

            if (review == null)
                return false;

            var oldRating = review.Rating;
            
            review.Title = editModel.Title;
            review.Body = editModel.Body;
            review.Rating = editModel.Rating;

            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync();

            if (oldRating != editModel.Rating)
                await UpdateAverageRating(review.MovieId, null);

            return true;
        }

        public async Task<bool> DeleteReview(int id)
        {
            Review review = await _dbContext.Reviews.FindAsync(id);

            if (review == null)
                return false;

            _dbContext.Reviews.Remove(review);

            await _dbContext.SaveChangesAsync();
            await UpdateAverageRating(review.MovieId, null);

            return true;
        }

        public async Task CreateComment(CommentCreate createModel)
        {
            var review = await _dbContext.Reviews.FindAsync(createModel.ReviewId);
            var reviewer = await _dbContext.Reviewers.FirstOrDefaultAsync(r => r.Username == createModel.ReviewerName);

            reviewer.NumberOfCommentsSubmitted++;
            review.Comments.Add(new Comment
            {
                ReviewerId = reviewer.Id,
                Body = createModel.Body,
                Timestamp = DateTime.Now
            });

            _dbContext.Reviews.Update(review);
            _dbContext.Reviewers.Update(reviewer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Comment> GetComment(int id)
        {
            return await _dbContext.Comments.FindAsync(id);
        }

        public async Task DeleteComment(int reviewId, Comment comment)
        {
            var review = await _dbContext.Reviews.FindAsync(reviewId);

            review.Comments.Remove(comment);

            _dbContext.Reviews.Update(review);

            await _dbContext.SaveChangesAsync();
        }

        public async Task LikeReview(int id, int reviewerId, bool like)
        {
            List<string> peopleWhoLikedReview;
            List<string> likedReviews;

            var review = await _dbContext.Reviews
                .FindAsync(id);

            var reviewer = await _dbContext.Reviewers
                .FindAsync(reviewerId);

            if (like)
            {
                peopleWhoLikedReview = (review.PeopleWhoLikedReview == null) ? 
                    new List<string>() : review.PeopleWhoLikedReview.ToList();

                likedReviews = (reviewer.LikedReviews == null) ? 
                    new List<string>() : reviewer.LikedReviews.ToList();

                review.NumberOfLikes++;
                peopleWhoLikedReview.Add(reviewer.Username);
                likedReviews.Add(id.ToString());

                review.PeopleWhoLikedReview = peopleWhoLikedReview.ToArray();
                reviewer.LikedReviews = likedReviews.ToArray();
            }
            else
            {
                peopleWhoLikedReview = review.PeopleWhoLikedReview.ToList();
                likedReviews = reviewer.LikedReviews.ToList();

                review.NumberOfLikes--;
                peopleWhoLikedReview.Remove(reviewer.Username);
                likedReviews.Remove(id.ToString());

                review.PeopleWhoLikedReview = (peopleWhoLikedReview.Count == 0) ?
                    null : peopleWhoLikedReview.ToArray();

                reviewer.LikedReviews = (likedReviews.Count == 0) ?
                    null : likedReviews.ToArray();
            }

            _dbContext.Reviews.Update(review);
            _dbContext.Reviewers.Update(reviewer);

            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateAverageRating(int movieId, float? rating)
        {
            var movie = await _dbContext.Movies.FindAsync(movieId);

            var ratings = await _dbContext.Reviews
                .Where(r => r.MovieId == movieId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (rating != null)
            {
                ratings.Add((float)rating);
            }

            if (ratings != null && ratings.Count > 0)
                movie.AverageRating = (float)Math.Round(ratings.Average(), 2);
            else
                movie.AverageRating = 0;

            await _dbContext.SaveChangesAsync();
        }

        private Task<IQueryable<Review>> ApplySorting(IQueryable<Review> reviews, string sortBy)
        {
            switch (sortBy)
            {
                case "rating_asc":
                    reviews = reviews.OrderBy(r => r.Rating);
                    break;
                case "rating_desc":
                    reviews = reviews.OrderByDescending(r => r.Rating);
                    break;
                case "movie_asc":
                    reviews = reviews.OrderBy(r => r.Movie.Title);
                    break;
                case "movie_desc":
                    reviews = reviews.OrderByDescending(r => r.Movie.Title);
                    break;
                case "reviewer_asc":
                    reviews = reviews.OrderBy(r => r.Reviewer.Username);
                    break;
                case "reviewer_desc":
                    reviews = reviews.OrderByDescending(r => r.Reviewer.Username);
                    break;
                case "time_asc":
                    reviews = reviews.OrderBy(r => r.SubmitTime);
                    break;
                default:
                    reviews = reviews.OrderByDescending(r => r.SubmitTime);
                    break;
            }

            return Task.FromResult(reviews.AsQueryable());
        }
    }
}
