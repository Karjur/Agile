using Backend.Model;
using System.ComponentModel.DataAnnotations;

namespace Backend.ModelView
{
    public class ApplicationView
    {
        [Required] public int id { get; set; }
        [Required] public string? description { get; init; }
        [Required] public DateTime entryDate { get; set; }
        [Required] public DateTime solveDate { get; set; }
        public bool hasBeenCompleted { get; set; }
        public bool isDueSoon { get; set; }
    }
}