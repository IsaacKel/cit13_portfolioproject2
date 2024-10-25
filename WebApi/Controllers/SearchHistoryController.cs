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
  public class SearchHistoryController : ControllerBase
  {
    private readonly IDataService _dataService;

    public SearchHistoryController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // Get search history by userId
    [HttpGet("{userId}")]
    public IActionResult GetSearchHistory(int userId)
    {
      var searchHistories = _dataService.GetSearchHistory(userId);
      if (searchHistories == null || !searchHistories.Any())
      {
        return NotFound("No search history found for this user.");
      }

      // Map to DTO
      var searchHistoryDtos = searchHistories.Adapt<List<SearchHistoryDTO>>();
      return Ok(searchHistoryDtos);
    }

    // Add search history
    [HttpPost]
    public IActionResult AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto, DateTime createdAt)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState); // Return validation errors
      }

      var searchHistory = _dataService.AddSearchHistory(searchHistoryDto.UserId, searchHistoryDto.SearchQuery, createdAt);
      var searchHistoryDtoResult = searchHistory.Adapt<SearchHistoryDTO>();

      return CreatedAtAction(nameof(GetSearchHistory), new { userId = searchHistoryDto.UserId }, searchHistoryDtoResult);
    }

    // Delete search history
    [HttpDelete("{userId}/{searchQuery}/{createdAt}")]
    public IActionResult DeleteSearchHistory(int userId, string searchQuery, DateTime createdAt)
    {
      var existingSearchHistory = _dataService.GetSearchHistory(userId, searchQuery, createdAt);
      _dataService.DeleteSearchHistory(userId, searchQuery, createdAt);
      if (existingSearchHistory == null)
      {
        return NotFound("Search history entry not found.");
      }

      _dataService.DeleteSearchHistory(userId, searchQuery, createdAt);
      return NoContent(); // 204 status code for successful deletion
    }
  }
}