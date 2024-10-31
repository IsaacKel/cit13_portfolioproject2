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
    public class RatingCrewController : BaseController
    {
        private readonly IDataService _dataService;

        public RatingCrewController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{tConst}")]
        public ActionResult<RatingCrew> GetRatingCrew(string tConst)
        {
            var ratingCrew = _dataService.GetRatingCrew(tConst);
            if (ratingCrew == null)
            {
                return NotFound();
            }
            return Ok(ratingCrew);
        }
    }
}