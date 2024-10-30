using Microsoft.AspNetCore.Mvc;
using DataLayer;
using WebApi.DTOs;
using Mapster;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> GetUserRatings(int userId, int pageNumber, int pageSize)
    {
      var totalRatings = await _dataService.GetUserRatingCountAsync(userId);
      if (totalRatings == 0) return NotFound();

      var userRatings = await _dataService.GetUserRatingsAsync(userId, pageNumber, pageSize);
      var userRatingDtos = new List<UserRatingDto>();

      foreach (var rating in userRatings)
      {
        var selfLink = await GetUrlAsync(nameof(GetUserRatingById), new { userId, ratingId = rating.Id });
        var userRatingDto = rating.Adapt<UserRatingDto>().WithSelfLink(selfLink);
        userRatingDtos.Add(userRatingDto);
      }

      var paginatedResult = CreatePagingUserAsync(nameof(GetUserRatings), userId, pageNumber, pageSize, totalRatings, userRatingDtos);
      return Ok(paginatedResult);
    }

    // Get a specific rating by userId and ratingId
    [HttpGet("{userId}/{ratingId}", Name = nameof(GetUserRatingById))]
    public async Task<IActionResult> GetUserRatingById(int userId, int ratingId)
    {
      var userRating = await _dataService.GetUserRatingAsync(ratingId);
      if (userRating == null) return NotFound();

      var userRatingDto = userRating.Adapt<UserRatingDto>().WithSelfLink(await GetUrlAsync(nameof(GetUserRatingById), new { userId, ratingId }));
      return Ok(userRatingDto);
    }

    // Add a new rating
    [HttpPost]
    public async Task<IActionResult> AddUserRating([FromBody] UserRatingDto userRatingDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      // var existingRating = _dataService.GetUserRatingByUserAndTConst(userRatingDto.UserId, userRatingDto.TConst);
      // if (existingRating != null)
      // {
      //   return Conflict("User has already rated this movie.");
      // }

      try
      {
        var userRating = await _dataService.AddUserRatingAsync(userRatingDto.UserId, userRatingDto.TConst, userRatingDto.Rating);
        var resultDto = userRating.Adapt<UserRatingDto>().WithSelfLink(await GetUrlAsync(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id }));

        return CreatedAtAction(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id }, resultDto);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
    }

    // Delete a rating
    [HttpDelete("{userId}/{ratingId}")]
    public async Task<IActionResult> DeleteUserRating(int userId, int ratingId)
    {
      if (await _dataService.GetUserRatingAsync(ratingId) == null) return NotFound();

      await _dataService.DeleteUserRatingAsync(ratingId);
      return NoContent();
    }

    // Update a rating by ratingId
    [HttpPut("{userId}/{ratingId}")]
    public async Task<IActionResult> UpdateUserRating(int userId, int ratingId, [FromBody] UserRatingDto userRatingDto)
    {
      var existingRating = await _dataService.GetUserRatingAsync(ratingId);
      if (existingRating == null || existingRating.UserId != userId)
      {
        return Forbid("User does not have permission to modify this rating.");
      }

      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _dataService.UpdateUserRatingAsync(userId, ratingId, userRatingDto.Rating);
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