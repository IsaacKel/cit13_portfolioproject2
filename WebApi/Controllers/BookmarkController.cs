using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.DTOs;
using DataLayer;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [EnableCors("AllowReactApp")]
  public class BookmarkController : BaseController
  {
    private readonly IDataService _dataService;

    public BookmarkController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }

    [HttpPost]
    public IActionResult AddBookmark(BookmarkDto dto)
    {
      // Log the incoming request details
      Console.WriteLine("Received bookmark request:");
      Console.WriteLine($"UserId: {dto.UserId}, TConst: {dto.TConst}, Note: {dto.Note}");

      // Validate the user ID
      if (!_dataService.UserExists(dto.UserId))
      {
        Console.WriteLine($"User with ID {dto.UserId} does not exist.");
        return NotFound(new { message = "User does not exist." });
      }

      // Check for existing bookmark
      var existingBookmark = _dataService.GetBookmarks(dto.UserId)
          .FirstOrDefault(b => b.TConst == dto.TConst && b.NConst == dto.NConst);

      if (existingBookmark != null)
      {
        Console.WriteLine($"Bookmark already exists for UserId: {dto.UserId}, TConst: {dto.TConst}");
        return Conflict(new { message = "Bookmark already exists for this title and user." });
      }

      // Add the bookmark
      var bookmark = _dataService.AddBookmark(dto.UserId, dto.TConst, dto.NConst, dto.Note);
      Console.WriteLine($"Bookmark added successfully: {bookmark.Id}");

      return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmark);
    }

    // Get all Bookmarks for a User with Pagination
    [HttpGet("user", Name = nameof(GetBookmarks))]
    [Authorize]
    public IActionResult GetBookmarks(int pageNumber = 1, int pageSize = 10)
    {
      var userId = int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out var userIdInt);

      if (userIdInt == null) return Unauthorized();

      var bookmarks = _dataService.GetBookmarks(userIdInt, pageNumber, pageSize);
      if (bookmarks == null || !bookmarks.Any())
        return NotFound();

      var totalItems = _dataService.GetBookmarkCountByUser(userIdInt);
      var bookmarkDtos = bookmarks.Select(MapToBookmarkDto).ToList();
      var paginatedResult = CreatePagingUser(nameof(GetBookmarks), userIdInt, pageNumber, pageSize, totalItems, bookmarkDtos);

      return Ok(paginatedResult);
    }

    // Get a specific Bookmark by ID with self-reference
    [HttpGet("{bookmarkId}", Name = nameof(GetBookmark))]
    public IActionResult GetBookmark(int userId, int bookmarkId)
    {
      var bookmark = _dataService.GetBookmark(userId, bookmarkId);
      return bookmark == null ? NotFound() : Ok(MapToBookmarkDto(bookmark));
    }

    // Update Bookmark with Existence Check
    [HttpPut("{bookmarkId}")]
    public IActionResult UpdateBookmark(int userId, int bookmarkId, BookmarkDto dto)
    {
      // Check if bookmark exists
      if (_dataService.GetBookmark(userId, bookmarkId) == null)
      {
        return NotFound();
      }

      _dataService.UpdateBookmark(userId, bookmarkId, dto.TConst, dto.NConst, dto.Note);
      return NoContent();
    }

    // Delete Bookmark with Existence Check
    [HttpDelete("{bookmarkId}")]
    public IActionResult DeleteBookmark(int bookmarkId)
    {
      var existingBookmark = _dataService.GetBookmarkById(bookmarkId);
      if (existingBookmark == null)
      {
        return NotFound();
      }

      _dataService.DeleteBookmark(bookmarkId);
      return NoContent();
    }

    // Private Helper Method to Map Bookmark to BookmarkDto
    private BookmarkDto MapToBookmarkDto(Bookmark bookmark)
    {
      return new BookmarkDto
      {
        Id = bookmark.Id,
        UserId = bookmark.UserId,
        TConst = bookmark.TConst,
        NConst = bookmark.NConst,
        Note = bookmark.Note,
        CreatedAt = bookmark.CreatedAt,
        SelfLink = GetUrl(nameof(GetBookmark), new { userId = bookmark.UserId, bookmarkId = bookmark.Id })
      };
    }
  }
}