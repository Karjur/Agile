using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Application
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required] public int id { get; set; }
        [Required] public string? description { get; init; }
        [Required] public DateTime entryDate { get; set; }
        [Required] public DateTime solveDate { get; set; }
        public bool hasBeenCompleted { get; set; }
    }
}