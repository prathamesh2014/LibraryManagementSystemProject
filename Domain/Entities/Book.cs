using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Book : BaseEntity
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publisher is required")]
        [StringLength(100)]
        public string Publisher { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publish year is required")]
        [Range(1500, 2100, ErrorMessage = "Enter a valid publish year")]
        [Display(Name = "Publish Year")]
        public int PublishYear { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Total copies must be at least 1")]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        [Display(Name = "Cover Image")]
        public string? ImagePath { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public ICollection<IssueBook> IssueBooks { get; set; } = new List<IssueBook>();
    }
}
