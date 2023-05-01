using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Model
{
    public class Application
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string? description { get; init; }
        public DateTime entryDate { get; set; }
        public DateTime solveDate { get; set; }
        public bool hasBeenCompleted { get; set; }
        public bool isDueSoon { get; set; }
    }
}