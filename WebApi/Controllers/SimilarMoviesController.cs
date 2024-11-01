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
    public class SimilarMoviesController : BaseController
    {
        private readonly IDataService _dataService;

        public SimilarMoviesController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("{tConst}")]
        public ActionResult<SimilarMovie> GetSimilarMovies(string tConst)
        {
            var similarMovies = _dataService.GetSimilarMovies(tConst);
            if (similarMovies == null)
            {
                return NotFound();
            }
            return Ok(similarMovies);
        }
    }
}