using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;

namespace DataLayer;

public class MovieDbContext : DbContext
{
  public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users { get; set; }
  public DbSet<Bookmark> Bookmarks { get; set; }
  public DbSet<UserRating> UserRatings { get; set; }
  public DbSet<SearchHistory> SearchHistories { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
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
  }

  //User Ratings Table Mapping
  private static void MapUserRatings(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<UserRating>().ToTable("user_ratings");
    modelBuilder.Entity<UserRating>().HasKey(ur => new { ur.TConst, ur.UserId });
    modelBuilder.Entity<UserRating>().Property(u => u.UserId).HasColumnName("userId");
    modelBuilder.Entity<UserRating>().Property(u => u.TConst).HasColumnName("tconst");
    modelBuilder.Entity<UserRating>().Property(u => u.Rating).HasColumnName("rating");
    modelBuilder.Entity<UserRating>().Property(u => u.CreatedAt).HasColumnName("ratingDate");
  }

  //Search History Table Mapping
  private static void MapSearchHistories(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<SearchHistory>().ToTable("search_history");
    modelBuilder.Entity<SearchHistory>().HasKey(sh => new { sh.UserId, sh.SearchQuery, sh.CreatedAt });
    modelBuilder.Entity<SearchHistory>().Property(s => s.UserId).HasColumnName("userId");
    modelBuilder.Entity<SearchHistory>().Property(s => s.SearchQuery).HasColumnName("searchQuery");
    modelBuilder.Entity<SearchHistory>().Property(s => s.CreatedAt).HasColumnName("searchDate");
  }
}

