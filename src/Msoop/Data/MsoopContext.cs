using System;
using Microsoft.EntityFrameworkCore;
using Msoop.Models;

namespace Msoop.Data
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
            modelBuilder.Entity<Sheet>().Property(s => s.LinksAgeLimitInDays).HasDefaultValue(14);
            modelBuilder.Entity<Subreddit>().HasKey(sub => new {sub.SheetId, sub.Name});
        }
    }
}
