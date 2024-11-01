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
    public class RatingActorsController : BaseController
    {
        private readonly IDataService _dataService;

        public RatingActorsController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{tConst}")]
        public ActionResult<RatingActor> GetRatingActors(string tConst)
        {
            var ratingActors = _dataService.GetRatingActors(tConst);
            if (ratingActors == null)
            {
                return NotFound();
            }
            return Ok(ratingActors);
        }
    }
}