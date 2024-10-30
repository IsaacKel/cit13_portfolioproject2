using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using Mapster;
using System.Threading.Tasks;

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
    public async Task<IActionResult> GetUser(int userId)
    {
      var user = await _dataService.GetUserAsync(userId);
      if (user == null) return NotFound();

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = await GenerateSelfLinkAsync(nameof(GetUser), new { userId });

      return Ok(userDto);
    }

    // -- GET USER by USERNAME --
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
      var user = await _dataService.GetUserAsync(username);
      if (user == null) return NotFound();

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = await GenerateSelfLinkAsync(nameof(GetUserByUsername), new { username });

      return Ok(userDto);
    }

    // -- REGISTER USER (Create) --
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO dto)
    {
      if (!ModelState.IsValid ||
          string.IsNullOrWhiteSpace(dto.Username) ||
          string.IsNullOrWhiteSpace(dto.Password) ||
          string.IsNullOrWhiteSpace(dto.Email))
      {
        return BadRequest(ModelState);
      }

      var user = await _dataService.AddUserAsync(dto.Username, dto.Password, dto.Email);
      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = await GenerateSelfLinkAsync(nameof(GetUser), new { userId = user.Id });

      return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);
    }

    // -- DELETE USER --
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
      if (await _dataService.GetUserAsync(userId) == null) return NotFound();

      await _dataService.DeleteUserAsync(userId);
      return NoContent();
    }
  }
}