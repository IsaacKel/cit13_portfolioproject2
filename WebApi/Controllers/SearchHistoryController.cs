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
    private const int DefaultPageSize = 10; // Default page size for pagination

    public SearchHistoryController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // Get search history by userId with pagination and self-referencing URIs
    [HttpGet("{userId}")]
    public IActionResult GetSearchHistory(int userId, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
      // Retrieve the search history for the user
      var searchHistories = _dataService.GetSearchHistory(userId);

      if (searchHistories == null || !searchHistories.Any())
      {
        return NotFound("No search history found for this user.");
      }

      // Calculate pagination details
      pageSize = pageSize > DefaultPageSize ? DefaultPageSize : pageSize; // Limit page size
      var pagedHistories = searchHistories
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      // Map to DTO and add self-referencing URIs
      var searchHistoryDtos = pagedHistories.Adapt<List<SearchHistoryDTO>>();
      foreach (var dto in searchHistoryDtos)
      {
        dto.SelfLink = Url.Action(nameof(GetSearchHistoryById), new { userId = dto.UserId, searchQuery = dto.SearchQuery, createdAt = dto.CreatedAt });
      }

      // Generate links for pagination
      var totalRecords = searchHistories.Count();
      var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

      var response = new
      {
        TotalRecords = totalRecords,
        PageSize = pageSize,
        PageNumber = pageNumber,
        TotalPages = totalPages,
        Data = searchHistoryDtos,
        PreviousPage = pageNumber > 1 ? Url.Action(nameof(GetSearchHistory), new { userId, pageNumber = pageNumber - 1, pageSize }) : null,
        NextPage = pageNumber < totalPages ? Url.Action(nameof(GetSearchHistory), new { userId, pageNumber = pageNumber + 1, pageSize }) : null
      };

      return Ok(response);
    }

    // Specific endpoint for retrieving a single search history item
    [HttpGet("{userId}/{searchQuery}/{createdAt}")]
    public IActionResult GetSearchHistoryById(int userId, string searchQuery, DateTime createdAt)
    {
      var searchHistory = _dataService.GetSearchHistory(userId, searchQuery, createdAt);

      if (searchHistory == null)
      {
        return NotFound("Search history entry not found.");
      }

      var searchHistoryDto = searchHistory.Adapt<SearchHistoryDTO>();
      searchHistoryDto.SelfLink = Url.Action(nameof(GetSearchHistoryById), new { userId, searchQuery, createdAt });

      return Ok(searchHistoryDto);
    }

    // Add search history
    [HttpPost]
    public IActionResult AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto, DateTime createdAt)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var searchHistory = _dataService.AddSearchHistory(searchHistoryDto.UserId, searchHistoryDto.SearchQuery, createdAt);
      var searchHistoryDtoResult = searchHistory.Adapt<SearchHistoryDTO>();
      searchHistoryDtoResult.SelfLink = Url.Action(nameof(GetSearchHistoryById), new { userId = searchHistoryDto.UserId, searchHistoryDto.SearchQuery, createdAt });

      return CreatedAtAction(nameof(GetSearchHistoryById), new { userId = searchHistoryDto.UserId, searchHistoryDto.SearchQuery, createdAt }, searchHistoryDtoResult);
    }

    // Delete search history
    [HttpDelete("{userId}/{searchQuery}/{createdAt}")]
    public IActionResult DeleteSearchHistory(int userId, string searchQuery, DateTime createdAt)
    {
      var existingSearchHistory = _dataService.GetSearchHistory(userId, searchQuery, createdAt);
      if (existingSearchHistory == null)
      {
        return NotFound("Search history entry not found.");
      }

      _dataService.DeleteSearchHistory(userId, searchQuery, createdAt);
      return NoContent();
    }
  }
}