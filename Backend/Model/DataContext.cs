using Microsoft.EntityFrameworkCore;

namespace Backend.Model
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Application>? Applications { get; set; }
    }
}