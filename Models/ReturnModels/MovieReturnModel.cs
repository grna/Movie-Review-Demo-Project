using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.OutgoingModels;
using System.Collections.Generic;

namespace SciFiReviewsApi.Models.ReturnModels
{
    public class MovieReturnModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int ReleaseYear { get; set; }

        public string Genre { get; set; }

        public string ImagePath { get; set; }

        public float AverageRating { get; set; }

        public IEnumerable<ReviewReturnModel> MovieReviews { get; set; } = new List<ReviewReturnModel>();
    }
}
