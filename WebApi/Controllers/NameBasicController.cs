using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameBasicController : ControllerBase
    {
        private readonly IDataService _dataService;

        public NameBasicController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/NameBasic/{nconst}
        [HttpGet("{nconst}")]
        public ActionResult<Person> GetNameBasicByNConst(string nconst)
        {
            var nameBasic = _dataService.GetPersonByNConst(nconst);
            if (nameBasic == null)
            {
                return NotFound();
            }
            return Ok(nameBasic);
        }

        // GET: api/NameBasic
        [HttpGet]
        public ActionResult<IList<Person>> GetAllNameBasics()
        {
            return Ok(_dataService.GetAllPersons());  // Assuming this retrieves all NameBasic entries
        }
    }
}
