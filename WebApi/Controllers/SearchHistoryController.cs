using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
using DataLayer;
using WebApi.DTOs;
using Mapster;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> GetSearchHistory(int searchId)
    {
      var searchHistory = await _dataService.GetSearchHistoryAsync(searchId);
      if (searchHistory == null) return NotFound();

      var dto = searchHistory.Adapt<SearchHistoryDTO>();
      dto.SelfLink = await GetUrlAsync(nameof(GetSearchHistory), new { searchId });

      return Ok(dto);
    }

    // Get paginated search history entries for a user
    [HttpGet("user/{userId}", Name = nameof(GetSearchHistoryByUser))]
    public async Task<IActionResult> GetSearchHistoryByUser(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var searchHistories = await _dataService.GetSearchHistoriesByUserAsync(userId, pageNumber, pageSize);
      if (searchHistories == null || !searchHistories.Any()) return NotFound(new { message = "Search history not found." });

      var totalItems = await _dataService.GetSearchHistoryCountByUserAsync(userId);
      var searchHistoryDtos = searchHistories.Select(sh => sh.Adapt<SearchHistoryDTO>()).ToList();
      foreach (var dto in searchHistoryDtos)
      {
        dto.SelfLink = await GetUrlAsync(nameof(GetSearchHistory), new { searchId = dto.Id });
      }

      var pagingResult = await CreatePagingUserAsync(nameof(GetSearchHistoryByUser), userId, pageNumber, pageSize, totalItems, searchHistoryDtos);
      return Ok(pagingResult);
    }

    // Add a new search history entry
    [HttpPost]
    public async Task<IActionResult> AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var searchHistory = await _dataService.AddSearchHistoryAsync(searchHistoryDto.UserId, searchHistoryDto.SearchQuery);
      var dto = searchHistory.Adapt<SearchHistoryDTO>();
      dto.SelfLink = await GetUrlAsync(nameof(GetSearchHistory), new { searchId = searchHistory.Id });

      return CreatedAtAction(nameof(GetSearchHistory), new { searchId = searchHistory.Id }, dto);
    }

    // Delete a search history entry by searchId
    [HttpDelete("{searchId}")]
    public async Task<IActionResult> DeleteSearchHistory(int searchId)
    {
      if (await _dataService.GetSearchHistoryAsync(searchId) == null) return NotFound("Search history not found.");

      await _dataService.DeleteSearchHistoryAsync(searchId);
      return NoContent();
    }
  }
}