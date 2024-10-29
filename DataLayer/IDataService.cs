using DataLayer.Models;
using System.Collections.Generic;

namespace DataLayer
{
  public interface IDataService
  {
    // --USER--
    User AddUser(string username, string password, string email);
    User GetUser(string username);
    User GetUser(int userId);
    void DeleteUser(int userId);

    // --BOOKMARK--
    IList<Bookmark> GetBookmarks(int userId, int pageNumber = 1, int pageSize = 10);
    Bookmark GetBookmark(int userId, int bookmarkId);
    Bookmark AddBookmark(int userId, string tconst, string nconst, string note);
    void UpdateBookmark(int userId, int bookmarkId, string tconst, string nconst, string note);
    void DeleteBookmark(int bookmarkId);
    int GetBookmarkCountByUser(int userId);

    // --SEARCH HISTORY--
    IList<SearchHistory> GetSearchHistory(int userId, int pageNumber = 1, int pageSize = 10);
    IList<SearchHistory> GetSearchHistoriesByUser(int userId, int pageNumber = 1, int pageSize = 10);
    SearchHistory GetSearchHistory(int userId, int searchId);
    SearchHistory AddSearchHistory(int userId, string searchQuery);
    void DeleteSearchHistory(int searchId);
    int GetSearchHistoryCountByUser(int userId);

    // --USER RATING--
    IList<UserRating> GetUserRatings(int userId, int pageNumber = 1, int pageSize = 10);
    UserRating GetUserRating(int ratingId);
    UserRating AddUserRating(int userId, string tconst, int rating);
    void DeleteUserRating(int ratingId);
    void UpdateUserRating(int userId, int ratingId, int rating);
    int GetUserRatingCount(int userId);
  }
}