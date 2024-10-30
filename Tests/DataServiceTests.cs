using DataLayer;
using DataLayer.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task AddUser_ValidData_ReturnsCreatedUser()
    {
      var newUser = await service.AddUserAsync("newUser", "password", "newUser@example.com");
      Assert.True(newUser.Id > 0);
      Assert.Equal("newUser", newUser.Username);

      var user = await service.GetUserAsync(newUser.Id);
      Assert.NotNull(user);
      Assert.Equal(newUser.Username, user.Username);
      Assert.Equal(newUser.Id, user.Id);

      // cleanup
      await service.DeleteUserAsync(newUser.Id);

      var deletedUser = await service.GetUserAsync(newUser.Id);
      Assert.Null(deletedUser);
    }

    [Fact]
    public async Task GetUser_InvalidId_ReturnsNull()
    {
      var user = await service.GetUserAsync(-1);
      Assert.Null(user);
    }

    /* UserRating Tests */

    [Fact]
    public async Task AddUserRating_ValidData_ReturnsCreatedRating()
    {
      // Ensure the user exists
      var newUser = await service.AddUserAsync("ratingUser", "password", "ratingUser@example.com");
      Assert.True(newUser.Id > 0);

      // Add rating for the newly created user
      var rating = await service.AddUserRatingAsync(newUser.Id, "tt26919084", 5);
      Assert.Equal(5, rating.Rating);

      var ratings = await service.GetUserRatingsAsync(newUser.Id);
      Assert.NotEmpty(ratings);
      Assert.Equal(5, ratings.First().Rating);

      // Cleanup
      await service.DeleteUserRatingAsync(rating.Id);
      await service.DeleteUserAsync(newUser.Id);

      var deletedRating = await service.GetUserRatingAsync(rating.Id);
      Assert.Null(deletedRating);

      await service.DeleteUserAsync(newUser.Id);
    }

    /* SearchHistory Tests */

    [Fact]
    public async Task AddSearchHistory_ValidQuery_ReturnsSearchHistory()
    {
      var newUser = await service.AddUserAsync("historyUser", "password", "historyUser@example.com");
      Assert.True(newUser.Id > 0);

      var history = await service.AddSearchHistoryAsync(newUser.Id, "testQuery");
      Assert.Equal("testQuery", history.SearchQuery);

      var historyList = await service.GetSearchHistoriesByUserAsync(newUser.Id);

      Assert.NotEmpty(historyList);
      Assert.Equal("testQuery", historyList.First().SearchQuery);

      // cleanup
      await service.DeleteSearchHistoryAsync(history.Id);

      var deletedHistory = await service.GetSearchHistoryAsync(history.Id);
      Assert.Null(deletedHistory);

      await service.DeleteUserAsync(newUser.Id);
    }

    /* UserBookmark Tests */

    [Fact]
    public async Task AddUserBookmark_ValidData_CreatesAndReturnsBookmark()
    {
      var newUser = await service.AddUserAsync("bookmarkUser", "password", "bookmarkUser@example.com");
      Assert.True(newUser.Id > 0);

      var bookmark = await service.AddBookmarkAsync(newUser.Id, "tt26919084", null, "Test note");
      Assert.True(bookmark.Id > 0);
      Assert.Equal("Test note", bookmark.Note);

      var retrievedBookmark = await service.GetBookmarkAsync(newUser.Id, bookmark.Id);
      Assert.Equal(bookmark.Id, retrievedBookmark.Id);
      Assert.Equal("Test note", retrievedBookmark.Note);

      // Cleanup
      await service.DeleteBookmarkAsync(bookmark.Id);

      var deletedBookmark = await service.GetBookmarkAsync(newUser.Id, bookmark.Id);
      Assert.Null(deletedBookmark);

      var bookmark1 = await service.AddBookmarkAsync(newUser.Id, null, "nm0000045", "Note 1");
      var bookmark2 = await service.AddBookmarkAsync(newUser.Id, "tt26919084", null, "Note 2");

      var bookmarks = await service.GetBookmarksAsync(newUser.Id);

      Assert.Contains(bookmarks, b => b.Note == "Note 1");
      Assert.Contains(bookmarks, b => b.Note == "Note 2");

      // Cleanup
      await service.DeleteBookmarkAsync(bookmark1.Id);
      await service.DeleteBookmarkAsync(bookmark2.Id);

      await service.DeleteUserAsync(newUser.Id);
    }
  }
}