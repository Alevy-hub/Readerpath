using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readerpath.Entities;

namespace Readerpath.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Bingo> Bingo { get; set; }
        public DbSet<BingoField> BingoField { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<BookAction> BookAction { get; set; }
        public DbSet<Edition> Edition { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<MonthBook> MonthBook { get; set; }
        public DbSet<TBR> TBR { get; set; }
        public DbSet<YearBook> YearBook { get; set; }
        public DbSet<YearChallenge> YearChallenge { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}