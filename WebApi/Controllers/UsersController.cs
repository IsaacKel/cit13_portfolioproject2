using DataLayer;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(IDataService dataService, LinkGenerator linkGenerator,Hashing hashing) : base(linkGenerator)
        {
            _dataService = dataService;
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

            (var hashedPwd, var salt) =_hashing.Hash(model.Password);

            _dataService.CreateUser(model.Name, model.UserName, hashedPwd, model.Email, "user", salt);

            return Ok();

        }
           [HttpPut]
            public IActionResult Login(LoginUserModel model)
            {
               var user = _dataService.GetUser(model.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            if(!_hashing.Verify(model.Password, user.Password, user.Salt))
            {
                return BadRequest();
            }

            var claims = new List<Claim>
            {
          
                new Claim(ClaimTypes.Name, user.Username)
            };

            var secret = "asdajodaoijfiwåafjiæpølåøåløåløl";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

    }




    //try
    //{
    //    var user = _dataService.CreateUser(model.Name,model.UserName, model.Password, model.Email, "user","salt");
    //    return Ok(CreateUserDto(user));
    //}
    //catch
    //{
    //    return Unauthorized();
    //}
}