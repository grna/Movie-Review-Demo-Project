using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.DtoModels
{
    public class SignUpModel
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        public DateTime DateOfBirth { get; }

        [Required, MaxLength(50)]
        public string Password { get; set; }

        [Required, MaxLength(50)]
        [Compare("Password")]
        public string RepeatPassword { get; set; }

        [Required, MaxLength(50)]
        public string EmailAddress { get; set; }
    }
}
