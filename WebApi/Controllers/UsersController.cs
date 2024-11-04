using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebApi.DTOs;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v3/users")]
    public class UsersController : BaseController
    {
        private readonly IDataService _dataService;
        private readonly Hashing _hashing;
        private readonly IConfiguration _configuration;

        public UsersController(IDataService dataService, IConfiguration configuration, LinkGenerator linkGenerator,Hashing hashing) : base(linkGenerator)
        {
            _configuration = configuration;
            _dataService = dataService;
            _hashing = hashing;
        }

        [HttpPost]
        public IActionResult CreateUser( CreateUserModel model)
        {
            // should be checking for password strength here
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

            (var hashedPwd, var salt) =_hashing.Hash(model.Password);

            _dataService.CreateUser(model.Name, model.UserName, hashedPwd, model.Email,  salt,"user");

            return Ok("Created User");

        }
           [HttpPut] // Login
            public IActionResult Login(LoginUserModel model)
            {
               var user = _dataService.GetUser(model.UserName);

            if (user == null)
            {
                return BadRequest("Can't login. Password or Username is wrong Is wrong");
            }

            if(!_hashing.Verify(model.Password, user.Password, user.Salt))
            {
                return BadRequest("Could'nt Verify");
            }

            var claims = new List<Claim>
            {
          
                new Claim(ClaimTypes.Name, user.Username)
            };

            var secret = _configuration.GetSection("Auth:Secret").Value;
         
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Token generated
            var token = new JwtSecurityToken(
                claims: claims,
                //Life time on token
                expires: DateTime.Now.AddSeconds(45),
                signingCredentials: creds
                );

            var Jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { username = user.Username, token = Jwt });
        }

    }

}