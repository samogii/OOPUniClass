using Microsoft.EntityFrameworkCore;
using OOPClass1.Models;

namespace OOPClass1.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
