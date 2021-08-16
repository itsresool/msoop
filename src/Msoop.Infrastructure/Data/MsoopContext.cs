using Microsoft.EntityFrameworkCore;
using Msoop.Core.Models;

namespace Msoop.Infrastructure.Data
{
    public class MsoopContext : DbContext
    {
        public MsoopContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Sheet> Sheets { get; set; }
        public DbSet<Subreddit> Subreddits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sheet>().Property(s => s.PostAgeLimitInDays).HasDefaultValue(value: 7);

            modelBuilder.Entity<Subreddit>().HasKey(sub => new { sub.SheetId, sub.Name });
        }
    }
}
