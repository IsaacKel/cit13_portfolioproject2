// using Microsoft.AspNetCore.Mvc;
// using WebApi.DTOs;
// using DataLayer;
// using Mapster;
// using Microsoft.AspNetCore.Authorization;

// namespace WebApi.Controllers
// {
//   [ApiController]
//   [Route("api/[controller]")]
//   public class UserController : BaseController
//   {
//     private readonly IDataService _dataService;

//     public UserController(IDataService dataService, LinkGenerator linkGenerator)
//       : base(linkGenerator)
//     {
//       _dataService = dataService;
//     }

//     // -- GET USER by ID --
//     [HttpGet("{userId}")]
//     public IActionResult GetUser(int userId)
//     {
//       var user = _dataService.GetUser(userId);
//       if (user == null) return NotFound();

//       var userDto = user.Adapt<UserDTO>();
//       userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId });

//       return Ok(userDto);
//     }

//     // -- GET USER by USERNAME --
//     [HttpGet("username/{username}")]
//     public IActionResult GetUserByUsername(string username)
//     {
//       var user = _dataService.GetUser(username);
//       if (user == null) return NotFound();

//       var userDto = user.Adapt<UserDTO>();
//       userDto.SelfLink = GenerateSelfLink(nameof(GetUserByUsername), new { username });

//       return Ok(userDto);
//     }

//     // -- REGISTER USER (Create) --
//     [HttpPost("register")]
//     public IActionResult RegisterUser([FromBody] UserRegisterDTO dto)
//     {
//       if (!ModelState.IsValid ||
//           string.IsNullOrWhiteSpace(dto.Username) ||
//           string.IsNullOrWhiteSpace(dto.Password) ||
//           string.IsNullOrWhiteSpace(dto.Email))
//       {
//         return BadRequest(ModelState);
//       }

//       var user = _dataService.AddUser(dto.Username, dto.Password, dto.Email);
//       var userDto = user.Adapt<UserDTO>();
//       userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId = user.Id });

//       return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);
//     }

//     // -- DELETE USER --
//     [HttpDelete("{userId}")]
//     [Authorize(Roles = "admin")]
//     public IActionResult DeleteUser(int userId)
//     {
//       if (_dataService.GetUser(userId) == null) return NotFound();

//       _dataService.DeleteUser(userId);
//       return NoContent();
//     }
//   }
// }

using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.DTOs;
using WebApi.Services;
using Mapster;

namespace WebApi.Controllers
{
  [ApiController]
  [Route("api/v3/users")]
  public class UsersController : BaseController
  {
    private readonly IDataService _dataService;
    private readonly Hashing _hashing;
    private readonly IConfiguration _configuration;

    public UsersController(IDataService dataService, IConfiguration configuration, LinkGenerator linkGenerator, Hashing hashing)
        : base(linkGenerator)
    {
      _configuration = configuration;
      _dataService = dataService;
      _hashing = hashing;
    }

    // -- GET USER by ID --
    [HttpGet("{userId}")]
    public IActionResult GetUser(int userId)
    {
      var user = _dataService.GetUser(userId);
      if (user == null) return NotFound();

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId });

      return Ok(userDto);
    }

    // -- GET USER by USERNAME --
    [HttpGet("username/{username}")]
    public IActionResult GetUserByUsername(string username)
    {
      var user = _dataService.GetUser(username);
      if (user == null) return NotFound();

      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUserByUsername), new { username });

      return Ok(userDto);
    }

    // -- REGISTER USER (Create) --
    [HttpPost("register")]
    public IActionResult RegisterUser([FromBody] UserRegisterDTO dto)
    {
      if (!ModelState.IsValid ||
          string.IsNullOrWhiteSpace(dto.Username) ||
          string.IsNullOrWhiteSpace(dto.Password) ||
          string.IsNullOrWhiteSpace(dto.Email))
      {
        return BadRequest(ModelState);
      }

      var user = _dataService.AddUser(dto.Username, dto.Password, dto.Email);
      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId = user.Id });

      return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);
    }

    // -- CREATE USER --
    [HttpPost]
    public IActionResult CreateUser(CreateUserModel model)
    {
      if (_dataService.GetUser(model.UserName) != null)
      {
        return BadRequest("User already exists");
      }
      if (string.IsNullOrEmpty(model.Password))
      {
        return BadRequest("No password");
      }
      if (string.IsNullOrEmpty(model.Email))
      {
        return BadRequest("No email");
      }

      (var hashedPwd, var salt) = _hashing.Hash(model.Password);

      _dataService.CreateUser(model.Name, model.UserName, hashedPwd, model.Email, salt, model.Role);

      return Ok("Created User");
    }

    // -- LOGIN USER --
    [HttpPut] // Login
    public IActionResult Login(LoginUserModel model)
    {
      var user = _dataService.GetUser(model.UserName);

      if (user == null)
      {
        return BadRequest("Can't login. Password or Username is wrong.");
      }

      if (!_hashing.Verify(model.Password, user.Password, user.Salt))
      {
        return BadRequest("Couldn't verify.");
      }

      var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

      var secret = _configuration.GetSection("Auth:Secret").Value;
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      // Token generated
      var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.Now.AddDays(7),
          signingCredentials: creds
      );

      var Jwt = new JwtSecurityTokenHandler().WriteToken(token);

      return Ok(new { username = user.Username, token = Jwt });
    }

    // -- DELETE USER --
    [HttpDelete("{userId}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteUser(int userId)
    {
      if (_dataService.GetUser(userId) == null) return NotFound();

      _dataService.DeleteUser(userId);
      return NoContent();
    }
  }
}