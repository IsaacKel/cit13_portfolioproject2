using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace cit13_portfolioproject2.WebApiTests
{
  public class SearchHistoryControllerTests
  {
    private const string SearchHistoryApi = "http://localhost:5002/api/searchhistory";
    private static readonly HttpClient client = new HttpClient();

    /* /api/searchhistory */

    [Fact]
    public async Task ApiSearchHistory_GetWithValidSearchId_OkAndSearchHistory()
    {
      int searchId = 1;
      var (searchHistory, statusCode) = await GetObject($"{SearchHistoryApi}/{searchId}");

      Assert.Equal(HttpStatusCode.OK, statusCode);
      Assert.Equal(searchId, searchHistory?.ValueInt("id"));
    }

    [Fact]
    public async Task ApiSearchHistory_GetWithInvalidSearchId_NotFound()
    {
      int searchId = 999;
      var (_, statusCode) = await GetObject($"{SearchHistoryApi}/{searchId}");

      Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Fact]
    public async Task ApiSearchHistory_GetWithValidUserId_OkAndPaginatedSearchHistories()
    {
      int userId = 1;
      int pageNumber = 1;
      int pageSize = 10;

      var (data, statusCode) = await TestHelpers.GetArray($"{SearchHistoryApi}/user/{userId}?pageNumber={pageNumber}&pageSize={pageSize}");

      Assert.Equal(HttpStatusCode.OK, statusCode);
      Assert.NotNull(data);
      Assert.True(data?.Count > 0);
    }

    [Fact]
    public async Task ApiSearchHistory_GetWithInvalidUserId_NotFound()
    {
      int userId = 999;
      var (_, statusCode) = await GetArray($"{SearchHistoryApi}/user/{userId}");

      Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Fact]
    public async Task ApiSearchHistory_PostWithValidData_Created()
    {
      var newSearchHistory = new
      {
        UserId = 1,
        SearchQuery = "sample query"
      };

      var (searchHistory, statusCode) = await PostData(SearchHistoryApi, newSearchHistory);

      Assert.Equal(HttpStatusCode.Created, statusCode);
      Assert.NotNull(searchHistory);
      Assert.Equal(newSearchHistory.UserId, searchHistory?.ValueInt("userId"));
      Assert.Equal(newSearchHistory.SearchQuery, searchHistory?.Value("searchQuery"));

      // Clean up after test
      string? id = searchHistory?["id"]?.ToString();
      if (id != null)
      {
        await DeleteData($"{SearchHistoryApi}/{id}");
      }
    }

    [Fact]
    public async Task ApiSearchHistory_PostWithInvalidData_BadRequest()
    {
      var invalidSearchHistory = new
      {
        UserId = 1,
        SearchQuery = ""
      };

      var (response, statusCode) = await PostData(SearchHistoryApi, invalidSearchHistory);

      Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }

    [Fact]
    public async Task ApiSearchHistory_DeleteWithValidSearchId_NoContent()
    {
      var newSearchHistory = new
      {
        UserId = 1,
        SearchQuery = "sample query"
      };

      var (searchHistory, _) = await PostData(SearchHistoryApi, newSearchHistory);

      // Extract selfLink from the searchHistory for deletion
      string? deleteUrl = searchHistory?["selfLink"]?.ToString();

      var statusCode = await DeleteData(deleteUrl);

      Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    // Helpers

    private async Task<T?> DeserializeData<T>(HttpResponseMessage response) where T : class
    {
      var data = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<T>(data);
    }

    private StringContent CreateJsonContent(object content) =>
        new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

    private async Task<(JsonArray?, HttpStatusCode)> GetArray(string url)
    {
      var response = await client.GetAsync(url);
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        return (new JsonArray(), HttpStatusCode.NotFound);
      }

      var jsonArray = await DeserializeData<JsonArray>(response);
      if (jsonArray != null) return (jsonArray, response.StatusCode);

      var jsonObject = await DeserializeData<JsonObject>(response);
      return (jsonObject != null ? new JsonArray { jsonObject } : null, response.StatusCode);
    }

    private async Task<(JsonObject?, HttpStatusCode)> GetObject(string url)
    {
      var response = await client.GetAsync(url);
      return (await DeserializeData<JsonObject>(response), response.StatusCode);
    }

    private async Task<(JsonObject?, HttpStatusCode)> PostData(string url, object content)
    {
      var response = await client.PostAsync(url, CreateJsonContent(content));
      return (await DeserializeData<JsonObject>(response), response.StatusCode);
    }

    private async Task<HttpStatusCode> PutData(string url, object content)
    {
      var response = await client.PutAsync(url, CreateJsonContent(content));
      return response.StatusCode;
    }

    private async Task<HttpStatusCode> DeleteData(string url)
    {
      var response = await client.DeleteAsync(url);
      return response.StatusCode;
    }
  }
}