using SciFiReviewsApi.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.DtoModels
{
    public class ReviewCreate
    {
        public MovieCreate Movie { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public DateTime SubmitTime { get; set; }

        [Required]
        public string ReviewerName { get; set; }
    }
}
