using System.ComponentModel.DataAnnotations;

namespace COS4040A.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "In-Progress")]
        InProgress = 1,

        [Display(Name = "Complete")]
        Complete = 2
    }
}