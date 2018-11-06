using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.ReturnModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Services
{
    public interface IMovieData
    {
        Task<List<Movie>> GetMovies(MoviesResourceParameters moviesResourceParameters);

        Task<Movie> GetMovie(int id);

        Task<Movie> CreateMovie(MovieCreate createModel);

        Task EditMovie(int id, MovieCreate editModel);

        Task DeleteMovie(int id);

        Task<bool> MovieExists(int id);
    }
}
