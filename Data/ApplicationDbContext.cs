using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
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
        public DbSet<TBRBook> TBRBooks { get; set; }
        public DbSet<YearBook> YearBooks { get; set; }
        public DbSet<YearChallenge> YearChallenges { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ChallengeColors> ChallengeColors { get; set; }
        public DbSet<UpdatePromptSeen> UpdatePromptSeen { get; set; }

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
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        property.SetColumnType("varchar(255)");
                        property.SetCharSet("utf8mb4");
                        property.SetCollation("utf8mb4_unicode_ci");
                    }
                }
            }
        }

    }
}