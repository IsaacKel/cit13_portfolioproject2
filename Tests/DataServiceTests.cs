using DataLayer;
using DataLayer.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Assignment4.Tests
{
  public class UserBookmarkTests
  {
    private IConfiguration configuration;
    private DataService service;

    public UserBookmarkTests()
    {
      var inMemorySettings = new Dictionary<string, string?>
            {
                {"ConnectionStrings:imdbDatabase", "Host=localhost;Database=imdb;Username=postgres;Password=kelsall"}
            };

      configuration = new ConfigurationBuilder()
          .AddInMemoryCollection(inMemorySettings)
          .Build();

      service = new DataService(configuration);
    }

    /* User Tests */

    [Fact]
    public void AddUser_ValidData_ReturnsCreatedUser()
    {
      var newUser = service.AddUser("newUser", "password", "newUser@example.com");
      Assert.True(newUser.Id > 0);
      Assert.Equal("newUser", newUser.Username);

      var user = service.GetUser(newUser.Id);
      Assert.NotNull(user);
      Assert.Equal(newUser.Username, user.Username);
      Assert.Equal(newUser.Id, user.Id);

      // cleanup
      service.DeleteUser(newUser.Id);

      var deletedUser = service.GetUser(newUser.Id);
      Assert.Null(deletedUser);
    }

    [Fact]
    public void GetUser_InvalidId_ReturnsNull()
    {
      var user = service.GetUser(-1);
      Assert.Null(user);
    }

    /* UserRating Tests */

    [Fact]
    public void AddUserRating_ValidData_ReturnsCreatedRating()
    {
      // Ensure the user exists
      var newUser = service.AddUser("ratingUser", "password", "ratingUser@example.com");
      Assert.True(newUser.Id > 0);

      // Add rating for the newly created user
      var rating = service.AddUserRating(newUser.Id, "tt26919084", 5);
      Assert.Equal(5, rating.Rating);

      var ratings = service.GetUserRatings(newUser.Id);
      Assert.NotEmpty(ratings);
      Assert.Equal(5, ratings.First().Rating);

      // Cleanup
      service.DeleteUserRating(rating.Id);
      service.DeleteUser(newUser.Id);

      var deletedRating = service.GetUserRating(rating.Id);
      Assert.Null(deletedRating);
    }

    /* SearchHistory Tests */

    [Fact]
    public void AddSearchHistory_ValidQuery_ReturnsSearchHistory()
    {

      var newUser = service.AddUser("historyUser", "password", "historyUser@example.com");
      Assert.True(newUser.Id > 0);

      var history = service.AddSearchHistory(newUser.Id, "testQuery");
      Assert.Equal("testQuery", history.SearchQuery);

      var historyList = service.GetSearchHistory(newUser.Id);
      Assert.NotEmpty(historyList);
      Assert.Equal("testQuery", historyList.First().SearchQuery);

      // cleanup
      service.DeleteSearchHistory(history.Id);

      var deletedHistory = service.GetSearchHistory(newUser.Id, history.Id);
      Assert.Null(deletedHistory);
    }

    // /* UserBookmark Tests */

    // [Fact]
    // public void AddUserBookmark_ValidData_CreatesAndReturnsBookmark()
    // {
    //   var bookmark = service.AddBookmark(1, "tt1234567", null, "Test note");
    //   Assert.True(bookmark.Id > 0);
    //   Assert.Equal("Test note", bookmark.Note);

    //   // Cleanup
    //   service.DeleteBookmark(bookmark.Id);
    // }
    // [Fact]
    // public void GetBookmark_ValidId_ReturnsBookmark()
    // {
    //   var bookmark = service.AddBookmark(1, "tt1234567", null, "Test note");
    //   var retrievedBookmark = service.GetBookmark(1, bookmark.Id);

    //   Assert.Equal(bookmark.Id, retrievedBookmark.Id);
    //   Assert.Equal("Test note", retrievedBookmark.Note);

    //   // Cleanup
    //   service.DeleteBookmark(bookmark.Id);
    // }

    // [Fact]
    // public void GetBookmarks_ValidUserId_ReturnsBookmarks()
    // {
    //   var bookmark1 = service.AddBookmark(1, null, "nm0000045", "Note 1");
    //   var bookmark2 = service.AddBookmark(1, "tt1234568", null, "Note 2");

    //   var bookmarks = service.GetBookmarks(1);

    //   Assert.Contains(bookmarks, b => b.Note == "Note 1");
    //   Assert.Contains(bookmarks, b => b.Note == "Note 2");

    //   // Cleanup
    //   service.DeleteBookmark(bookmark1.Id);
    //   service.DeleteBookmark(bookmark2.Id);
    // }

    // [Fact]
    // public void DeleteBookmark_ValidId_RemovesBookmark()
    // {
    //   var bookmark = service.AddBookmark(1, "tt1234567", null, "Temporary note");
    //   service.DeleteBookmark(bookmark.Id);

    //   var deletedBookmark = service.GetBookmark(1, bookmark.Id);
    //   Assert.Null(deletedBookmark);
    // }
  }
}