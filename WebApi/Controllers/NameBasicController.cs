using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameBasicController : BaseController
    {
        private readonly IDataService _dataService;

        public NameBasicController(IDataService dataService, LinkGenerator linkGenerator)
            : base(linkGenerator)
        {
            _dataService = dataService;
        }

        // GET: api/NameBasic/{nconst}
        [HttpGet("{nconst}")]
        public ActionResult<NameBasic> GetNameBasicByNConst(string nconst)
        {
            var nameBasic = _dataService.GetNameByNConst(nconst);
            if (nameBasic == null)
            {
                return NotFound();
            }
            return Ok(nameBasic);
        }

        // GET: api/NameBasic
        [HttpGet(Name = "GetAllNameBasics")]
        public ActionResult<IList<NameBasic>> GetAllNameBasics(int page = 1, int pageSize = DefaultPageSize)
        {
            var allNames = _dataService.GetAllNames();

            var paginatedNames = CreatePaging("GetAllNameBasics", page, pageSize, allNames.Count,
                                              allNames.Skip((page - 1) * pageSize).Take(pageSize));

            return Ok(paginatedNames);
        }
    }
}
