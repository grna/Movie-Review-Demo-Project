using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.ReturnModels;
using SciFiReviewsApi.Services;

namespace SciFiReviewsApi.Controllers
{
    [Route("api/reviewers")]
    [ApiController]
    public class ReviewersController : ControllerBase
    {
        private IReviewerData _reviewerData;

        public ReviewersController(IReviewerData reviewerData)
        {
            _reviewerData = reviewerData;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewers()
        {
            var reviewers = await _reviewerData.GetReviewers();

            var returnModel = Mapper.Map<List<Reviewer>>(reviewers);

            return Ok(returnModel);
        }

        [HttpGet("{username}", Name = "GetReviewer")]
        public async Task<IActionResult> GetReviewer(string username, int? id)
        {
            var reviewer = await _reviewerData.GetReviewer(username, id);

            if (reviewer == null)
                return NotFound();

            var returnModel = Mapper.Map<ReviewerReturnModel>(reviewer);

            return Ok(returnModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReviewer(int userId)
        {
            try
            {
                Reviewer reviewer = await _reviewerData.CreateReviewer(userId);

                return CreatedAtRoute("GetReviewer", new { id = reviewer.Id }, reviewer);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewer(int id)
        {
            try
            {
                if (!await _reviewerData.DeleteReviewer(id))
                    return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}