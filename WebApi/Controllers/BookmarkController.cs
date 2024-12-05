using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookmarkController : BaseController
  {
    private readonly IDataService _dataService;

    public BookmarkController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }

    // Add Bookmark
    [HttpPost]
    public IActionResult AddBookmark(BookmarkDto dto)
    {
      // Check if a bookmark with the same TConst or NConst already exists for this user
      var existingBookmark = _dataService.GetBookmarks(dto.UserId)
                                         .FirstOrDefault(b => b.TConst == dto.TConst && b.NConst == dto.NConst);

      if (existingBookmark != null)
      {
        return Conflict(new { message = "Bookmark already exists for this title and user." });
      }

      if (!_dataService.UserExists(dto.UserId))
      {
        return NotFound(new { message = "User does not exist." });
      }

      var bookmark = _dataService.AddBookmark(dto.UserId, dto.TConst, dto.NConst, dto.Note);
      var bookmarkDto = MapToBookmarkDto(bookmark);
      return CreatedAtAction(nameof(GetBookmark), new { userId = bookmark.UserId, bookmarkId = bookmark.Id }, bookmarkDto);
    }

    // Get all Bookmarks for a User with Pagination
    [HttpGet("user/{userId}", Name = nameof(GetBookmarks))]
    public IActionResult GetBookmarks(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var bookmarks = _dataService.GetBookmarks(userId, pageNumber, pageSize);
      if (bookmarks == null || !bookmarks.Any())
        return NotFound();

      var totalItems = _dataService.GetBookmarkCountByUser(userId);
      var bookmarkDtos = bookmarks.Select(MapToBookmarkDto).ToList();
      var paginatedResult = CreatePagingUser(nameof(GetBookmarks), userId, pageNumber, pageSize, totalItems, bookmarkDtos);

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