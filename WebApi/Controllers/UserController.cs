using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using System.Linq;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IDataService _dataService;

    public UserController(IDataService dataService)
    {
      _dataService = dataService;
    }

    // -- GET USER by ID --
    [HttpGet("{userId}")]
    public IActionResult GetUser(int userId)
    {
      var user = _dataService.GetUser(userId);
      if (user == null)
        return NotFound();  // Return 404 if user is not found

      var userDto = new UserDTO
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email
      };

      return Ok(userDto);  // Return 200 with the user DTO
    }

    // -- GET USER by USERNAME --
    [HttpGet("username/{username}")]
    public IActionResult GetUserByUsername(string username)
    {
      var user = _dataService.GetUser(username);
      if (user == null)
        return NotFound();  // Return 404 if user is not found

      var userDto = new UserDTO
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email
      };

      return Ok(userDto);  // Return 200 with the user DTO
    }

    // -- REGISTER USER (Create) --
    [HttpPost("register")]
    public IActionResult RegisterUser(UserRegisterDTO dto)
    {
      // Simple validation to ensure username and password aren't empty
      if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
        return BadRequest("Username, Password, and Email are required.");

      // Add user via data service
      var user = _dataService.AddUser(dto.Username, dto.Password, dto.Email);

      // Return the created user as a DTO
      var userDto = new UserDTO
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email
      };

      return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);  // Return 201 Created with user DTO
    }

    // -- DELETE USER --
    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
      var existingUser = _dataService.GetUser(userId);
      if (existingUser == null)
        return NotFound();  // Return 404 if user does not exist

      _dataService.DeleteUser(userId);  // Perform deletion

      return NoContent();  // Return 204 No Content as there's no response body
    }
  }
}