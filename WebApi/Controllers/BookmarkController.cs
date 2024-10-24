using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookmarkController : ControllerBase
  {
    private readonly IDataService _dataService;

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
      return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmarkDto);
    }

    // Get all Bookmarks for a User
    [HttpGet("user/{userId}")]
    public IActionResult GetBookmarks(int userId)
    {
      var bookmarks = _dataService.GetBookmarks(userId);
      return Ok(bookmarks);
    }

    // Get a Bookmark by ID
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
        Note = bookmark.Note
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
}