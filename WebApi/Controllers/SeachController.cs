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
    public class SearchController : BaseController
    {
        private readonly IDataService _dataService;

        public SearchController(IDataService dataService, LinkGenerator linkGenerator)
          : base(linkGenerator)
        {
            _dataService = dataService;
        }


        // --  --
        [HttpGet("name/{searchTerm}")]
        public ActionResult<SearchName> GetSearchNames(string searchTerm)
        {
            var names = _dataService.GetSearchNames(searchTerm);
            if (names == null)
            {
                return NotFound();
            }
            return Ok(names);
        }
        [HttpGet("title/{searchTerm}")]
        public ActionResult<SearchTitle> GetSearchTitles(string searchTerm)
        {
            var titles = _dataService.GetSearchTitles(searchTerm);
            if (titles == null)
            {
                return NotFound();
            }
            return Ok(titles);
        }
    }
}