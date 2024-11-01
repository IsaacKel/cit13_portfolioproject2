﻿// using DataLayer;
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
    public class TitleCharacterController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TitleCharacterController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: api/TitleCharacter/{nconst}
        [HttpGet("{nconst}")]
        public ActionResult<IList<TitleCharacterDto>> GetTitleCharactersByPerson(string nconst)
        {
            var titleCharacters = _dataService.GetTitleCharactersByPerson(nconst);

            var titleCharacterDtos = titleCharacters.Select(tc => new TitleCharacterDto
            {
                NConst = tc.NConst,
                TConst = tc.TConst,
                Character = tc.Character,
                PrimaryTitle = tc.TitleBasic?.PrimaryTitle,
                Poster = tc.TitleBasic?.Poster
            }).ToList();

            return Ok(titleCharacterDtos);
        }
    }
}