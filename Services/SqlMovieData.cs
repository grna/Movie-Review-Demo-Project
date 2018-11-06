using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.ReturnModels;

namespace SciFiReviewsApi.Services
{
    public class SqlMovieData : IMovieData
    {
        private SciFiReviewsDbContext _dbContext;
        private IPosterData _posterData;

        public SqlMovieData(SciFiReviewsDbContext dbContext, IPosterData posterData)
        {
            _dbContext = dbContext;
            _posterData = posterData;
        }


        public async Task<List<Movie>> GetMovies(MoviesResourceParameters moviesResourceParameters)
        {
            var movies = _dbContext.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(moviesResourceParameters.SearchWord))
                movies = movies.Where(m => m.Title.Contains(moviesResourceParameters.SearchWord)
                    || m.Author.Contains(moviesResourceParameters.SearchWord));

            return await movies.ToListAsync();
        }      

        public async Task<Movie> GetMovie(int id)
        {
            return await _dbContext.Movies
                .FindAsync(id);
        }

        public async Task<Movie> CreateMovie(MovieCreate createModel)
        {
            Movie movie = new Movie
            {
                Title = createModel.Title,
                Author = createModel.Author,
                ReleaseYear = createModel.ReleaseYear,
                ImagePath = await _posterData.GetMoviePoster(createModel.ReleaseYear, createModel.Title),
                AverageRating = 0
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            return movie;
        }

        public async Task EditMovie(int id, MovieCreate editModel)
        {
            var movie = await _dbContext.Movies.FindAsync(id);

            movie.Title = editModel.Title;
            movie.Author = editModel.Author;
            movie.ReleaseYear = editModel.ReleaseYear;

            _dbContext.Movies.Add(movie);

             await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            _dbContext.Movies.Remove(movie);

            await _dbContext.SaveChangesAsync();
        }

        

        public async Task<bool> MovieExists(int id)
        {
            return await _dbContext.Movies.AnyAsync(m => m.Id == id);
        }
    }
}
