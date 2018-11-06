using System;
using System.ComponentModel.DataAnnotations;

namespace SciFiReviewsApi.Models.EntityModels
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        public virtual Reviewer Reviewer { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
