using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using DataLayer;
using Mapster;
using System;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        public ActionResult<PagedResponse<SearchName>> GetSearchNames(string searchTerm, int pageNumber = 1, int pageSize = DefaultPageSize)
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

            // Replace nconst with self-link
                foreach (var name in pagedNames)
                {
                if (name.NConst != null)
                { 
                    name.NConst = new Uri($"{Request.Scheme}://{Request.Host}/api/NameBasic/{name.NConst}").ToString();
                }
            }

            var response = CreatePagedResponse(pagedNames, pageNumber, pageSize, totalItems, "GetSearchNames");

            return Ok(response);
        }

        [HttpGet("title/{searchTerm}")]
        public ActionResult<PagedResponse<SearchTitle>> GetSearchTitles(string searchTerm, int pageNumber = 1, int pageSize = DefaultPageSize)
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

            // Replace tconst with self-link
            foreach (var title in pagedTitles)
            {
                if (title.TConst != null)
                {
                    title.TConst = new Uri($"{Request.Scheme}://{Request.Host}/api/Title/{title.TConst}").ToString();
                }
            }

            var response = CreatePagedResponse(pagedTitles, pageNumber, pageSize, totalItems, "GetSearchTitles");

            return Ok(response);
        }

        [HttpGet("title/numvotes")]
        public ActionResult<PagedResponse<SearchTitleNumvote>> GetSearchTitlesNumvote(string? searchTerm = "null", string? searchTitleType = "null", string? searchGenre = "null", int? searchYear = -1, int pageNumber = 1, int pageSize = DefaultPageSize)
        {
            var titles = _dataService.GetSearchTitlesNumvote(searchTerm, searchTitleType, searchGenre, searchYear);
            if (titles == null || !titles.Any())
            {
                return NotFound();
            }
            var totalItems = titles.Count();
            var pagedTitles = titles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var title in pagedTitles)
            {
                if (title.TConst != null)
                {
                    title.TConst = new Uri($"{Request.Scheme}://{Request.Host}/api/Title/{title.TConst}").ToString();
                }
            }
            var response = CreatePagedResponse(pagedTitles, pageNumber, pageSize, totalItems, "GetSearchTitlesNumvote");
            return Ok(response);
        }
        [HttpGet("title/rating")]
        public ActionResult<PagedResponse<SearchTitleRating>> GetSearchTitlesRating(string? searchTerm = "null", string? searchTitleType = "null", string? searchGenre = "null", int? searchYear = -1, int pageNumber = 1, int pageSize = DefaultPageSize)
        {
            var titles = _dataService.GetSearchTitlesRating(searchTerm, searchTitleType, searchGenre, searchYear);
            if (titles == null || !titles.Any())
            {
                return NotFound();
            }
            var totalItems = titles.Count();
            var pagedTitles = titles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var title in pagedTitles)
            {
                if (title.TConst != null)
                {
                    title.TConst = new Uri($"{Request.Scheme}://{Request.Host}/api/Title/{title.TConst}").ToString();
                }
            }
            var response = CreatePagedResponse(pagedTitles, pageNumber, pageSize, totalItems, "GetSearchTitlesRating");
            return Ok(response);
        }
    }
}