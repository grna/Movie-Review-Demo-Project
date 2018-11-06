using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Models.EntityModels
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(50)]
        public string PasswordHash { get; set; }

        [Required]
        public string EmailAddress { get; set; }
    }
}
