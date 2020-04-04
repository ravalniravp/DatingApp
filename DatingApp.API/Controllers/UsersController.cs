using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.DTOS;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users =await _repo.GetUser();
            
            var usersToReturn = _mapper.Map<IEnumerable<UsersForListDTO>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UsersForDetailedDTO>(user);

            return Ok(userToReturn);
        }

    }
}