using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.DtoModels
{
    public class MovieCreate
    {
        [Required, MaxLength(50)]
        public string Title { get; set; }

        [Required, MaxLength(40)]
        public string Author { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        public string Genre { get; set; }
    }
}
