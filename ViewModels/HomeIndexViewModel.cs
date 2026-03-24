using COS4040A.Models;

namespace COS4040A.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Project> RecentCompletedProjects { get; set; } = new();
    }
}