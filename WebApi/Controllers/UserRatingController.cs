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
  [Authorize] // Basic authorization
  public class UserRatingController : ControllerBase
  {
    private readonly IDataService _dataService;

    public UserRatingController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // Get all ratings for a user
    [HttpGet("{userId}")]
    public IActionResult GetUserRatings(int userId)
    {
      var userRatings = _dataService.GetUserRatings(userId);
      if (userRatings == null || !userRatings.Any())
      {
        return NotFound("No ratings found for this user.");
      }

      var userRatingDtos = userRatings.Adapt<List<UserRatingDto>>();
      return Ok(userRatingDtos);
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
    public IActionResult GetUserRating(int userId, int tconst)
    {
      var userRating = _dataService.GetUserRating(userId, tconst);
      if (userRating == null)
      {
        return NotFound("Rating not found.");
      }

      var userRatingDto = userRating.Adapt<UserRatingDto>();
      return Ok(userRatingDto);
    }

    // Delete a rating by composite key (userId and tconst)
    [HttpDelete("{userId}/{tconst}")]
    public IActionResult DeleteUserRating(int userId, int tconst)
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