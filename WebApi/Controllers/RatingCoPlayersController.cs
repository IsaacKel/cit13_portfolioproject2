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
    public class RatingCoPlayersController : BaseController
    {
        private readonly IDataService _dataService;

        public RatingCoPlayersController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{nConst}")]
        public ActionResult<RatingCoPlayer> GetRatingCoPlayers(string nConst)
        {
            var coplayer = _dataService.GetRatingCoPlayers(nConst);
            if (coplayer == null)
            {
                return NotFound();
            }
            return Ok(coplayer);
        }
    }
}