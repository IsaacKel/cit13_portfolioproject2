using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleCharacterController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TitleCharacterController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/TitleCharacter/{nconst}
        [HttpGet("{nconst}")]
        public ActionResult<IList<TitleCharacter>> GetTitleCharactersByPerson(string nconst)
        {
            return Ok(_dataService.GetTitleCharactersByPerson(nconst));
        }


    }
}
