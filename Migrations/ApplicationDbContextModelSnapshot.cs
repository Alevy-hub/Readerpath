﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Readerpath.Data;

#nullable disable

namespace Readerpath.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Readerpath.Entities.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Readerpath.Entities.Bingo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Bingos");
                });

            modelBuilder.Entity("Readerpath.Entities.BingoField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BingoId")
                        .HasColumnType("int");

                    b.Property<int?>("BookActionId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("BingoId");

                    b.HasIndex("BookActionId");

                    b.ToTable("BingoFields");
                });

            modelBuilder.Entity("Readerpath.Entities.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GenreId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("Readerpath.Entities.BookAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateFinished")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateStarted")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("EditionId")
                        .HasColumnType("int");

                    b.Property<string>("Opinion")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<float?>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("EditionId");

                    b.ToTable("BookActions");
                });

            modelBuilder.Entity("Readerpath.Entities.ChallengeColors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ColorForFive")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ColorForFour")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ColorForNoRating")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ColorForOne")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ColorForThree")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("ColorForTwo")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("ChallengeColors");
                });

            modelBuilder.Entity("Readerpath.Entities.Edition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<int?>("Pages")
                        .HasColumnType("int");

                    b.Property<int?>("PublisherId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("PublisherId");

                    b.ToTable("Editions");
                });

            modelBuilder.Entity("Readerpath.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Readerpath.Entities.MonthBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BestBookId")
                        .HasColumnType("int");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("WorstBookId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BestBookId");

                    b.HasIndex("WorstBookId");

                    b.ToTable("MonthBooks");
                });

            modelBuilder.Entity("Readerpath.Entities.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("Readerpath.Entities.TBR", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("TBRs");
                });

            modelBuilder.Entity("Readerpath.Entities.UpdatePromptSeen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("UpdatePromptSeen");
                });

            modelBuilder.Entity("Readerpath.Entities.YearBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BestBookId")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("WorstBookId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BestBookId");

                    b.HasIndex("WorstBookId");

                    b.ToTable("YearBooks");
                });

            modelBuilder.Entity("Readerpath.Entities.YearChallenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("CongratsShowed")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .UseCollation("utf8mb4_unicode_ci")
                        .HasAnnotation("MySQL:Charset", "utf8mb4");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("YearChallenges");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Readerpath.Entities.BingoField", b =>
                {
                    b.HasOne("Readerpath.Entities.Bingo", "Bingo")
                        .WithMany()
                        .HasForeignKey("BingoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Readerpath.Entities.BookAction", "BookAction")
                        .WithMany()
                        .HasForeignKey("BookActionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Bingo");

                    b.Navigation("BookAction");
                });

            modelBuilder.Entity("Readerpath.Entities.Book", b =>
                {
                    b.HasOne("Readerpath.Entities.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Readerpath.Entities.Genre", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("Readerpath.Entities.BookAction", b =>
                {
                    b.HasOne("Readerpath.Entities.Edition", "Edition")
                        .WithMany()
                        .HasForeignKey("EditionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Edition");
                });

            modelBuilder.Entity("Readerpath.Entities.Edition", b =>
                {
                    b.HasOne("Readerpath.Entities.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Readerpath.Entities.Publisher", "Publisher")
                        .WithMany()
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Book");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("Readerpath.Entities.MonthBook", b =>
                {
                    b.HasOne("Readerpath.Entities.BookAction", "BestBook")
                        .WithMany()
                        .HasForeignKey("BestBookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Readerpath.Entities.BookAction", "WorstBook")
                        .WithMany()
                        .HasForeignKey("WorstBookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BestBook");

                    b.Navigation("WorstBook");
                });

            modelBuilder.Entity("Readerpath.Entities.TBR", b =>
                {
                    b.HasOne("Readerpath.Entities.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("Readerpath.Entities.YearBook", b =>
                {
                    b.HasOne("Readerpath.Entities.BookAction", "BestBook")
                        .WithMany()
                        .HasForeignKey("BestBookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Readerpath.Entities.BookAction", "WorstBook")
                        .WithMany()
                        .HasForeignKey("WorstBookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BestBook");

                    b.Navigation("WorstBook");
                });
#pragma warning restore 612, 618
        }
    }
}
