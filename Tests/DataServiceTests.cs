using DataLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System.Collections.Generic;
using Xunit;

namespace Assignment4.Tests;

public class DataServiceTests
{
  private IConfiguration configuration;
  private DataService service;

  public DataServiceTests()
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

  /* SearchHistory Tests */
  [Fact]
  public void AddSearchHistory_ValidData_ReturnsSearchHistoryObject()
  {
    var searchHistory = service.AddSearchHistory(1, "TestQuery2", DateTime.UtcNow);

    //Assert.True(searchHistory.Id > 0);
    Assert.Equal("TestQuery2", searchHistory.SearchQuery);
    Assert.Equal(1, searchHistory.UserId);

    // cleanup
    //service.DeleteSearchHistory(searchHistory.UserId);
  }

  [Fact]
  public void GetSearchHistory_ValidUserId_ReturnsSearchHistoryList()
  {
    var history = service.GetSearchHistory(1);
    Assert.NotEmpty(history);
    Assert.Equal(1, history.First().UserId);
  }

  /* UserRating Tests */
  [Fact]
  public void AddUserRating_ValidData_ReturnsUserRatingObject()
  {
    var rating = service.AddUserRating(1, "tt17679810", 7);

    Assert.True(rating.UserId > 0);
    Assert.Equal(7, rating.Rating);
    Assert.Equal(1, rating.UserId);

    // cleanup
    //service.DeleteUserRating(rating.UserId);
  }

  [Fact]
  public void GetUserRatings_ValidUserId_ReturnsUserRatingsList()
  {
    var ratings = service.GetUserRatings(1);
    Assert.NotEmpty(ratings);
    Assert.Equal(1, ratings.First().UserId);
  }
}