using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciFiReviewsApi.Helpers;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.OutgoingModels;
using SciFiReviewsApi.Models.ReturnModels;
using SciFiReviewsApi.Services;

namespace SciFiReviewsApi.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private IReviewData _reviewData;
        private IUrlHelper _urlHelper;
        private IReviewerData _reviewerData;
        private ITypeHelperService _typeHelperService;

        public ReviewsController(IReviewData reviewData, IUrlHelper urlHelper, 
            IReviewerData reviewerData, ITypeHelperService typeHelperService)
        {
            _reviewData = reviewData;
            _urlHelper = urlHelper;
            _reviewerData = reviewerData;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetReviews")]
        public async Task<IActionResult> GetReviews([FromQuery]ReviewsResourceParameters reviewsResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ReviewReturnModel>(
                reviewsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var reviews = await _reviewData.GetReviews(reviewsResourceParameters);

            var returnModel = Mapper.Map<List<ReviewReturnModel>>(reviews);

            var pagedReturnModel = await PagedList<ReviewReturnModel>.Create(returnModel,
                reviewsResourceParameters.PageNumber,
                reviewsResourceParameters.PageSize);

            var previousPageLink = pagedReturnModel.HasPrevious ?
                CreateReviewsResourceUri(reviewsResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = pagedReturnModel.HasNext ?
                CreateReviewsResourceUri(reviewsResourceParameters,
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

            var links = CreateLinksForReviews(reviewsResourceParameters,
                pagedReturnModel.HasNext, pagedReturnModel.HasPrevious);

            var shapedReturnModel = pagedReturnModel.AsEnumerable().ShapeData(reviewsResourceParameters.Fields);

            var shapedReturnModelWithLinks = shapedReturnModel.Select(review =>
            {
                var reviewAsDictionary = review as IDictionary<string, object>;
                var reviewLinks = CreateLinksForReview(
                    (int)reviewAsDictionary["Id"], reviewsResourceParameters.Fields);
                reviewAsDictionary.Add("links", reviewLinks);

                return reviewAsDictionary;
            });

            return Ok(shapedReturnModelWithLinks);
        }

        [HttpGet("{id}", Name = "GetReview")]
        public async Task<IActionResult> GetReview(int id, [FromQuery]string fields)
        {
            if (!_typeHelperService.TypeHasProperties<ReviewReturnModel>(fields))
            {
                return BadRequest();
            }

            var review = await _reviewData.GetReview(id);

            if (review == null)
                return NotFound();

            var returnModel = Mapper.Map<ReviewReturnModel>(review);

            var links = CreateLinksForReview(id, fields);

            var returnModelWithLinks = returnModel.ShapeData(fields)
                as IDictionary<string, object>;

            returnModelWithLinks.Add("links", links);

            return Ok(returnModelWithLinks);
        }

        [HttpPost(Name = "CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreate creationModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var review = await _reviewData.CreateReview(creationModel);

                var returnModel = Mapper.Map<ReviewReturnModel>(review);

                var links = CreateLinksForReview(returnModel.Id, null);

                var returnModelWithLinks = returnModel.ShapeData(null)
                    as IDictionary<string, object>;

                returnModelWithLinks.Add("links", links);

                return CreatedAtRoute("GetReview", new { id = returnModel.Id }, returnModelWithLinks);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut(Name = "EditReview")]
        public async Task<IActionResult> EditReview([FromBody] ReviewEdit editModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                if (!await _reviewData.EditReview(editModel))
                    return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}", Name = "DeleteReview")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                if (!await _reviewData.DeleteReview(id))
                    return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{id}/comments", Name = "CreateComment")]
        public async Task<IActionResult> CreateComment(int id, [FromBody]CommentCreate createModel)
        {
            if (createModel == null || !ModelState.IsValid)
                return BadRequest();

            var review = await _reviewData.GetReview(id);

            if (review == null)
                return NotFound();

            try
            {
                await _reviewData.CreateComment(createModel);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}/comments/{commentId}", Name = "DeleteComment")]
        public async Task<IActionResult> DeleteComment(int id, int commentId)
        {
            var review = await _reviewData.GetReview(id);

            if (review == null)
                return NotFound();

            var comment = await _reviewData.GetComment(commentId);

            if (comment == null)
                return NotFound();

            await _reviewData.DeleteComment(id, comment);
            return NoContent();
        }

        [HttpPut("{reviewId}/like", Name = "LikeReview")]
        public async Task<IActionResult> LikeReview([FromRoute]int reviewId, [FromQuery]int reviewerId, [FromQuery]bool like)
        {
            var review = await _reviewData.GetReview(reviewId);
            var reviewer = await _reviewerData.GetReviewer(null, reviewerId);

            if (review == null || reviewer == null)
                return NotFound();

            if (review.NumberOfLikes == 0 && like == false)
                return BadRequest();

            await _reviewData.LikeReview(reviewId, reviewerId, like);

            var returnModel = Mapper.Map<ReviewReturnModel>(review);

            return Ok(returnModel);
        }

        private string CreateReviewsResourceUri(ReviewsResourceParameters reviewsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetReviews", new
                    {
                        fields = reviewsResourceParameters.Fields,
                        sortBy = reviewsResourceParameters.SortBy,
                        movieGenre = reviewsResourceParameters.MovieGenre,
                        pageNumber = reviewsResourceParameters.PageNumber - 1,
                        pageSize = reviewsResourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetReviews", new
                    {
                        fields = reviewsResourceParameters.Fields,
                        sortBy = reviewsResourceParameters.SortBy,
                        movieGenre = reviewsResourceParameters.MovieGenre,
                        pageNumber = reviewsResourceParameters.PageNumber + 1,
                        pageSize = reviewsResourceParameters.PageSize
                    });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetReviews", new
                    {
                        fields = reviewsResourceParameters.Fields,
                        sortBy = reviewsResourceParameters.SortBy,
                        movieGenre = reviewsResourceParameters.MovieGenre,
                        pageNumber = reviewsResourceParameters.PageNumber,
                        pageSize = reviewsResourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<ReturnModelLink> CreateLinksForReview(int id, string fields)
        {
            var links = new List<ReturnModelLink>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new ReturnModelLink(
                    _urlHelper.Link("GetReview", new { id = id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new ReturnModelLink(
                    _urlHelper.Link("GetReview", new { id = id, fields = fields }),
                    "self",
                    "GET"));
            }

            links.Add(new ReturnModelLink(
                _urlHelper.Link("DeleteReview", new { id = id }),
                "delete_review",
                "DELETE"));

            links.Add(new ReturnModelLink(
                _urlHelper.Link("CreateComment", new { id = id }),
                "create_comment",
                "POST"));

            return links;
        }

        private IEnumerable<ReturnModelLink> CreateLinksForReviews(
            ReviewsResourceParameters reviewsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<ReturnModelLink>();

            links.Add(new ReturnModelLink(CreateReviewsResourceUri(reviewsResourceParameters,
                ResourceUriType.Current),
                "self", "GET"));

            if (hasNext)
            {
                links.Add(new ReturnModelLink(CreateReviewsResourceUri(reviewsResourceParameters,
                        ResourceUriType.NextPage),
                        "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new ReturnModelLink(CreateReviewsResourceUri(reviewsResourceParameters,
                        ResourceUriType.PreviousPage),
                        "previousPage", "GET"));
            }

            return links;
        }
    }
}
