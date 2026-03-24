using System.ComponentModel.DataAnnotations;
using COS4040A.Models;

namespace COS4040A.ViewModels
{
    public class CreateProjectViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Programming language is required.")]
        [StringLength(50, ErrorMessage = "Programming language cannot exceed 50 characters.")]
        public string ProgrammingLanguage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public ProjectStatus? Status { get; set; }
    }
}