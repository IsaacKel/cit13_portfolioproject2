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
        public ActionResult<IList<KnownForTitle>> GetKnownForTitlesByPerson(string nconst)
        {
            return Ok(_dataService.GetKnownForTitlesByPerson(nconst));
        }

    }
}
