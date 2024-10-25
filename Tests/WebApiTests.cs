// using System.Net;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Xunit;

// namespace Assignment4.Tests;

// public class WebServiceTests
// {
//   private const string BaseUrl = "http://localhost:5002/api";
//   private const string SearchHistoryApi = $"{BaseUrl}/searchhistory";
//   private const string UserRatingApi = $"{BaseUrl}/userrating";

//   /* SearchHistory Tests */
//   [Fact]
//   public async Task ApiSearchHistory_GetWithValidUserId_OkAndSearchHistoryList()
//   {
//     var (data, statusCode) = await GetArray($"{SearchHistoryApi}?userid=1");

//     Assert.Equal(HttpStatusCode.OK, statusCode);
//     Assert.NotEmpty(data);
//     Assert.Equal(1, data?.FirstElement("userid"));
//   }

//   [Fact]
//   public async Task ApiSearchHistory_PostWithValidData_Created()
//   {
//     var newSearchHistory = new
//     {
//       UserId = 1,
//       SearchQuery = "TestQuery"
//     };

//     var (history, statusCode) = await PostData(SearchHistoryApi, newSearchHistory);

//     Assert.Equal(HttpStatusCode.Created, statusCode);
//     Assert.Equal("TestQuery", history?.Value("searchquery"));
//     Assert.Equal(1, history?.Value("userid"));

//     // Cleanup
//     await DeleteData($"{SearchHistoryApi}/{history?.Value("id")}");
//   }

//   /* UserRating Tests */
//   [Fact]
//   public async Task ApiUserRating_GetWithValidUserId_OkAndRatingsList()
//   {
//     var (data, statusCode) = await GetArray($"{UserRatingApi}?userid=1");

//     Assert.Equal(HttpStatusCode.OK, statusCode);
//     Assert.NotEmpty(data);
//     Assert.Equal(1, data?.FirstElement("userid"));
//   }

//   [Fact]
//   public async Task ApiUserRating_PostWithValidData_Created()
//   {
//     var newRating = new
//     {
//       UserId = 1,
//       MovieId = 1,
//       Rating = 8
//     };

//     var (rating, statusCode) = await PostData(UserRatingApi, newRating);

//     Assert.Equal(HttpStatusCode.Created, statusCode);
//     Assert.Equal(8, rating?.Value("rating"));
//     Assert.Equal(1, rating?.Value("userid"));

//     // Cleanup
//     await DeleteData($"{UserRatingApi}/{rating?.Value("id")}");
//   }

//   // Helpers
//   private async Task<(JsonArray?, HttpStatusCode)> GetArray(string url)
//   {
//     var client = new HttpClient();
//     var response = await client.GetAsync(url);
//     var data = await response.Content.ReadAsStringAsync();
//     return (JsonSerializer.Deserialize<JsonArray>(data), response.StatusCode);
//   }

//   private async Task<(JsonObject?, HttpStatusCode)> PostData(string url, object content)
//   {
//     var client = new HttpClient();
//     var requestContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
//     var response = await client.PostAsync(url, requestContent);
//     var data = await response.Content.ReadAsStringAsync();
//     return (JsonSerializer.Deserialize<JsonObject>(data), response.StatusCode);
//   }

//   private async Task<HttpStatusCode> DeleteData(string url)
//   {
//     var client = new HttpClient();
//     var response = await client.DeleteAsync(url);
//     return response.StatusCode;
//   }
// }