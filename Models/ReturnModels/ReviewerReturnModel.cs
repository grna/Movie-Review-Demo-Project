using System.Collections.Generic;

namespace SciFiReviewsApi.Models.ReturnModels
{
    public class ReviewerReturnModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public int NumberOfReviewsSubmitted { get; set; }

        public int NumberOfCommentsSubmitted { get; set; }

        public List<int> LikedReviews { get; set; }
    }
}
