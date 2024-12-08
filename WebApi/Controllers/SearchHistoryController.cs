using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DataLayer.Models;
using DataLayer;
using WebApi.DTOs;
using Mapster;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [EnableCors("AllowReactApp")]
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
    [HttpGet("user", Name = nameof(GetSearchHistoryByUser))]
    [Authorize]
    public IActionResult GetSearchHistoryByUser(int pageNumber = 1, int pageSize = 10)
    {
      var userId = int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out var userIdInt);


      if (userIdInt == null) return Unauthorized();

      var searchHistories = _dataService.GetSearchHistoriesByUser(userIdInt, pageNumber, pageSize);
      if (searchHistories == null || !searchHistories.Any()) return NotFound(new { message = "Search history not found." });

      var totalItems = _dataService.GetSearchHistoryCountByUser(userIdInt);
      var searchHistoryDtos = searchHistories.Select(sh => sh.Adapt<SearchHistoryDTO>()).ToList();
      searchHistoryDtos.ForEach(dto => dto.SelfLink = GetUrl(nameof(GetSearchHistory), new { searchId = dto.Id }));

      return Ok(CreatePagingUser(nameof(GetSearchHistoryByUser), userIdInt, pageNumber, pageSize, totalItems, searchHistoryDtos));
    }

    // Add a new search history entry
    [HttpPost]
    public IActionResult AddSearchHistory([FromBody] SearchHistoryDTO searchHistoryDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (!_dataService.UserExists(searchHistoryDto.UserId))
      {
        return NotFound(new { message = "User does not exist." });
      }

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