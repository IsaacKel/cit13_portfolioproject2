using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitlePrincipalController : BaseController
    {
        private readonly IDataService _dataService;

        public TitlePrincipalController(IDataService dataService, LinkGenerator linkGenerator)
            : base(linkGenerator)
        {
            _dataService = dataService;
        }

        // GET: api/TitlePrincipal/by-name/{nconst}
        [HttpGet("by-name/{nconst}", Name = "GetTitlePrincipalsByName")]
        public ActionResult<IEnumerable<TitlePrincipal>> GetTitlePrincipalsByName(string nconst, int page = 1, int pageSize = DefaultPageSize)
        {
            var titlePrincipals = _dataService.GetTitlePrincipalsByName(nconst);
            var totalCount = titlePrincipals.Count;

            // Apply pagination
            var pagedResult = titlePrincipals.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = CreatePagingNConst("GetTitlePrincipalsByName", nconst, page, pageSize, totalCount, titlePrincipals.Skip((page - 1) * pageSize).Take(pageSize));

            return Ok(result);
        }



        // GET: api/TitlePrincipal/by-title/{tconst}
        [HttpGet("by-title/{tconst}")]
        public ActionResult<IList<TitlePrincipal>> GetTitlePrincipalsByTitle(string tconst)
        {
            return Ok(_dataService.GetTitlePrincipalsByTitle(tconst));
        }


        [HttpGet("{tConst}/principals")]
        public ActionResult<IList<TitlePrincipal>> GetTitlePrincipals(string tConst)
        {
            try
            {
                var results = _dataService.GetTitlePrincipals(tConst);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Controller: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
