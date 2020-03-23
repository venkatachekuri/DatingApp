using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Dtos;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DatingAppAPI.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
         private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo , IConfiguration config )
        {
              _repo = repo;
              _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

        //    if(!ModelState.IsValid)
        //    return BadRequest(ModelState);

           userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
           if(await _repo.UserExists(userForRegisterDto.Username))
           return BadRequest("Username already exists!!");

           var userToCreate = new User 
           {
               Username = userForRegisterDto.Username
           };

           var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

           return StatusCode(201);
           
        }

           [HttpPost("login")]
           public async Task<IActionResult> Login(UserForLoginDTO userForLoginDto)
           {
            
            //throw new System.Exception("Computer Says No!!");

            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower() , userForLoginDto.Password);
            
            if(userFromRepo==null)
            return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name , userFromRepo.Username.ToString())
            };


            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key , Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                  Subject = new ClaimsIdentity(claims),
                  Expires = System.DateTime.Now.AddDays(1),
                  SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            }
            );

           }
    }
}