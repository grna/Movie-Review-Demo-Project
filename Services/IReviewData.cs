using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Services
{
    public interface IReviewData
    {
        Task<List<Review>> GetReviews(ReviewsResourceParameters reviewsResourceParameters);

        Task<Review> GetReview(int id);

        Task<Review> CreateReview(ReviewCreate creationModel);

        Task<bool> EditReview(ReviewEdit editModel);

        Task<bool> DeleteReview(int id);

        Task<Comment> GetComment(int id);

        Task CreateComment(CommentCreate createModel);

        Task DeleteComment(int reviewId, Comment comment);

        Task LikeReview(int id, int reviewerId, bool like);
    }
}
