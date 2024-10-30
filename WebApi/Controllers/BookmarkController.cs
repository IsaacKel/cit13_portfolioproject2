using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> AddBookmarkAsync(BookmarkDto dto)
    {
      // Check if a bookmark with the same TConst or NConst already exists for this user
      var existingBookmark = (await _dataService.GetBookmarksAsync(dto.UserId))
                                         .FirstOrDefault(b => b.TConst == dto.TConst && b.NConst == dto.NConst);

      // if (existingBookmark != null)
      // {
      //   return Conflict(new { message = "Bookmark already exists for this title and user." });
      // }

      var bookmark = await _dataService.AddBookmarkAsync(dto.UserId, dto.TConst, dto.NConst, dto.Note);
      var bookmarkDto = MapToBookmarkDto(bookmark);
      return CreatedAtAction(nameof(GetBookmarkAsync), new { userId = bookmark.UserId, bookmarkId = bookmark.Id }, bookmarkDto);
    }

    // Get all Bookmarks for a User with Pagination
    [HttpGet("user/{userId}", Name = nameof(GetBookmarksAsync))]
    public async Task<IActionResult> GetBookmarksAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var bookmarks = await _dataService.GetBookmarksAsync(userId, pageNumber, pageSize);
      if (bookmarks == null || !bookmarks.Any())
        return NotFound();

      var totalItems = await _dataService.GetBookmarkCountByUserAsync(userId);
      var bookmarkDtos = bookmarks.Select(MapToBookmarkDto).ToList();
      var paginatedResult = CreatePagingUserAsync(nameof(GetBookmarksAsync), userId, pageNumber, pageSize, totalItems, bookmarkDtos);

      return Ok(paginatedResult);
    }

    // Get a specific Bookmark by ID with self-reference
    [HttpGet("{bookmarkId}", Name = nameof(GetBookmarkAsync))]
    public async Task<IActionResult> GetBookmarkAsync(int userId, int bookmarkId)
    {
      var bookmark = await _dataService.GetBookmarkAsync(userId, bookmarkId);
      return bookmark == null ? NotFound() : Ok(MapToBookmarkDto(bookmark));
    }

    // Update Bookmark with Existence Check
    [HttpPut("{bookmarkId}")]
    public async Task<IActionResult> UpdateBookmarkAsync(int userId, int bookmarkId, BookmarkDto dto)
    {
      // Check if bookmark exists
      if (await _dataService.GetBookmarkAsync(userId, bookmarkId) == null)
      {
        return NotFound();
      }

      await _dataService.UpdateBookmarkAsync(userId, bookmarkId, dto.TConst, dto.NConst, dto.Note);
      return NoContent();
    }

    // Delete Bookmark with Existence Check
    [HttpDelete("{bookmarkId}")]
    public async Task<IActionResult> DeleteBookmarkAsync(int bookmarkId)
    {
      var existingBookmark = await _dataService.GetBookmarkByIdAsync(bookmarkId);
      if (existingBookmark == null)
      {
        return NotFound();
      }

      await _dataService.DeleteBookmarkAsync(bookmarkId);
      return NoContent();
    }

    // Private Helper Method to Map Bookmark to BookmarkDto
    private async Task<BookmarkDto> MapToBookmarkDto(Bookmark bookmark)
    {
      return new BookmarkDto
      {
        UserId = bookmark.UserId,
        TConst = bookmark.TConst,
        NConst = bookmark.NConst,
        Note = bookmark.Note,
        SelfLink = await GetUrlAsync(nameof(GetBookmarkAsync), new { userId = bookmark.UserId, bookmarkId = bookmark.Id })
      };
    }
  }
}