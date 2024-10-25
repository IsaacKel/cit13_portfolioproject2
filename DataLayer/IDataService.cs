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
    IList<Bookmark> GetBookmarks(int userId);
    Bookmark GetBookmark(int userId, int bookmarkId);
    Bookmark AddBookmark(int userId, string tconst, string nconst, string note);
    void UpdateBookmark(int userId, int bookmarkId, string tconst, string nconst, string note);
    void DeleteBookmark(int userId, int bookmarkId);

    // --SEARCH HISTORY--
    IList<SearchHistory> GetSearchHistory(int userId);
    SearchHistory GetSearchHistory(int userId, string searchQuery, DateTime createdAt);
    SearchHistory AddSearchHistory(int userId, string searchQuery, DateTime createdAt);
    void DeleteSearchHistory(int userId, string searchQuery, DateTime createdAt);

    // --USER RATING--
    IList<UserRating> GetUserRatings(int userId);
    UserRating GetUserRating(int userId, string tconst);
    UserRating AddUserRating(int userId, string tconst, int rating);
    void DeleteUserRating(int userId, string tconst);
  }
}