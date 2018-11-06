using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SciFiReviewsApi.Models.EntityModels
{
    public class Reviewer
    {
        private string _likedReviews;

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string Username { get; set; }

        public virtual User User { get; set; }

        public int NumberOfReviewsSubmitted { get; set; } = 0;

        public int NumberOfCommentsSubmitted { get; set; } = 0;

        [NotMapped]
        public string[] LikedReviews
        {
            get
            {
                return (_likedReviews != null) ? _likedReviews.Trim().Split(",") : new string[0];
            }
            set
            {
                _likedReviews = (value == null) ? null : string.Join(",", value);
            }
        }
    }
}
