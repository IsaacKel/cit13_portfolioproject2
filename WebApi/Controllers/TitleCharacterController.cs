// using DataLayer;
// using DataLayer.Models;
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;

// namespace WebApi.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class TitleCharacterController : ControllerBase
//     {
//         private readonly IDataService _dataService;

//         public TitleCharacterController(IDataService dataService)
//         {
//             _dataService = dataService;
//         }

//         // GET: api/TitleCharacter/{nconst}
// [HttpGet("{nconst}")]
// public ActionResult<IList<TitleCharacter>> GetTitleCharactersByPerson(string nconst)
// {
//     return Ok(_dataService.GetTitleCharactersByPerson(nconst));
// }


//     }
// }


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
    public class TitleCharacterController : BaseController
    {
        private readonly IDataService _dataService;

        // public TitleCharacterController(IDataService dataService)
        // {
        //     _dataService = dataService;
        // }

        public TitleCharacterController(IDataService dataService, LinkGenerator linkGenerator)
        : base(linkGenerator)
        {
            _dataService = dataService;
        }
        [HttpGet("{nconst}", Name = "GetTitleCharactersByPerson")]
        public ActionResult<IList<TitleCharacterDto>> GetTitleCharactersByPerson(string nconst, int page = 1, int pageSize = DefaultPageSize)
        {
            Console.WriteLine($"Received request for nconst: {nconst} on Page: {page} with Page Size: {pageSize}");

            var titleCharacters = _dataService.GetTitleCharactersByPerson(nconst);
            var totalItems = titleCharacters.Count;

            var pagedTitleCharacters = titleCharacters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(tc => new TitleCharacterDto
                {
                    Character = tc.Character,
                    PrimaryTitle = tc.TitleBasic?.PrimaryTitle,
                    Poster = tc.TitleBasic?.Poster
                }).ToList();

            Console.WriteLine($"Paged Character Count: {pagedTitleCharacters.Count}, Total Items: {totalItems}");

            // Use the current nconst in pagination links
            var result = CreatePaging("GetTitleCharactersByPerson", page, pageSize, totalItems, pagedTitleCharacters);
            return Ok(result);
        }
    }
}