using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ISBN { get; set; } = string.Empty;

        public string? Publisher { get; set; }

        [Required]
        [Range(1000, 9999)]
        public int PublishYear { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select an author")]
        public int AuthorId { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Author> Authors { get; set; } = new List<Author>();
    }
}


