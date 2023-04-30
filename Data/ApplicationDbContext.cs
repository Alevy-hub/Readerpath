using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readerpath.Entities;


namespace Readerpath.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Bingo> Bingos { get; set; }
        public DbSet<BingoField> BingoFields { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAction> BookActions { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MonthBook> MonthBooks { get; set; }
        public DbSet<TBR> TBRs { get; set; }
        public DbSet<YearBook> YearBooks { get; set; }
        public DbSet<YearChallenge> YearChallenges { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ChallengeColors> ChallengeColors { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		public ApplicationDbContext()
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("utf8mb4_unicode_ci");
            modelBuilder.HasDefaultSchema("ReaderPathDb");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}