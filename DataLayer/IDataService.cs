using DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer
{
  public interface IDataService
  {
    // --USER--
    Task<User> AddUserAsync(string username, string password, string email);
    Task<User> GetUserAsync(string username);
    Task<User> GetUserAsync(int userId);
    Task DeleteUserAsync(int userId);

    // --BOOKMARK--
    Task<IList<Bookmark>> GetBookmarksAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<Bookmark> GetBookmarkAsync(int userId, int bookmarkId);
    Task<Bookmark> GetBookmarkByIdAsync(int bookmarkId);
    Task<Bookmark> AddBookmarkAsync(int userId, string tconst, string nconst, string note);
    Task UpdateBookmarkAsync(int userId, int bookmarkId, string tconst, string nconst, string note);
    Task DeleteBookmarkAsync(int bookmarkId);
    Task<int> GetBookmarkCountByUserAsync(int userId);

    // --SEARCH HISTORY--
    Task<IList<SearchHistory>> GetSearchHistoryAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<IList<SearchHistory>> GetSearchHistoriesByUserAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<SearchHistory> GetSearchHistoryAsync(int searchId);
    Task<SearchHistory> AddSearchHistoryAsync(int userId, string searchQuery);
    Task DeleteSearchHistoryAsync(int searchId);
    Task<int> GetSearchHistoryCountByUserAsync(int userId);

    // --USER RATING--
    Task<IList<UserRating>> GetUserRatingsAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<UserRating> GetUserRatingAsync(int ratingId);
    Task<UserRating> GetUserRatingByUserAndTConstAsync(int userId, string tconst);
    Task<UserRating> AddUserRatingAsync(int userId, string tconst, int rating);
    Task DeleteUserRatingAsync(int ratingId);
    Task UpdateUserRatingAsync(int userId, int ratingId, int rating);
    Task<int> GetUserRatingCountAsync(int userId);
  }
}