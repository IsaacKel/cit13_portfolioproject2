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
    public class TitleController : BaseController
    {
        private readonly IDataService _dataService;

        public TitleController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // -- GET USER by ID --
        [HttpGet("{tConst}")]
        public ActionResult<TitleBasic> GetTitleByTConst(string tConst)
        {
            var titleBasic = _dataService.GetTitleByTConst(tConst);
            if (titleBasic == null)
            {
                return NotFound();
            }
            return Ok(titleBasic);
        }
    }
}