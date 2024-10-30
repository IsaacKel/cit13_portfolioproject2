using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    // public DataService(MovieDbContext context)
    // {
    //   _context = context;
    // }


    // -- Helper Methods --

    private async Task<IList<T>> GetPagedResultsAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class =>
        await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    private async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    private async Task<bool> ExistsAsync<T>(int id) where T : class =>
        await _context.Set<T>().FindAsync(id) != null;

    private async Task<T> FindByIdAsync<T>(int id) where T : class =>
        await _context.Set<T>().FindAsync(id);

    // -- USER --

    public async Task<User> AddUserAsync(string username, string password, string email)
    {
      var user = new User { Username = username, Password = password, Email = email };
      _context.Users.Add(user);
      await SaveChangesAsync();
      return user;
    }

    public async Task<User> GetUserAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User> GetUserAsync(int userId) =>
        await FindByIdAsync<User>(userId);

    public async Task DeleteUserAsync(int userId)
    {
      var user = await FindByIdAsync<User>(userId);
      if (user != null)
      {
        _context.Users.Remove(user);
        await SaveChangesAsync();
      }
    }

    // -- BOOKMARK --

    public async Task<IList<Bookmark>> GetBookmarksAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.Bookmarks.Where(b => b.UserId == userId).OrderBy(b => b.Id);
      return await GetPagedResultsAsync(query, pageNumber, pageSize);
    }

    public async Task<int> GetBookmarkCountByUserAsync(int userId) =>
        await _context.Bookmarks.CountAsync(b => b.UserId == userId);

    public async Task<Bookmark> GetBookmarkByIdAsync(int bookmarkId) =>
        await _context.Bookmarks.FirstOrDefaultAsync(b => b.Id == bookmarkId);

    public async Task<Bookmark> GetBookmarkAsync(int userId, int bookmarkId) =>
        await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == bookmarkId);

    public async Task<Bookmark> AddBookmarkAsync(int userId, string? tconst, string? nconst, string note)
    {
      await ValidateUserExistsAsync(userId);

      if ((tconst == null && nconst == null) || (tconst != null && nconst != null))
        throw new ArgumentException("Specify either tconst or nconst, not both.");

      var bookmark = new Bookmark
      {
        UserId = userId,
        TConst = tconst,
        NConst = nconst,
        Note = note,
        CreatedAt = DateTime.UtcNow
      };
      _context.Bookmarks.Add(bookmark);
      await SaveChangesAsync();
      return bookmark;
    }

    public async Task UpdateBookmarkAsync(int userId, int bookmarkId, string tconst, string nconst, string note)
    {
      var bookmark = await GetBookmarkAsync(userId, bookmarkId);
      if (bookmark != null)
      {
        bookmark.TConst = tconst;
        bookmark.NConst = nconst;
        bookmark.Note = note;
        bookmark.CreatedAt = DateTime.UtcNow;
        await SaveChangesAsync();
      }
    }

    public async Task DeleteBookmarkAsync(int bookmarkId)
    {
      var bookmark = await FindByIdAsync<Bookmark>(bookmarkId);
      if (bookmark != null)
      {
        _context.Bookmarks.Remove(bookmark);
        await SaveChangesAsync();
      }
    }

    // -- SEARCH HISTORY --

    public async Task<IList<SearchHistory>> GetSearchHistoryAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderBy(sh => sh.CreatedAt);
      return await GetPagedResultsAsync(query, pageNumber, pageSize);
    }

    public async Task<SearchHistory> GetSearchHistoryAsync(int searchId) =>
        await FindByIdAsync<SearchHistory>(searchId);

    public async Task<IList<SearchHistory>> GetSearchHistoriesByUserAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderBy(sh => sh.CreatedAt);
      return await GetPagedResultsAsync(query, pageNumber, pageSize);
    }

    public async Task<int> GetSearchHistoryCountByUserAsync(int userId) =>
        await _context.SearchHistories.CountAsync(sh => sh.UserId == userId);

    public async Task<SearchHistory> AddSearchHistoryAsync(int userId, string searchQuery)
    {
      var searchHistory = new SearchHistory
      {
        UserId = userId,
        SearchQuery = searchQuery,
        CreatedAt = DateTime.UtcNow
      };
      _context.SearchHistories.Add(searchHistory);
      await SaveChangesAsync();
      return searchHistory;
    }

    public async Task DeleteSearchHistoryAsync(int searchId)
    {
      var searchHistory = await FindByIdAsync<SearchHistory>(searchId);
      if (searchHistory != null)
      {
        _context.SearchHistories.Remove(searchHistory);
        await SaveChangesAsync();
      }
    }

    // -- USER RATING --

    public async Task<IList<UserRating>> GetUserRatingsAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.UserRatings.Where(ur => ur.UserId == userId).OrderBy(ur => ur.Id);
      return await GetPagedResultsAsync(query, pageNumber, pageSize);
    }

    public async Task<UserRating> GetUserRatingAsync(int ratingId) =>
        await FindByIdAsync<UserRating>(ratingId);

    public async Task<UserRating> GetUserRatingByUserAndTConstAsync(int userId, string tconst) =>
        await _context.UserRatings.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.TConst == tconst);

    public async Task<int> GetUserRatingCountAsync(int userId) =>
        await _context.UserRatings.CountAsync(ur => ur.UserId == userId);

    public async Task<UserRating> AddUserRatingAsync(int userId, string tconst, int rating)
    {
      var userRating = new UserRating
      {
        UserId = userId,
        TConst = tconst,
        Rating = rating,
        CreatedAt = DateTime.UtcNow
      };
      _context.UserRatings.Add(userRating);
      await SaveChangesAsync();
      return userRating;
    }

    public async Task UpdateUserRatingAsync(int ratingId, int rating)
    {
      var userRating = await FindByIdAsync<UserRating>(ratingId);
      if (userRating != null)
      {
        userRating.Rating = rating;
        userRating.CreatedAt = DateTime.UtcNow;
        await SaveChangesAsync();
      }
    }

    public async Task UpdateUserRatingAsync(int userId, int ratingId, int rating)
    {
      var userRating = await GetUserRatingAsync(ratingId);
      if (userRating != null)
      {
        userRating.Rating = rating;
        userRating.CreatedAt = DateTime.UtcNow;
        await SaveChangesAsync();
      }
    }

    public async Task DeleteUserRatingAsync(int ratingId)
    {
      var userRating = await FindByIdAsync<UserRating>(ratingId);
      if (userRating != null)
      {
        _context.UserRatings.Remove(userRating);
        await SaveChangesAsync();
      }
    }

    // -- Private Helper Methods --

    private async Task ValidateUserExistsAsync(int userId)
    {
      if (!await ExistsAsync<User>(userId))
        throw new ArgumentException("User with specified ID does not exist.");
    }
  }
}