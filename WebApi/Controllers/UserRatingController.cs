using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
using DataLayer;
using WebApi.DTOs;
using Mapster;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserRatingController : ControllerBase
  {
    private readonly IDataService _dataService;
    private const int DefaultPageSize = 10;

    public UserRatingController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // Get all ratings for a user with pagination and self-links
    [HttpGet("{userId}")]
    public IActionResult GetUserRatings(int userId, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
      var totalRatings = _dataService.GetUserRatingCount(userId);
      if (totalRatings == 0)
      {
        return NotFound("No ratings found for this user.");
      }

      // Ensure pageSize is within acceptable limits
      pageSize = Math.Min(pageSize, DefaultPageSize);
      var userRatings = _dataService.GetUserRatings(userId, pageNumber, pageSize);

      var userRatingDtos = userRatings.Select(r => new UserRatingDto
      {
        UserId = r.UserId,
        TConst = r.TConst,
        Rating = r.Rating,
        SelfLink = Url.Action(nameof(GetUserRating), new { userId = r.UserId, tconst = r.TConst })
      }).ToList();

      var totalPages = (int)Math.Ceiling(totalRatings / (double)pageSize);
      var paginationMetadata = new
      {
        totalCount = totalRatings,
        pageSize,
        currentPage = pageNumber,
        totalPages,
        previousPageLink = pageNumber > 1 ? Url.Action(nameof(GetUserRatings), new { userId, pageNumber = pageNumber - 1, pageSize }) : null,
        nextPageLink = pageNumber < totalPages ? Url.Action(nameof(GetUserRatings), new { userId, pageNumber = pageNumber + 1, pageSize }) : null
      };

      return Ok(new { ratings = userRatingDtos, pagination = paginationMetadata });
    }

    // Add a new rating
    [HttpPost]
    public IActionResult AddUserRating([FromBody] UserRatingDto userRatingDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState); // Return validation errors
      }

      var userRating = _dataService.AddUserRating(userRatingDto.UserId, userRatingDto.TConst, userRatingDto.Rating);
      var userRatingDtoResult = userRating.Adapt<UserRatingDto>();

      return CreatedAtAction(nameof(GetUserRating), new { userId = userRatingDto.UserId, tconst = userRatingDto.TConst }, userRatingDtoResult);
    }

    // Get a specific rating by composite key (userId and tconst)
    [HttpGet("{userId}/{tconst}")]
    public IActionResult GetUserRating(int userId, string tconst)
    {
      var userRating = _dataService.GetUserRating(userId, tconst);
      if (userRating == null)
      {
        return NotFound("Rating not found.");
      }

      var userRatingDto = userRating.Adapt<UserRatingDto>();
      userRatingDto.SelfLink = Url.Action(nameof(GetUserRating), new { userId, tconst });
      return Ok(userRatingDto);
    }

    // Delete a rating by composite key (userId and tconst)
    [HttpDelete("{userId}/{tconst}")]
    public IActionResult DeleteUserRating(int userId, string tconst)
    {
      var existingRating = _dataService.GetUserRating(userId, tconst);
      if (existingRating == null)
      {
        return NotFound("Rating not found.");
      }

      _dataService.DeleteUserRating(userId, tconst);
      return NoContent(); // 204 status code for successful deletion
    }
  }
}