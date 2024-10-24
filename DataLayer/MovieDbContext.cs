using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer;

public class MovieDbContext : MovieDbContext
{
  public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users { get; set; }
  public DbSet<Bookmark> Bookmarks { get; set; }
  public DbSet<UserRating> UserRatings { get; set; }
  public DbSet<SearchHistory> SearchHistories { get; set; }

  public override void OnModelCreating(ModelBuilder modelBuilder)
  {
    MapUsers(modelBuilder);
    MapBookmarks(modelBuilder);
    MapUserRatings(modelBuilder);
    MapSearchHistories(modelBuilder);
  }
  //User Table Mapping
  private static void MapUsers(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>().ToTable("users");
    modelBuilder.Entity<User>().HasKey(u => u.Id);
    modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("userId");
    modelBuilder.Entity<User>().Property(u => u.Username).HasColumnName("username");
    modelBuilder.Entity<User>().Property(u => u.Email).HasColumnName("email");
    modelBuilder.Entity<User>().Property(u => u.Password).HasColumnName("password");
  }

  //Bookmark Table Mapping
  private static void MapBookmarks(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Bookmark>().ToTable("bookmarks");
    modelBuilder.Entity<Bookmark>().HasKey(b => b.Id);
    modelBuilder.Entity<Bookmark>().Property(b => b.Id).HasColumnName("bookmarkId");
    modelBuilder.Entity<Bookmark>().Property(b => b.TConst).HasColumnName("tconst");
    modelBuilder.Entity<Bookmark>().Property(b => b.NConst).HasColumnName("nconst");
    modelBuilder.Entity<Bookmark>().Property(b => b.Note).HasColumnName("note");
    modelBuilder.Entity<Bookmark>().Property(b => b.CreatedAt).HasColumnName("bookmarkDate");
    // modelBuilder.Entity<Bookmark>().Property(b => b.UpdatedAt).HasColumnName("updated_at");

  }

  //User Ratings Table Mapping
  private static void MapUserRatings(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Bookmark>().ToTable("user_ratings");
    // modelBuilder.Entity<Bookmark>().HasKey(u => u.Id);
    modelBuilder.Entity<Bookmark>().Property(u => u.Id).HasColumnName("userId");
    modelBuilder.Entity<Bookmark>().Property(u => u.TConst).HasColumnName("tconst");
    modelBuilder.Entity<Bookmark>().Property(u => u.Rating).HasColumnName("rating");
    modelBuilder.Entity<Bookmark>().Property(u => u.CreatedAt).HasColumnName("ratingDate");
  }

  //Search History Table Mapping
  private static void MapSearchHistories(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Bookmark>().ToTable("search_history");
    modelBuilder.Entity<Bookmark>().HasKey(s => s.Id);
    modelBuilder.Entity<Bookmark>().Property(s => s.Id).HasColumnName("userId");
    modelBuilder.Entity<Bookmark>().Property(s => s.SearchQuery).HasColumnName("searchQuery");
    modelBuilder.Entity<Bookmark>().Property(s => s.CreatedAt).HasColumnName("searchDate");
  }
}

