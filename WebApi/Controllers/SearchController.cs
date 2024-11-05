using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using Mapster;
using System;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

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

        [HttpGet("name/{searchTerm}")]
        public ActionResult<PagedResponse<SearchName>> GetSearchNames(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var names = _dataService.GetSearchNames(searchTerm);
            if (names == null || !names.Any())
            {
                return NotFound();
            }

            var totalItems = names.Count();
            var pagedNames = names
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = CreatePagedResponse(pagedNames, pageNumber, pageSize, totalItems, "GetSearchNames");

            return Ok(response);
        }

        [HttpGet("title/{searchTerm}")]
        public ActionResult<PagedResponse<SearchTitle>> GetSearchTitles(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var titles = _dataService.GetSearchTitles(searchTerm);
            if (titles == null || !titles.Any())
            {
                return NotFound();
            }

            var totalItems = titles.Count();
            var pagedTitles = titles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = CreatePagedResponse(pagedTitles, pageNumber, pageSize, totalItems, "GetSearchTitles");

            return Ok(response);
        }
    }
}