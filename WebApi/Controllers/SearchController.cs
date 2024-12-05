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

        [HttpGet("title")]
        public ActionResult<PagedResponse<SearchTitle>> GetSearchTitles(
            string? query = "null",
            string? sortBy = "popularity",
            string? titleType = "null",
            string? genre = "null",
            int? year = -1,
            int pageNumber = 1,
            int pageSize = DefaultPageSize)
        {
            titleType = UnformatTitleType(titleType);
            genre = genre?.ToLower();
            IEnumerable<SearchTitle> titles;

            if (sortBy == "rating")
            {
                var ratingTitles = _dataService.GetSearchTitlesRating(query, titleType, genre, year);
                titles = ratingTitles.Select(rt => new SearchTitle
                {
                    TConst = rt.TConst,
                    PrimaryTitle = rt.PrimaryTitle,
                    Poster = rt.Poster,
                    Rating = rt.Rating,
                    StartYear = rt.StartYear,
                    Genre = rt.Genre
                });
            }
            else
            {
                var numvoteTitles = _dataService.GetSearchTitlesNumvote(query, titleType, genre, year);
                titles = numvoteTitles.Select(nt => new SearchTitle
                {
                    TConst = nt.TConst,
                    PrimaryTitle = nt.PrimaryTitle,
                    Poster = nt.Poster,
                    Rating = nt.Rating,
                    StartYear = nt.StartYear,
                    Genre = nt.Genre
                });
            }

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

            var response = CreatePagedResponse(pagedTitles, pageNumber, pageSize, totalItems, "GetSearchTitles");
            return Ok(response);
        }

        private string UnformatTitleType(string titleType)
        {
            if (string.IsNullOrEmpty(titleType))
            {
                return titleType;
            }

            // Remove spaces and capitalize the first letter of each word
            titleType = titleType.Replace(" ", "");

            // Replace "TV" with "Tv"
            titleType = titleType.Replace("TV", "Tv");

            // Lowercase the first letter
            if (titleType.Length > 0)
            {
                titleType = char.ToLower(titleType[0]) + titleType.Substring(1);
            }

            return titleType;
        }
    }
}