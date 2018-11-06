using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.OutgoingModels;
using SciFiReviewsApi.Models.ReturnModels;
using SciFiReviewsApi.Services;

namespace SciFiReviewsApi.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private IMovieData _movieData;
        private IUrlHelper _urlHelper;
        private IReviewData _reviewData;

        public MoviesController(IMovieData movieData, IUrlHelper urlHelper, IReviewData reviewData)
        {
            _movieData = movieData;
            _urlHelper = urlHelper;
            _reviewData = reviewData;
        }

        [HttpGet(Name = "GetMovies")]
        public async Task<IActionResult> GetMovies([FromQuery]MoviesResourceParameters moviesResourceParameters)
        {
            var movies = await _movieData.GetMovies(moviesResourceParameters);

            var returnModel = Mapper.Map<List<MovieReturnModel>>(movies);

            var pagedReturnModel = await PagedList<MovieReturnModel>.Create(returnModel,
                moviesResourceParameters.PageNumber,
                moviesResourceParameters.PageSize);

            var previousPageLink = pagedReturnModel.HasPrevious ?
                CreateMoviesResourceUri(moviesResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = pagedReturnModel.HasNext ?
                CreateMoviesResourceUri(moviesResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = pagedReturnModel.TotalCount,
                pageSize = pagedReturnModel.PageSize,
                currentPage = pagedReturnModel.CurrentPage,
                totalPages = pagedReturnModel.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            return Ok(pagedReturnModel);
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _movieData.GetMovie(id);

            if (movie == null)
                return NotFound();

            var returnModel = Mapper.Map<MovieReturnModel>(movie);

            var movieReviews = await _reviewData.GetReviews(new ReviewsResourceParameters { MovieId = id });

            returnModel.MovieReviews = Mapper.Map<List<ReviewReturnModel>>(movieReviews);

            return Ok(returnModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreate createModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                Movie movie = await _movieData.CreateMovie(createModel);

                return CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMovie(int id, [FromBody] MovieCreate editModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!await _movieData.MovieExists(id))
                return NotFound();

            try
            {
                await _movieData.EditMovie(id, editModel);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (!await _movieData.MovieExists(id))
                return NotFound();

            try
            {
                await _movieData.DeleteMovie(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string CreateMoviesResourceUri(MoviesResourceParameters moviesResourceParameters, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetMovies", new
                    {
                        searchWord = moviesResourceParameters.SearchWord,
                        sortBy = moviesResourceParameters.SortBy,
                        movieGenre = moviesResourceParameters.MovieGenre,
                        pageNumber = moviesResourceParameters.PageNumber - 1,
                        pageSize = moviesResourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetMovies", new
                    {
                        searchWord = moviesResourceParameters.SearchWord,
                        sortBy = moviesResourceParameters.SortBy,
                        movieGenre = moviesResourceParameters.MovieGenre,
                        pageNumber = moviesResourceParameters.PageNumber + 1,
                        pageSize = moviesResourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetMovies", new
                    {
                        searchWord = moviesResourceParameters.SearchWord,
                        sortBy = moviesResourceParameters.SortBy,
                        movieGenre = moviesResourceParameters.MovieGenre,
                        pageNumber = moviesResourceParameters.PageNumber,
                        pageSize = moviesResourceParameters.PageSize
                    });
            }
        }
    }
}