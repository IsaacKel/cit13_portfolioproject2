using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitlePrincipalController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TitlePrincipalController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/TitlePrincipal/{tconst}
        [HttpGet("{tconst}")]
        public ActionResult<IList<TitlePrincipal>> GetTitlePrincipalsByTitle(string tconst)
        {
            return Ok(_dataService.GetTitlePrincipalsByTitle(tconst));
        }

    }
}
