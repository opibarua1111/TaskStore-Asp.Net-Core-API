using Microsoft.EntityFrameworkCore;
using TaskStore.Model;

namespace TaskStore.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Model.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
