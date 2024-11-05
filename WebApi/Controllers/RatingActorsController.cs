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
        public ActionResult<PagedResponse<RatingActor>> GetRatingActors(string tConst, int pageNumber = 1, int pageSize = 10)
        {
            var ratingActors = _dataService.GetRatingActors(tConst);
            if (ratingActors == null || !ratingActors.Any())
            {
                return NotFound();
            }

            var totalItems = ratingActors.Count();
            var pagedRatingActors = ratingActors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = CreatePagedResponse(pagedRatingActors, pageNumber, pageSize, totalItems, "GetRatingActors");

            return Ok(response);
        }
    }
}