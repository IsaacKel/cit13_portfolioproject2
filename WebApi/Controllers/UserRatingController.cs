using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
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

    // Get all ratings for a user with pagination and self-links
    [HttpGet("{userId}", Name = nameof(GetUserRatings))]
    public IActionResult GetUserRatings(int userId, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
      var totalRatings = _dataService.GetUserRatingCount(userId);

      if (totalRatings == 0)
      {
        return NotFound();
      }

      var userRatings = _dataService.GetUserRatings(userId, pageNumber, pageSize);
      var userRatingDtos = userRatings.Select(r =>
      {
        var dto = r.Adapt<UserRatingDto>();
        dto.SelfLink = GetUrl(nameof(GetUserRatingById), new { userId = r.UserId, ratingId = r.Id });
        return dto;
      }).ToList();
      var paginatedResult = CreatePagingUser(nameof(GetUserRatings), userId, pageNumber, pageSize, totalRatings, userRatingDtos);
      return Ok(paginatedResult);
    }

    // Get a specific rating by userId and ratingId
    [HttpGet("{userId}/{ratingId}", Name = nameof(GetUserRatingById))]
    public IActionResult GetUserRatingById(int ratingId)
    {
      var userRating = _dataService.GetUserRating(ratingId);
      if (userRating == null)
      {
        return NotFound();
      }

      var userRatingDto = userRating.Adapt<UserRatingDto>();
      userRatingDto.SelfLink = GetUrl(nameof(GetUserRatingById), new { ratingId });

      return Ok(userRatingDto);
    }

    // Add a new rating
    [HttpPost]
    public IActionResult AddUserRating([FromBody] UserRatingDto userRatingDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var userRating = _dataService.AddUserRating(userRatingDto.UserId, userRatingDto.TConst, userRatingDto.Rating);
        var userRatingDtoResult = userRating.Adapt<UserRatingDto>();
        userRatingDtoResult.SelfLink = GetUrl(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id });

        return CreatedAtAction(nameof(GetUserRatingById), new { userId = userRatingDto.UserId, ratingId = userRating.Id }, userRatingDtoResult);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
    }

    // Delete a rating
    [HttpDelete("{userId}/{ratingId}")]
    public IActionResult DeleteUserRating(int ratingId)
    {
      var existingUserRating = _dataService.GetUserRating(ratingId);
      if (existingUserRating == null)
      {
        return NotFound();
      }

      _dataService.DeleteUserRating(ratingId);
      return NoContent();
    }

    // Update a rating by ratingId
    [HttpPut("{userId}/{ratingId}")]
    public IActionResult UpdateUserRating(int userId, int ratingId, [FromBody] UserRatingDto userRatingDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState); // Return validation errors
      }

      var existingRating = _dataService.GetUserRating(ratingId);
      if (existingRating == null)
      {
        return NotFound();
      }

      _dataService.UpdateUserRating(userId, ratingId, userRatingDto.Rating);
      return NoContent(); // 204 status code for successful update
    }
  }
}