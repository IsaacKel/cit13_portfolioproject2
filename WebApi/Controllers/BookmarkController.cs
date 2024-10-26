using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookmarkController : ControllerBase
  {
    private readonly IDataService _dataService;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    public BookmarkController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // Add Bookmark
    [HttpPost]
    public IActionResult AddBookmark(BookmarkDto dto)
    {
      var bookmark = _dataService.AddBookmark(dto.UserId, dto.TConst, dto.NConst, dto.Note);
      var bookmarkDto = new BookmarkDto
      {
        UserId = bookmark.UserId,
        TConst = bookmark.TConst,
        NConst = bookmark.NConst,
        Note = bookmark.Note
      };
      return CreatedAtAction(nameof(GetBookmark), new { userId = bookmark.UserId, bookmarkId = bookmark.Id }, bookmarkDto);
    }

    // Get all Bookmarks for a User with pagination
    [HttpGet("user/{userId}")]
    public IActionResult GetBookmarks(int userId, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
      // Apply page size constraints
      pageSize = pageSize > MaxPageSize ? DefaultPageSize : pageSize;

      // Retrieve paginated bookmarks from the data service
      var bookmarks = _dataService.GetBookmarks(userId, pageNumber, pageSize);

      if (bookmarks == null || !bookmarks.Any())
      {
        return NotFound("No bookmarks found for this user.");
      }

      // Convert bookmarks to DTOs and add self-referencing URIs
      var bookmarkDtos = bookmarks.Select(b => new BookmarkDto
      {
        UserId = b.UserId,
        TConst = b.TConst,
        NConst = b.NConst,
        Note = b.Note,
        SelfLink = Url.Action(nameof(GetBookmark), new { userId = b.UserId, bookmarkId = b.Id })
      }).ToList();

      // Create links for pagination
      var totalCount = _dataService.GetBookmarkCount(userId); // Assume this method exists to get total count
      var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

      var response = new PaginatedResponse<BookmarkDto>
      {
        Data = bookmarkDtos,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPages = totalPages,
        TotalCount = totalCount,
        NextPage = pageNumber < totalPages ? Url.Action(nameof(GetBookmarks), new { userId, pageNumber = pageNumber + 1, pageSize }) : null,
        PreviousPage = pageNumber > 1 ? Url.Action(nameof(GetBookmarks), new { userId, pageNumber = pageNumber - 1, pageSize }) : null
      };

      return Ok(response);
    }

    // Get a specific Bookmark by ID with self-reference
    [HttpGet("{bookmarkId}")]
    public IActionResult GetBookmark(int userId, int bookmarkId)
    {
      var bookmark = _dataService.GetBookmark(userId, bookmarkId);
      if (bookmark == null)
        return NotFound();

      var bookmarkDto = new BookmarkDto
      {
        UserId = bookmark.UserId,
        TConst = bookmark.TConst,
        NConst = bookmark.NConst,
        Note = bookmark.Note,
        SelfLink = Url.Action(nameof(GetBookmark), new { userId = bookmark.UserId, bookmarkId = bookmark.Id })
      };
      return Ok(bookmarkDto);
    }

    // Update Bookmark
    [HttpPut("{bookmarkId}")]
    public IActionResult UpdateBookmark(int userId, int bookmarkId, BookmarkDto dto)
    {
      _dataService.UpdateBookmark(userId, bookmarkId, dto.TConst, dto.NConst, dto.Note);
      return NoContent();
    }

    // Delete Bookmark
    [HttpDelete("{bookmarkId}")]
    public IActionResult DeleteBookmark(int userId, int bookmarkId)
    {
      _dataService.DeleteBookmark(userId, bookmarkId);
      return NoContent();
    }
  }

  // Helper class for paginated response
  public class PaginatedResponse<T>
  {
    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string NextPage { get; set; }
    public string PreviousPage { get; set; }
  }
}