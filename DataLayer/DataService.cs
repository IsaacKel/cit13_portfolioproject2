using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer
{
  public class DataService : IDataService
  {
    private readonly MovieDbContext _context;

    public DataService(IConfiguration configuration)
    {
      var options = new DbContextOptionsBuilder<MovieDbContext>()
        .UseNpgsql(configuration.GetConnectionString("MovieDb"))
        .Options;

      _context = new MovieDbContext(options);
    }

    // --BOOKMARKS--
    public IList<Bookmark> GetBookmarks()
    {
      //return _context.Bookmarks.ToList();
      return _context.Bookmarks
            .Include(b => b.User)
            .Select(b => new Bookmark
            {
              Id = b.Id,
              TConst = b.TConst,
              NConst = b.NConst,
              Note = b.Note,
              CreatedAt = b.CreatedAt,
              UpdatedAt = b.UpdatedAt,
              User = new User
              {
                Id = b.User.Id,
                Username = b.User.Username,
                Password = b.User.Password,
                Email = b.User.Email,
                CreatedAt = b.User.CreatedAt,
                UpdatedAt = b.User.UpdatedAt
              }
            })
            .ToList();
    }

    public Bookmark GetBookmark(int id)
    {
      return _context.Bookmarks.FirstOrDefault(b => b.Id == id);
    }

    public void AddBookmark(Bookmark bookmark)
    {
      _context.Bookmarks.Add(bookmark);
      _context.SaveChanges();
    }

    public void UpdateBookmark(Bookmark bookmark)
    {
      _context.Bookmarks.Update(bookmark);
      _context.SaveChanges();
    }

    public void DeleteBookmark(int id)
    {
      var bookmark = _context.Bookmarks.FirstOrDefault(b => b.Id == id);
      _context.Bookmarks.Remove(bookmark);
      _context.SaveChanges();
    }

    // --SEARCH HISTORY--
    public IList<SearchHistory> GetSearchHistory()
    {
      return _context.SearchHistories.ToList();
    }

    public SearchHistory GetSearchHistory(int id)
    {
      return _context.SearchHistories.FirstOrDefault(s => s.UserId == id);
    }

    public void AddSearchHistory(SearchHistory searchHistory)
    {
      _context.SearchHistories.Add(searchHistory);
      _context.SaveChanges();
    }

    public void UpdateSearchHistory(SearchHistory searchHistory)
    {
      _context.SearchHistories.Update(searchHistory);
      _context.SaveChanges();
    }

    public void DeleteSearchHistory(int id)
    {
      var searchHistory = _context.SearchHistories.FirstOrDefault(s => s.UserId == id);
      _context.SearchHistories.Remove(searchHistory);
      _context.SaveChanges();
    }

    // --USER--
    public IList<User> GetUsers()
    {
      return _context.Users.ToList();
    }

    public User GetUser(int id)
    {
      return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public void AddUser(User user)
    {
      _context.Users.Add(user);
      _context.SaveChanges();
    }

    public void UpdateUser(User user)
    {
      _context.Users.Update(user);
      _context.SaveChanges();
    }

    public void DeleteUser(int id)
    {
      var user = _context.Users.FirstOrDefault(u => u.Id == id);
      _context.Users.Remove(user);
      _context.SaveChanges();
    }

    // --USER RATINGS--
    public IList<UserRating> GetUserRatings()
    {
      return _context.UserRatings.ToList();
    }

    public UserRating GetUserRating(int userId, int tConst)
    {
      return _context.UserRatings.FirstOrDefault(ur => ur.UserId == userId && ur.TConst == tConst);
    }

    public void AddUserRating(UserRating userRating)
    {
      _context.UserRatings.Add(userRating);
      _context.SaveChanges();
    }

    public void UpdateUserRating(UserRating userRating)
    {
      _context.UserRatings.Update(userRating);
      _context.SaveChanges();
    }

    public void DeleteUserRating(int userId, int tConst)
    {
      var userRating = _context.UserRatings.FirstOrDefault(ur => ur.UserId == userId && ur.TConst == tConst);
      _context.UserRatings.Remove(userRating);
      _context.SaveChanges();
    }
  }
}