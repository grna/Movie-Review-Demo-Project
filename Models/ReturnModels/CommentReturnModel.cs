using System;

namespace SciFiReviewsApi.Models.ReturnModels
{
    public class CommentReturnModel
    {
        public int Id { get; set; }

        public int ReviewerId { get; set; }

        public string ReviewerName { get; set; }

        public string Body { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
