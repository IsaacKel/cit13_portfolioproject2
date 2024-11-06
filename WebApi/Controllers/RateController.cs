using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using Mapster;
using System;
using DataLayer.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : BaseController
    {
        private readonly IDataService _dataService;

        public RateController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{tConst}/{rating}/{userId}")]
       public ActionResult rate(string tConst, int rating, int userId)
        {
            if (!_dataService.UserExists(userId))
            {
                return NotFound(new { message = "User does not exist." });
            }
            else if (_dataService.GetTitleByTConst(tConst) == null)
            {
                return NotFound(new { message = "Title does not exist." });
            }
            else if (rating < 1 || rating > 10)
            {
                return BadRequest(new { message = "Rating must be between 1 and 10." });
            }
            _dataService.rate(tConst, rating, userId);
            return Ok();
        }
    }
}