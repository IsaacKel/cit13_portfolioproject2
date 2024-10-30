// using System.Net;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Xunit;

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
//       var (data, statusCode) = await GetArray($"{BookmarksApi}/user/{userId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.NotNull(data);
//       Assert.True(data?.Count > 0);
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithInvalidUserId_NotFound()
//     {
//       int userId = 999;
//       var (_, statusCode) = await GetArray($"{BookmarksApi}/user/{userId}");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithValidBookmarkId_OkAndBookmark()
//     {
//       int userId = 1;
//       int bookmarkId = 1;
//       var (bookmark, statusCode) = await GetObject($"{BookmarksApi}/{bookmarkId}?userId={userId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.Equal(userId, int.Parse(bookmark?.Value("userId")?.ToString() ?? "0"));
//     }

//     [Fact]
//     public async Task ApiBookmarks_GetWithInvalidBookmarkId_NotFound()
//     {
//       int userId = 1;
//       var (_, statusCode) = await GetObject($"{BookmarksApi}/999?userId={userId}");

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
//       var (bookmark, statusCode) = await PostData(BookmarksApi, newBookmark);

//       Assert.Equal(HttpStatusCode.Created, statusCode);
//       Assert.NotNull(bookmark);
//       Assert.Equal(newBookmark.UserId, int.Parse(bookmark?.Value("userId")?.ToString() ?? "0"));
//       Assert.Equal(newBookmark.TConst, bookmark?.Value("tConst"));

//       // Clean up after test
//       string? id = bookmark?["id"]?.ToString();
//       if (id != null)
//       {
//         await DeleteData($"{BookmarksApi}/{id}");
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
//       var (bookmark, _) = await PostData(BookmarksApi, initialBookmark);

//       // Use the selfLink directly as the putUrl
//       string? putUrl = bookmark?["selfLink"]?.ToString();

//       var update = new
//       {
//         UserId = 1,
//         TConst = "tt7704882",
//         NConst = (string?)null,
//         Note = "Updated Note"
//       };

//       var statusCode = await PutData(putUrl, update);
//       Assert.Equal(HttpStatusCode.NoContent, statusCode);

//       // Verify update
//       var (updatedBookmark, _) = await GetObject(putUrl);

//       // Clean up after test
//       await DeleteData(putUrl);
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
//       var (bookmark, _) = await PostData(BookmarksApi, newBookmark);

//       // Extract selfLink from the bookmark
//       string? deleteUrl = bookmark?["selfLink"]?.ToString();

//       var statusCode = await DeleteData(deleteUrl);

//       Assert.Equal(HttpStatusCode.NoContent, statusCode);
//     }

//     // Helpers

//     async Task<(JsonArray?, HttpStatusCode)> GetArray(string url)
//     {
//       var client = new HttpClient();
//       var response = await client.GetAsync(url);
//       var data = await response.Content.ReadAsStringAsync();

//       if (response.StatusCode == HttpStatusCode.NotFound)
//       {
//         return (new JsonArray(), HttpStatusCode.NotFound);
//       }

//       try
//       {
//         // Try to deserialize as a JsonArray first
//         var jsonArray = JsonSerializer.Deserialize<JsonArray>(data);
//         return (jsonArray, response.StatusCode);
//       }
//       catch (JsonException)
//       {
//         // If deserialization as an array fails, try parsing as a JsonObject instead
//         var jsonObject = JsonSerializer.Deserialize<JsonObject>(data);
//         if (jsonObject != null)
//         {
//           var wrappedArray = new JsonArray { jsonObject };
//           return (wrappedArray, response.StatusCode);
//         }
//         throw; // rethrow if it's neither array nor object
//       }
//     }

//     async Task<(JsonObject?, HttpStatusCode)> GetObject(string url)
//     {
//       var client = new HttpClient();
//       var response = await client.GetAsync(url);
//       var data = await response.Content.ReadAsStringAsync();
//       return (JsonSerializer.Deserialize<JsonObject>(data), response.StatusCode);
//     }

//     async Task<(JsonObject?, HttpStatusCode)> PostData(string url, object content)
//     {
//       var client = new HttpClient();
//       var requestContent = new StringContent(
//           JsonSerializer.Serialize(content),
//           Encoding.UTF8,
//           "application/json");
//       var response = await client.PostAsync(url, requestContent);
//       var data = await response.Content.ReadAsStringAsync();
//       return (JsonSerializer.Deserialize<JsonObject>(data), response.StatusCode);
//     }

//     async Task<HttpStatusCode> PutData(string url, object content)
//     {
//       var client = new HttpClient();
//       var response = await client.PutAsync(
//           url,
//           new StringContent(
//               JsonSerializer.Serialize(content),
//               Encoding.UTF8,
//               "application/json"));
//       return response.StatusCode;
//     }

//     async Task<HttpStatusCode> DeleteData(string url)
//     {
//       var client = new HttpClient();
//       var response = await client.DeleteAsync(url);
//       return response.StatusCode;
//     }
//   }

//   static class HelperExt
//   {
//     public static string? Value(this JsonNode node, string name)
//     {
//       var value = node[name];
//       return value?.ToString();
//     }
//   }
// }