using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using Mapster;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : BaseController
  {
    private readonly IDataService _dataService;

    public UserController(IDataService dataService, LinkGenerator linkGenerator)
      : base(linkGenerator)
    {
      _dataService = dataService;
    }


    // -- GET USER by ID --
    [HttpGet("{userId}")]
    public IActionResult GetUser(int userId)
    {
      var user = _dataService.GetUser(userId);
      if (user == null)
        return NotFound("User not found.");

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId });

      return Ok(userDto);
    }

    // -- GET USER by USERNAME --
    [HttpGet("username/{username}")]
    public IActionResult GetUserByUsername(string username)
    {
      var user = _dataService.GetUser(username);
      if (user == null)
        return NotFound("User not found.");

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUserByUsername), new { username });

      return Ok(userDto);
    }

    // -- REGISTER USER (Create) --
    [HttpPost("register")]
    public IActionResult RegisterUser([FromBody] UserRegisterDTO dto)
    {
      if (!ModelState.IsValid)  // Validation via ModelState
        return BadRequest(ModelState);

      var user = _dataService.AddUser(dto.Username, dto.Password, dto.Email);
      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId = user.Id });

      return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);
    }

    // -- DELETE USER --
    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
      if (_dataService.GetUser(userId) == null)
        return NotFound("User not found.");

      _dataService.DeleteUser(userId);
      return NoContent();
    }
  }
}