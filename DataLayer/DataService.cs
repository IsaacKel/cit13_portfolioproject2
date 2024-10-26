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
        .UseNpgsql(configuration.GetConnectionString("imdbDatabase"))
        .Options;

      _context = new MovieDbContext(options);
    }

    // --USER--
    public User AddUser(string username, string password, string email)
    {
      var user = new User
      {
        Username = username,
        Password = password,
        Email = email
      };
      _context.Users.Add(user);
      _context.SaveChanges();
      return user;
    }

    public User GetUser(string username)
    {
      return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public User GetUser(int userId)
    {
      return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    public void DeleteUser(int userId)
    {
      var user = _context.Users.FirstOrDefault(u => u.Id == userId);
      if (user != null)
      {
        _context.Users.Remove(user);
        _context.SaveChanges();
      }
    }

    // --BOOKMARK-- 
    // Get paginated bookmarks for a specific user
    public IList<Bookmark> GetBookmarks(int userId, int pageNumber = 1, int pageSize = 10)
    {
      // Apply pagination using Skip and Take
      return _context.Bookmarks
                     .Where(b => b.UserId == userId)
                     .OrderBy(b => b.Id)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();
    }

    // Get total count of bookmarks for a specific user (for pagination)
    public int GetBookmarkCount(int userId)
    {
      return _context.Bookmarks.Count(b => b.UserId == userId);
    }

    // Get a specific bookmark by userId and bookmarkId
    public Bookmark GetBookmark(int userId, int bookmarkId)
    {
      return _context.Bookmarks
                     .FirstOrDefault(b => b.UserId == userId && b.Id == bookmarkId);
    }

    // Add a new bookmark
    public Bookmark AddBookmark(int userId, string tconst, string nconst, string note)
    {
      // Check if user exists
      var userExists = _context.Users.Any(u => u.Id == userId);
      if (!userExists)
      {
        throw new ArgumentException("User with specified ID does not exist.");
      }

      // Check if tconst exists
      var titleExists = _context.Titles.Any(t => t.TConst == tconst);
      if (!titleExists)
      {
        throw new ArgumentException("The specified TConst does not exist in the Titles table.");
      }


      var bookmark = new Bookmark
      {
        UserId = userId,
        TConst = tconst,
        NConst = nconst,
        Note = note,
        CreatedAt = DateTime.UtcNow
      };
      _context.Bookmarks.Add(bookmark);
      _context.SaveChanges();
      return bookmark;
    }

    // Update an existing bookmark
    public void UpdateBookmark(int userId, int bookmarkId, string tconst, string nconst, string note)
    {
      var bookmark = _context.Bookmarks
                             .FirstOrDefault(b => b.UserId == userId && b.Id == bookmarkId);
      if (bookmark != null)
      {
        bookmark.TConst = tconst;
        bookmark.NConst = nconst;
        bookmark.Note = note;
        bookmark.CreatedAt = DateTime.UtcNow;
        _context.SaveChanges();
      }
    }

    // Delete a specific bookmark
    public void DeleteBookmark(int userId, int bookmarkId)
    {
      var bookmark = _context.Bookmarks
                             .FirstOrDefault(b => b.UserId == userId && b.Id == bookmarkId);
      if (bookmark != null)
      {
        _context.Bookmarks.Remove(bookmark);
        _context.SaveChanges();
      }
    }

    // --SEARCH HISTORY--
    // Method to retrieve paginated search history for a specific user
    public IList<SearchHistory> GetSearchHistory(int userId, int pageNumber = 1, int pageSize = 10)
    {
      return _context.SearchHistories
                     .Where(sh => sh.UserId == userId)
                     .OrderBy(sh => sh.CreatedAt)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();
    }

    // Method to retrieve a specific search history entry by userId, searchQuery, and createdAt
    public SearchHistory GetSearchHistory(int userId, string searchQuery, DateTime createdAt)
    {
      return _context.SearchHistories
                     .FirstOrDefault(sh => sh.UserId == userId
                                        && sh.SearchQuery == searchQuery
                                        && sh.CreatedAt == createdAt);
    }

    public SearchHistory AddSearchHistory(int userId, string searchQuery, DateTime createdAt)
    {
      var searchHistory = new SearchHistory
      {
        UserId = userId,
        SearchQuery = searchQuery,
        CreatedAt = DateTime.UtcNow
      };
      _context.SearchHistories.Add(searchHistory);
      _context.SaveChanges();
      return searchHistory;
    }

    public void DeleteSearchHistory(int userId, string searchQuery, DateTime createdAt)
    {
      var searchHistory = _context.SearchHistories
                                  .FirstOrDefault(sh => sh.UserId == userId
                                                     && sh.SearchQuery == searchQuery
                                                     && sh.CreatedAt == createdAt);
      if (searchHistory != null)
      {
        _context.SearchHistories.Remove(searchHistory);
        _context.SaveChanges();
      }
    }

    // --USER RATING--
    public IList<UserRating> GetUserRatings(int userId, int pageNumber = 1, int pageSize = 10)
    {
      return _context.UserRatings
                     .Where(ur => ur.UserId == userId)
                     .OrderBy(ur => ur.UserId)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();
    }

    public UserRating GetUserRating(int userId, string tconst)
    {
      return _context.UserRatings
                     .FirstOrDefault(ur => ur.UserId == userId
                                        && ur.TConst == tconst);
    }

    public UserRating AddUserRating(int userId, string tconst, int rating)
    {
      var userRating = new UserRating
      {
        UserId = userId,
        TConst = tconst,
        Rating = rating,
        CreatedAt = DateTime.UtcNow
      };
      _context.UserRatings.Add(userRating);
      _context.SaveChanges();
      return userRating;
    }

    public void DeleteUserRating(int userId, string tconst)
    {
      var userRating = _context.UserRatings
                               .FirstOrDefault(ur => ur.UserId == userId
                                                  && ur.TConst == tconst);
      if (userRating != null)
      {
        _context.UserRatings.Remove(userRating);
        _context.SaveChanges();
      }
    }

    public int GetUserRatingCount(int userId)
    {
      return _context.UserRatings
                     .Count(ur => ur.UserId == userId);
    }
  }
}