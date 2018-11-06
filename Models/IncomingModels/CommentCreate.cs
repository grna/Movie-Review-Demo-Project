using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.DtoModels
{
    public class CommentCreate
    {
        [Required]
        public string ReviewerName { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
