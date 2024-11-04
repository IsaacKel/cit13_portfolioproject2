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

        private PagedResponse<T> CreatePagedResponse<T>(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems, string actionName)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var selfLink = GenerateFullLink(actionName, new { pageNumber, pageSize });
            var nextPageLink = pageNumber < totalPages ? GenerateFullLink(actionName, new { pageNumber = pageNumber + 1, pageSize }) : null;
            var prevPageLink = pageNumber > 1 ? GenerateFullLink(actionName, new { pageNumber = pageNumber - 1, pageSize }) : null;

            return new PagedResponse<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Links = new PaginationLinks
                {
                    Self = selfLink,
                    Next = nextPageLink,
                    Previous = prevPageLink
                },
                Items = items
            };
        }

        private string GenerateFullLink(string actionName, object routeValues)
        {
            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Url.Action(actionName, routeValues)}");
            return uri.ToString();
        }

        public class PagedResponse<T>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
            public PaginationLinks Links { get; set; }
            public IEnumerable<T> Items { get; set; }
        }

        public class PaginationLinks
        {
            public string Self { get; set; }
            public string Next { get; set; }
            public string Previous { get; set; }
        }
    }
}