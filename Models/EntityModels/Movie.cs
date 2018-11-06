using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.EntityModels
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [Required, MaxLength(40)]
        public string Author { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        public string Genre { get; set; }

        public string ImagePath { get; set; }

        [Required]
        public float AverageRating { get; set; }
    }
}
