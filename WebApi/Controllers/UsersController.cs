using DataLayer;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v3/users")]
    public class UsersController : BaseController
    {
        private readonly IDataService _dataService;

        public UsersController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
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

        


            try
            {
                var user = _dataService.CreateUser(model.Name,model.UserName, model.Password, model.Email, "user","salt");
                return Ok(RegisterDTO(user));
            }
            catch
            {
                return Unauthorized();
            }
        }

    }
}
