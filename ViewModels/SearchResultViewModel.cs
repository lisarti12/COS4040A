using COS4040A.Models;

namespace COS4040A.ViewModels
{
    public class SearchResultViewModel
    {
        public Project Project { get; set; } = null!;
        public int MatchCount { get; set; }
    }
}