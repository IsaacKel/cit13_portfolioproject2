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
    private const int DefaultPageSize = 10;

    public SearchHistoryController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }

    // Get search history by userId with pagination and self-referencing URIs
    [HttpGet("{userId}", Name = nameof(GetSearchHistory))]
    public IActionResult GetSearchHistory(int userId, int pageNumber = 0, int pageSize = DefaultPageSize)
    {
      var searchHistories = _dataService.GetSearchHistory(userId);

      if (searchHistories == null || !searchHistories.Any())
      {
        return NotFound("No search history found for this user.");
      }

      pageSize = pageSize > DefaultPageSize ? DefaultPageSize : pageSize;

      var pagedHistories = searchHistories
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .Adapt<List<SearchHistoryDTO>>();

      foreach (var dto in pagedHistories)
      {
        dto.SelfLink = GetUrl(nameof(GetSearchHistoryById), new { userId = dto.UserId, dto.SearchQuery, dto.CreatedAt });
      }

      var response = CreatePaging(
          nameof(GetSearchHistory),
          pageNumber,
          pageSize,
          searchHistories.Count(),
          pagedHistories);

      return Ok(response);
    }

    // Specific endpoint for retrieving a single search history item
    [HttpGet("{userId}/{searchQuery}/{createdAt}", Name = nameof(GetSearchHistoryById))]
    public IActionResult GetSearchHistoryById(int userId, string searchQuery, DateTime createdAt)
    {
      var searchHistory = _dataService.GetSearchHistory(userId, searchQuery, createdAt);

      if (searchHistory == null)
      {
        return NotFound("Search history entry not found.");
      }

      var searchHistoryDto = searchHistory.Adapt<SearchHistoryDTO>();
      searchHistoryDto.SelfLink = GetUrl(nameof(GetSearchHistoryById), new { userId, searchQuery, createdAt });

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
      searchHistoryDtoResult.SelfLink = GetUrl(nameof(GetSearchHistoryById), new { userId = searchHistoryDto.UserId, searchHistoryDto.SearchQuery, createdAt });

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