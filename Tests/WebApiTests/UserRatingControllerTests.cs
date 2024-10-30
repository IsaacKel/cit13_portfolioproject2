// using System.Net;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Xunit;

// namespace cit13_portfolioproject2.WebApiTests.UserRatingControllerTests
// {
//   public class UserRatingControllerTests
//   {
//     private const string UserRatingsApi = "http://localhost:5002/api/userrating";
//     private static readonly HttpClient client = new HttpClient();

//     [Fact]
//     public async Task ApiUserRatings_GetUserRatingsWithValidUserId_OkAndRatings()
//     {
//       int userId = 1;
//       var (ratings, statusCode) = await GetObject($"{UserRatingsApi}/{userId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.NotNull(ratings);
//     }

//     [Fact]
//     public async Task ApiUserRatings_GetUserRatingsWithInvalidUserId_NotFound()
//     {
//       int userId = 999;
//       var (_, statusCode) = await GetObject($"{UserRatingsApi}/{userId}");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     [Fact]
//     public async Task ApiUserRatings_GetUserRatingByIdWithValidIds_OkAndRating()
//     {
//       int userId = 1;
//       int ratingId = 1;
//       var (rating, statusCode) = await GetObject($"{UserRatingsApi}/{userId}/{ratingId}");

//       Assert.Equal(HttpStatusCode.OK, statusCode);
//       Assert.NotNull(rating);
//     }

//     [Fact]
//     public async Task ApiUserRatings_GetUserRatingByIdWithInvalidIds_NotFound()
//     {
//       int userId = 1;
//       int ratingId = 999;
//       var (_, statusCode) = await GetObject($"{UserRatingsApi}/{userId}/{ratingId}");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     [Fact]
//     public async Task ApiUserRatings_AddUserRating_ValidData_Created()
//     {
//       var newRating = new
//       {
//         UserId = 1,
//         TConst = "tt16120138",
//         Rating = 8,
//       };
//       var (rating, statusCode) = await PostData($"{UserRatingsApi}", newRating);

//       Console.WriteLine(rating);
//       Console.WriteLine(statusCode);
//       Console.WriteLine("newratintg" + newRating);

//       Assert.Equal(HttpStatusCode.Created, statusCode);
//       Assert.NotNull(rating);
//       Assert.Equal(newRating.UserId, rating?.ValueInt("userId"));
//       Assert.Equal(newRating.TConst, rating?.Value("tConst"));
//       Assert.Equal(newRating.Rating, rating?.ValueInt("rating"));

//       // Clean up after test
//       string? id = rating?["id"]?.ToString();
//       if (id != null)
//       {
//         await DeleteData($"{UserRatingsApi}/{newRating.UserId}/{id}");
//       }
//     }

//     [Fact]
//     public async Task ApiUserRatings_AddUserRating_InvalidData_BadRequest()
//     {
//       var invalidRating = new
//       {
//         UserId = 1,
//         TConst = "",
//         Rating = 11
//       };
//       var (_, statusCode) = await PostData($"{UserRatingsApi}", invalidRating);

//       Assert.Equal(HttpStatusCode.BadRequest, statusCode);
//     }

//     [Fact]
//     public async Task ApiUserRatings_DeleteUserRatingWithValidIds_NoContent()
//     {
//       var newRating = new
//       {
//         UserId = 1,
//         TConst = "tt16120138",
//         Rating = 8
//       };
//       var (rating, _) = await PostData($"{UserRatingsApi}", newRating);

//       string? deleteUrl = $"{UserRatingsApi}/{newRating.UserId}/{rating?["id"]?.ToString()}";
//       var statusCode = await DeleteData(deleteUrl);

//       Assert.Equal(HttpStatusCode.NoContent, statusCode);
//     }

//     [Fact]
//     public async Task ApiUserRatings_DeleteUserRatingWithInvalidIds_NotFound()
//     {
//       var statusCode = await DeleteData($"{UserRatingsApi}/1/999");

//       Assert.Equal(HttpStatusCode.NotFound, statusCode);
//     }

//     // [Fact]
//     // public async Task ApiUserRatings_UpdateUserRating_ValidData_NoContent()
//     // {
//     //   var newRating = new
//     //   {
//     //     UserId = 1,
//     //     TConst = "tt16120138",
//     //     Rating = 8
//     //   };
//     //   var (rating, _) = await PostData($"{UserRatingsApi}", newRating);

//     //   var updatedRating = new
//     //   {
//     //     UserId = 1,
//     //     TConst = "tt16120138",
//     //     Rating = 9
//     //   };
//     //   string? updateUrl = $"{UserRatingsApi}/{newRating.UserId}/{rating?["id"]?.ToString()}";
//     //   var statusCode = await PutData(updateUrl, updatedRating);

//     //   Assert.Equal(HttpStatusCode.NoContent, statusCode);

//     //   // Clean up after test
//     //   await DeleteData(updateUrl);
//     // }

//     [Fact]
//     public async Task ApiUserRatings_UpdateUserRating_InvalidData_BadRequest()
//     {
//       var newRating = new
//       {
//         UserId = 1,
//         TConst = "tt16120138",
//         Rating = 8,
//       };
//       var (rating, _) = await PostData($"{UserRatingsApi}", newRating);

//       var invalidRating = new
//       {
//         UserId = 1,
//         TConst = "tt16120138",
//         Rating = 11,
//       };

//       string? updateUrl = $"{UserRatingsApi}/{newRating.UserId}/{rating?["id"]?.ToString()}";
//       var (_, statusCode) = await PutData(updateUrl, invalidRating);

//       Assert.Equal(HttpStatusCode.BadRequest, statusCode);

//       // Clean up after test
//       await DeleteData(updateUrl);
//     }

//     // Helpers

//     private async Task<T?> DeserializeData<T>(HttpResponseMessage response) where T : class
//     {
//       var data = await response.Content.ReadAsStringAsync();
//       return JsonSerializer.Deserialize<T>(data);
//     }

//     private StringContent CreateJsonContent(object content) =>
//         new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
//     private async Task<(JsonObject?, HttpStatusCode)> GetObject(string url)
//     {
//       var response = await client.GetAsync(url);
//       return (await DeserializeData<JsonObject>(response), response.StatusCode);
//     }

//     private async Task<(JsonObject?, HttpStatusCode)> PostData(string url, object content)
//     {
//       var response = await client.PostAsync(url, CreateJsonContent(content));
//       return (await DeserializeData<JsonObject>(response), response.StatusCode);
//     }

//     private async Task<HttpStatusCode> DeleteData(string url)
//     {
//       var response = await client.DeleteAsync(url);
//       return response.StatusCode;
//     }

//     private async Task<(JsonObject?, HttpStatusCode)> PutData(string url, object content)
//     {
//       var response = await client.PutAsync(url, CreateJsonContent(content));
//       return (await DeserializeData<JsonObject>(response), response.StatusCode);
//     }
//   }

// static class HelperExt
// {
//   public static string? Value(this JsonNode node, string name) =>
//       node[name]?.ToString();

//   public static int? ValueInt(this JsonNode node, string name) =>
//       int.TryParse(node[name]?.ToString(), out var result) ? result : null;
// }
// }