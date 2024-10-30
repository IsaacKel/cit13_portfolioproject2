using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
using DataLayer;
using WebApi.DTOs;
using Mapster;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class SearchHistoryController : BaseController
  {
    private readonly IDataService _dataService;

    public SearchHistoryController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }

    // Get search history by searchId
    [HttpGet("{searchId}", Name = nameof(GetSearchHistory))]
    public IActionResult GetSearchHistory(int searchId)
    {
      var searchHistory = _dataService.GetSearchHistory(searchId);
      if (searchHistory == null)
      {
        return NotFound();
      }

      var searchHistoryDto = searchHistory.Adapt<SearchHistoryDTO>();
      searchHistoryDto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId });

      return Ok(searchHistoryDto);
    }

    // Get paginated search history entries for a user
    [HttpGet("user/{userId}", Name = nameof(GetSearchHistoryByUser))]
    public IActionResult GetSearchHistoryByUser(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var searchHistories = _dataService.GetSearchHistoriesByUser(userId, pageNumber, pageSize);
      if (searchHistories == null || !searchHistories.Any())
      {
        return NotFound(new { message = "Search history not found." });
      }

      var totalItems = _dataService.GetSearchHistoryCountByUser(userId);
      var searchHistoryDtos = searchHistories.Select(sh =>
      {
        var dto = sh.Adapt<SearchHistoryDTO>();
        dto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId = sh.Id });
        return dto;
      }).ToList();

      var paginatedResult = CreatePaging(nameof(GetSearchHistoryByUser), userId, pageNumber, pageSize, totalItems, searchHistoryDtos);
      return Ok(paginatedResult);
    }

    // Add a new search history entry
    [HttpPost]
    public IActionResult AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState); // Return validation errors
      }

      var searchHistory = _dataService.AddSearchHistory(searchHistoryDto.UserId, searchHistoryDto.SearchQuery);
      var searchHistoryDtoResult = searchHistory.Adapt<SearchHistoryDTO>();
      searchHistoryDtoResult.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId = searchHistory.Id });

      return CreatedAtAction(nameof(GetSearchHistory), new { searchId = searchHistory.Id }, searchHistoryDtoResult);
    }

    // Delete a search history entry by searchId
    [HttpDelete("{searchId}")]
    public IActionResult DeleteSearchHistory(int searchId)
    {
      var existingSearchHistory = _dataService.GetSearchHistory(searchId);
      if (existingSearchHistory == null)
      {
        return NotFound("Search history not found.");
      }

      _dataService.DeleteSearchHistory(searchId);
      return NoContent();
    }
  }
}