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
      if (searchHistory == null) return NotFound();

      var dto = searchHistory.Adapt<SearchHistoryDTO>();
      dto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId });

      return Ok(dto);
    }

    // Get paginated search history entries for a user
    [HttpGet("user/{userId}", Name = nameof(GetSearchHistoryByUser))]
    public IActionResult GetSearchHistoryByUser(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var searchHistories = _dataService.GetSearchHistoriesByUser(userId, pageNumber, pageSize);
      if (searchHistories == null || !searchHistories.Any()) return NotFound(new { message = "Search history not found." });

      var totalItems = _dataService.GetSearchHistoryCountByUser(userId);
      var searchHistoryDtos = searchHistories.Select(sh => sh.Adapt<SearchHistoryDTO>()).ToList();
      searchHistoryDtos.ForEach(dto => dto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId = dto.Id }));

      return Ok(CreatePagingUser(nameof(GetSearchHistoryByUser), userId, pageNumber, pageSize, totalItems, searchHistoryDtos));
    }

    // Add a new search history entry
    [HttpPost]
    public IActionResult AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var searchHistory = _dataService.AddSearchHistory(searchHistoryDto.UserId, searchHistoryDto.SearchQuery);
      var dto = searchHistory.Adapt<SearchHistoryDTO>();
      dto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId = searchHistory.Id });

      return CreatedAtAction(nameof(GetSearchHistory), new { searchId = searchHistory.Id }, dto);
    }

    // Delete a search history entry by searchId
    [HttpDelete("{searchId}")]
    public IActionResult DeleteSearchHistory(int searchId)
    {
      if (_dataService.GetSearchHistory(searchId) == null) return NotFound("Search history not found.");

      _dataService.DeleteSearchHistory(searchId);
      return NoContent();
    }
  }
}