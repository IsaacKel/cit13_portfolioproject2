using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace cit13_portfolioproject2.WebApiTests.UserControllerTests
{
  public class UserControllerTests
  {
    private const string UsersApi = "http://localhost:5002/api/user";
    private static readonly HttpClient client = new HttpClient();

    /* /api/user */

    [Fact]
    public async Task ApiUsers_GetUserWithValidId_OkAndUserDetails()
    {
      int userId = 1;
      var (user, statusCode) = await GetObject($"{UsersApi}/{userId}");

      Assert.Equal(HttpStatusCode.OK, statusCode);
      Assert.NotNull(user);
      Assert.Equal(userId, user?.ValueInt("id"));
    }

    [Fact]
    public async Task ApiUsers_GetUserWithInvalidId_NotFound()
    {
      int userId = 999;
      var (_, statusCode) = await GetObject($"{UsersApi}/{userId}");

      Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Fact]
    public async Task ApiUsers_GetUserByUsernameWithValidUsername_OkAndUserDetails()
    {
      string username = "john_doe";
      var (user, statusCode) = await GetObject($"{UsersApi}/username/{username}");

      Assert.Equal(HttpStatusCode.OK, statusCode);
      Assert.NotNull(user);
      Assert.Equal(username, user?.Value("username"));
    }

    [Fact]
    public async Task ApiUsers_GetUserByUsernameWithInvalidUsername_NotFound()
    {
      string username = "invaliduser";
      var (_, statusCode) = await GetObject($"{UsersApi}/username/{username}");

      Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Fact]
    public async Task ApiUsers_RegisterUser_ValidData_Created()
    {
      var newUser = new
      {
        Username = "newuser",
        Password = "password123",
        Email = "newuser@example.com"
      };
      var (user, statusCode) = await PostData($"{UsersApi}/register", newUser);

      Assert.Equal(HttpStatusCode.Created, statusCode);
      Assert.NotNull(user);
      Assert.Equal(newUser.Username, user?.Value("username"));
      Assert.Equal(newUser.Email, user?.Value("email"));

      // Clean up after test
      string? id = user?["id"]?.ToString();
      if (id != null)
      {
        await DeleteData($"{UsersApi}/{id}");
      }
    }

    [Fact]
    public async Task ApiUsers_RegisterUser_InvalidData_BadRequest()
    {
      var invalidUser = new
      {
        Username = "",
        Password = "123",
        Email = "invalidemail"
      };
      var (_, statusCode) = await PostData($"{UsersApi}/register", invalidUser);

      Assert.Equal(HttpStatusCode.BadRequest, statusCode);
    }

    [Fact]
    public async Task ApiUsers_DeleteUserWithValidId_NoContent()
    {
      var newUser = new
      {
        Username = "todelete",
        Password = "password123",
        Email = "todelete@example.com"
      };
      var (user, _) = await PostData($"{UsersApi}/register", newUser);

      string? deleteUrl = $"{UsersApi}/{user?["id"]?.ToString()}";
      var statusCode = await DeleteData(deleteUrl);

      Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    [Fact]
    public async Task ApiUsers_DeleteUserWithInvalidId_NotFound()
    {
      var statusCode = await DeleteData($"{UsersApi}/999");

      Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    // Helpers

    private async Task<T?> DeserializeData<T>(HttpResponseMessage response) where T : class
    {
      var data = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<T>(data);
    }

    private StringContent CreateJsonContent(object content) =>
        new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

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

    private async Task<HttpStatusCode> DeleteData(string url)
    {
      var response = await client.DeleteAsync(url);
      return response.StatusCode;
    }
  }

  static class HelperExt
  {
    public static string? Value(this JsonNode node, string name) =>
        node[name]?.ToString();

    public static int? ValueInt(this JsonNode node, string name) =>
        int.TryParse(node[name]?.ToString(), out var result) ? result : null;
  }
}