using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NameBasicController : ControllerBase
    {
        private readonly IDataService _dataService;

        public NameBasicController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/NameBasic/{nconst}
        [HttpGet("{nconst}")]
        [Authorize]
        public ActionResult<NameBasic> GetNameBasicByNConst(string nconst)
        {
            try
            { 
              var nameBasic = _dataService.GetNameByNConst(1,nconst);
              if (nameBasic == null)
                {
                return NotFound();
                }
                return Ok(nameBasic);
            }
            catch
            {
                return Unauthorized();
            }
        }

        // GET: api/NameBasic
        [HttpGet]
        [Authorize]

        public ActionResult<IList<NameBasic>> GetAllNameBasics()
        {
            try
            {
                return Ok(_dataService.GetAllNames(1));  // Assuming this retrieves all NameBasic entries
            }
            catch
            {
                return Unauthorized();
            }
       }
}
}
