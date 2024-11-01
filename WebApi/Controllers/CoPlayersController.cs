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
    public class CoPlayersController : BaseController
    {
        private readonly IDataService _dataService;

        public CoPlayersController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{nConst}")]
        public ActionResult<CoPlayer> GetCoPlayers(string nConst)
        {
            var coplayer = _dataService.GetCoPlayers(nConst);
            if (coplayer == null)
            {
                return NotFound();
            }
            return Ok(coplayer);
        }
    }
}