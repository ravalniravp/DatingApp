using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.DTOS;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;

        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult>
        Register(UserForRegisterDTO userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _repo.UserExists(userForRegisterDto.UserName))
                ModelState
                    .AddModelError("Username", "Username already exist. ");

            // validate the request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userToCreate =
                new User { UserName = userForRegisterDto.UserName };

            var createdUser =
                await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDto)
        {
            var userFromRepo =
                await _repo
                    .LogIN(userForLoginDto.UserName.ToLower(),
                    userForLoginDto.Password);

            if (userFromRepo == null) return Unauthorized();

            var claims =
                new []
                {
                    new Claim(ClaimTypes.NameIdentifier,
                        userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.UserName)
                };

            var key =
                new SymmetricSecurityKey(Encoding
                        .UTF8
                        .GetBytes(_config
                            .GetSection("AppSettings:Token")
                            .Value));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
