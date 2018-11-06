using System.ComponentModel.DataAnnotations;

namespace SciFiReviewsApi.Models.IncomingModels
{
    public class CommentEdit
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
