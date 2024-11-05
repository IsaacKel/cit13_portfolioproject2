using WebApi.DTOs;
using System.Linq;
using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnownForTitleController : ControllerBase
    {
        private readonly IDataService _dataService;

        public KnownForTitleController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/KnownForTitle/{nconst}
        [HttpGet("{nconst}")]
        public ActionResult<IList<KnownForTitleDto>> GetKnownForTitlesByName(string nconst)
        {
            var knownForTitles = _dataService.GetKnownForTitlesByName(nconst);

            var knownForTitleDtos = knownForTitles.Select(kft => new KnownForTitleDto
            {
                NConst = kft.NConst,
                KnownForTitles = kft.KnownForTitles,
                PrimaryTitle = kft.TitleBasic?.PrimaryTitle,
                Poster = kft.TitleBasic?.Poster
            }).ToList();

            return Ok(knownForTitleDtos);
        }
    }
}