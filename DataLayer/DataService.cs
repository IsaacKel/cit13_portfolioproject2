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

    // -- Helper Methods --

    private IList<T> GetPagedResults<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {
      return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    private bool SaveChangesWithValidation()
    {
      return _context.SaveChanges() > 0;
    }

    // -- USER --

    public User AddUser(string username, string password, string email)
    {
      var user = new User
      {
        Username = username,
        Password = password,
        Email = email
      };
      _context.Users.Add(user);
      SaveChangesWithValidation();
      return user;
    }

    public User GetUser(string username) => _context.Users.FirstOrDefault(u => u.Username == username);

    public User GetUser(int userId)
    {
      var user = _context.Users.Find(userId);
      return user;
    }

    public void DeleteUser(int userId)
    {
      var user = _context.Users.Find(userId);
      if (user != null)
      {
        _context.Users.Remove(user);
        SaveChangesWithValidation();
      }
    }

    // -- BOOKMARK --

    public IList<Bookmark> GetBookmarks(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.Bookmarks.Where(b => b.UserId == userId).OrderBy(b => b.Id);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public int GetBookmarkCountByUser(int userId) => _context.Bookmarks.Count(b => b.UserId == userId);

    public Bookmark GetBookmark(int userId, int bookmarkId)
    {
      return _context.Bookmarks.FirstOrDefault(b => b.UserId == userId && b.Id == bookmarkId);
    }

    public Bookmark AddBookmark(int userId, string? tconst, string? nconst, string note)
    {
      if (!_context.Users.Any(u => u.Id == userId))
        throw new ArgumentException("User with specified ID does not exist.");

      // Validate that either tconst or nconst is provided, but not both
      if ((tconst == null && nconst == null) || (tconst != null && nconst != null))
      {
        throw new ArgumentException("Either tconst or nconst must be provided, but not both.");
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
      SaveChangesWithValidation();
      return bookmark;
    }

    public void UpdateBookmark(int userId, int bookmarkId, string tconst, string nconst, string note)
    {
      var bookmark = GetBookmark(userId, bookmarkId);
      if (bookmark != null)
      {
        bookmark.TConst = tconst;
        bookmark.NConst = nconst;
        bookmark.Note = note;
        bookmark.CreatedAt = DateTime.UtcNow;
        SaveChangesWithValidation();
      }
    }

    public void DeleteBookmark(int bookmarkId)
    {
      var bookmark = _context.Bookmarks.Find(bookmarkId);
      if (bookmark != null)
      {
        _context.Bookmarks.Remove(bookmark);
        SaveChangesWithValidation();
      }
    }

    // -- SEARCH HISTORY --

    public IList<SearchHistory> GetSearchHistory(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderBy(sh => sh.CreatedAt);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public IList<SearchHistory> GetSearchHistoriesByUser(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderByDescending(sh => sh.CreatedAt);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public int GetSearchHistoryCountByUser(int userId) => _context.SearchHistories.Count(sh => sh.UserId == userId);

    public SearchHistory GetSearchHistory(int searchId)
    {
      return _context.SearchHistories.FirstOrDefault(sh => sh.Id == searchId);
    }

    public SearchHistory AddSearchHistory(int userId, string searchQuery)
    {
      var searchHistory = new SearchHistory
      {
        UserId = userId,
        SearchQuery = searchQuery,
        CreatedAt = DateTime.UtcNow
      };
      _context.SearchHistories.Add(searchHistory);
      SaveChangesWithValidation();
      return searchHistory;
    }

    public void DeleteSearchHistory(int searchId)
    {
      var searchHistory = _context.SearchHistories.Find(searchId);
      if (searchHistory != null)
      {
        _context.SearchHistories.Remove(searchHistory);
        SaveChangesWithValidation();
      }
    }

    // -- USER RATING --

    public IList<UserRating> GetUserRatings(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.UserRatings.Where(ur => ur.UserId == userId).OrderBy(ur => ur.Id);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public int GetUserRatingCount(int userId) => _context.UserRatings.Count(ur => ur.UserId == userId);

    public UserRating GetUserRating(int ratingId)
    {
      return _context.UserRatings.FirstOrDefault(ur => ur.Id == ratingId);
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
      SaveChangesWithValidation();
      return userRating;
    }

    public void UpdateUserRating(int userId, int ratingId, int rating)
    {
      var userRating = GetUserRating(ratingId);
      if (userRating != null)
      {
        userRating.Rating = rating;
        userRating.CreatedAt = DateTime.UtcNow;
        SaveChangesWithValidation();
      }
    }

    public void DeleteUserRating(int ratingId)
    {
      var userRating = _context.UserRatings.Find(ratingId);
      if (userRating != null)
      {
        _context.UserRatings.Remove(userRating);
        SaveChangesWithValidation();
      }
    }
  }
}