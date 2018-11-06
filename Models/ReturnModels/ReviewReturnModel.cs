using SciFiReviewsApi.Models.ReturnModels;
using System;
using System.Collections.Generic;

namespace SciFiReviewsApi.Models.OutgoingModels
{
    public class ReviewReturnModel : LinkedResourceBase
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public float Rating { get; set; }

        public DateTime SubmitTime { get; set; }

        public MovieReturnModel Movie { get; set; }

        public ReviewerReturnModel Reviewer { get; set; }

        public ICollection<CommentReturnModel> Comments { get; set; }

        public int NumberOfLikes { get; set; }

        public List<string> PeopleWhoLikedReview { get; set; }
    }
}
