using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Comment
{
    public class CommentCreateDto
    {
        [StringLength(250, ErrorMessage = "Review text is longer than 250 characters")]
        public string? Review { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public required int Rating { get; set; }

        [Required(ErrorMessage = "Salon id is required")]
        public required int SalonId { get; set; }

        [Required(ErrorMessage = "Customer is is required")]
        public required int CustomerId { get; set; }
    }
}
