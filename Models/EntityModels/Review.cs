using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.EntityModels
{
    public class Review
    {
        private string _peopleWhoLikedReview;

        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public DateTime SubmitTime { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        public virtual Reviewer Reviewer { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public int NumberOfLikes { get; set; }

        [NotMapped]
        public string[] PeopleWhoLikedReview
        {
            get
            {
                return (_peopleWhoLikedReview != null) ? _peopleWhoLikedReview.Split(",") : null;
            }
            set
            {
                _peopleWhoLikedReview = (value == null) ? null : string.Join(",", value);
            }
        }
    }
}
