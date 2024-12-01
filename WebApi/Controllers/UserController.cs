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
  [Route("api/v3/user")]
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

    // -- GET USER by Email --
    [HttpGet("email/{email}")]
    public IActionResult GetUserByEmail(string email)
    {
       var user = _dataService.GetUserByEmail(email);
       if (user == null) return NotFound();

       var userDto = user.Adapt<UserDTO>();
       userDto.SelfLink = GenerateSelfLink(nameof(GetUserByEmail), new { email });

       return Ok(userDto);
    }

        // -- REGISTER USER / CREATE USER --
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

            if (_dataService.GetUser(dto.Username) != null)
            {
                return BadRequest("User already exists");
            }
            if (_dataService.GetUserByEmail(dto.Email) != null)
            {
                return BadRequest("Email already exists");
            }
            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("No password");
            }
            if (dto.Password.Length < 8)
            {
                return BadRequest("Password must be at least 8 characters long.");
            }

            if (!dto.Password.Any(char.IsUpper))
            {
                return BadRequest("Password must contain at least one uppercase letter.");
            }

            if (!dto.Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return BadRequest("Password must contain at least one special character.");
            }

            if (!dto.Password.Any(char.IsDigit))
            {
                return BadRequest("Password must contain at least one digit.");
            }

            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("No email");
            }





            (var hashedPwd, var salt) = _hashing.Hash(dto.Password);

     var user = _dataService.CreateUser(dto.Name, dto.Username, hashedPwd, dto.Email, salt, dto.Role);

      //var user = _dataService.AddUser(dto.Username, dto.Password, dto.Email);
      var userDto = user.Adapt<UserDTO>();
      userDto.SelfLink = GenerateSelfLink(nameof(GetUser), new { userId = user.Id });

      return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, userDto);
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

      return Ok(new { username = user.Username, token = Jwt, user.Id });
    }

    // -- DELETE USER --
    [HttpDelete("{userId}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteUser(int userId)
    {
      if (_dataService.GetUser(userId) == null) return NotFound("Coundn't deplete, because ID not found");

      _dataService.DeleteUser(userId);
      return NoContent();
    }
  }
}