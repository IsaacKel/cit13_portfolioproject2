// using System.Net;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Xunit;
// using cit13_portfolioproject2.Tests;

// namespace cit13_portfolioproject2.WebApiTests
// {
//   public class BookmarkControllerTests
//   {
//     private const string BookmarksApi = "http://localhost:5002/api/bookmark";

//     /* /api/bookmark */

//     [Fact]
//     public async Task ApiBookmarks_GetWithValidUserId_OkAndAllBookmarks()
//     {
//       int userId = 1;
//       var (data, statusCode) = await HelperTest.GetArray($"{BookmarksApi}/user/{userId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.NotNull(data);
//       Assert.True(data?.Count > 0);
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithInvalidUserId_NotFound()
//     {
//       int userId = 999;
//       var (_, statusCode) = await HelperTest.GetArray($"{BookmarksApi}/user/{userId}");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithValidBookmarkId_OkAndBookmark()
//     {
//       int userId = 1;
//       int bookmarkId = 1;
//       var (bookmark, statusCode) = await HelperTest.GetObject($"{BookmarksApi}/{bookmarkId}?userId={userId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.Equal(userId, int.Parse(bookmark?.Value("userId")?.ToString() ?? "0"));
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithInvalidBookmarkId_NotFound()
//     {
//       int userId = 1;
//       var (_, statusCode) = await HelperTest.GetObject($"{BookmarksApi}/999?userId={userId}");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     [Fact]
//     public async Task ApiBookmarks_PostWithBookmark_Created()
//     {
//       var newBookmark = new
//       {
//         UserId = 1,
//         TConst = "tt7704882",
//         NConst = (string?)null,
//         Note = "Test Bookmark"
//       };
//       var (bookmark, statusCode) = await HelperTest.PostData(BookmarksApi, newBookmark);

//       Assert.Equal(HttpStatusCode.Created, statusCode);
//       Assert.NotNull(bookmark);
//       Assert.Equal(newBookmark.UserId, int.Parse(bookmark?.Value("userId")?.ToString() ?? "0"));
//       Assert.Equal(newBookmark.TConst, bookmark?.Value("tConst"));

//       // Clean up after test
//       string? id = bookmark?["id"]?.ToString();
//       if (id != null)
//       {
//         await HelperTest.DeleteData($"{BookmarksApi}/{id}");
//       }
//     }

//     [Fact]
//     public async Task ApiBookmarks_PutWithValidBookmark_Ok()
//     {
//       var initialBookmark = new
//       {
//         UserId = 1,
//         TConst = "tt7704882",
//         NConst = (string?)null,
//         Note = "Initial Note"
//       };
//       var (bookmark, _) = await HelperTest.PostData(BookmarksApi, initialBookmark);

//       // Use the selfLink directly as the putUrl
//       string? putUrl = bookmark?["selfLink"]?.ToString();

//       var update = new
//       {
//         UserId = 1,
//         TConst = "tt7704882",
//         NConst = (string?)null,
//         Note = "Updated Note"
//       };

//       var statusCode = await HelperTest.PutData(putUrl, update);
//       Assert.Equal(HttpStatusCode.NoContent, statusCode);

//       // Verify update
//       var (updatedBookmark, _) = await HelperTest.GetObject(putUrl);

//       // Clean up after test
//       await HelperTest.DeleteData(putUrl);
//     }

//     [Fact]
//     public async Task ApiBookmarks_DeleteWithValidId_Ok()
//     {
//       var newBookmark = new
//       {
//         UserId = 1,
//         TConst = "tt7704882",
//         NConst = (string?)null,
//         Note = "Test Bookmark"
//       };
//       var (bookmark, _) = await HelperTest.PostData(BookmarksApi, newBookmark);

//       // Extract selfLink from the bookmark
//       string? deleteUrl = bookmark?["selfLink"]?.ToString();

//       var statusCode = await HelperTest.DeleteData(deleteUrl);

//       Assert.Equal(HttpStatusCode.NoContent, statusCode);
//     }
//   }
// }