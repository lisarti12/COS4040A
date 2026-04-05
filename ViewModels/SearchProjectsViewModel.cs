using System.ComponentModel.DataAnnotations;

namespace COS4040A.ViewModels
{
    public class SearchProjectsViewModel
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;

        public List<SearchResultViewModel> Results { get; set; } = new();
    }
}