using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "user")]
    public class NameBasicController : ControllerBase
    {
        private readonly IDataService _dataService;

        public NameBasicController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/NameBasic/{nconst}
        [HttpGet("{nconst}")]
     //   [Authorize(Roles = "user")]
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
        [HttpGet]
       // [Authorize(Roles = "admin")]
        public ActionResult<IList<NameBasic>> GetAllNameBasics()
        {
           
                return Ok(_dataService.GetAllNames());  // Need paging
      
       }
}
}
