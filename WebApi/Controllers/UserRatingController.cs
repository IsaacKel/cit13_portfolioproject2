using Microsoft.AspNetCore.Mvc;
using DataLayer;
using WebApi.DTOs;
using Mapster;
using System.Linq;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserRatingController : BaseController
  {
    private readonly IDataService _dataService;

    public UserRatingController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }

    // Get paginated ratings for a user
    [HttpGet("{userId}", Name = nameof(GetUserRatings))]
    public IActionResult GetUserRatings(int userId, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
      var totalRatings = _dataService.GetUserRatingCount(userId);
      if (totalRatings == 0) return NotFound();

      var userRatings = _dataService.GetUserRatings(userId, pageNumber, pageSize);
      var userRatingDtos = userRatings.Select(r => r.Adapt<UserRatingDto>().WithSelfLink(GetUrl(nameof(GetUserRatingById), new { userId, ratingId = r.Id }))).ToList();

      var paginatedResult = CreatePagingUser(nameof(GetUserRatings), userId, pageNumber, pageSize, totalRatings, userRatingDtos);
      return Ok(paginatedResult);
    }

    // Get a specific rating by userId and ratingId
    [HttpGet("{userId}/{ratingId}", Name = nameof(GetUserRatingById))]
    public IActionResult GetUserRatingById(int userId, int ratingId)
    {
      var userRating = _dataService.GetUserRating(ratingId);
      if (userRating == null) return NotFound();

      var userRatingDto = userRating.Adapt<UserRatingDto>().WithSelfLink(GetUrl(nameof(GetUserRatingById), new { userId, ratingId }));
      return Ok(userRatingDto);
    }

    // Add a new rating
    [HttpPost]
    public IActionResult AddUserRating([FromBody] UserRatingDto userRatingDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var existingRating = _dataService.GetUserRatingByUserAndTConst(userRatingDto.UserId, userRatingDto.TConst);
      if (existingRating != null)
      {
        return Conflict("User has already rated this title.");
      }

      try
      {
        var userRating = _dataService.AddUserRating(userRatingDto.UserId, userRatingDto.TConst, userRatingDto.Rating);
        var resultDto = userRating.Adapt<UserRatingDto>().WithSelfLink(GetUrl(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id }));

        return CreatedAtAction(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id }, resultDto);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
    }

    // Delete a rating
    [HttpDelete("{userId}/{ratingId}")]
    public IActionResult DeleteUserRating(int userId, int ratingId)
    {
      if (_dataService.GetUserRating(ratingId) == null) return NotFound();

      _dataService.DeleteUserRating(ratingId);
      return NoContent();
    }

    // Update a rating by ratingId
    [HttpPut("{userId}/{ratingId}")]
    public IActionResult UpdateUserRating(int userId, int ratingId, [FromBody] UserRatingDto userRatingDto)
    {
      var existingRating = _dataService.GetUserRating(ratingId);
      if (existingRating == null || existingRating.UserId != userId)
      {
        return Forbid("User does not have permission to modify this rating.");
      }

      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (_dataService.GetUserRating(ratingId) == null) return NotFound();

      _dataService.UpdateUserRating(userId, ratingId, userRatingDto.Rating);
      return NoContent();
    }
  }
}

public static class DtoExtensions
{
  public static T WithSelfLink<T>(this T dto, string selfLink) where T : class
  {
    if (dto is UserRatingDto ratingDto) ratingDto.SelfLink = selfLink;
    return dto;
  }
}