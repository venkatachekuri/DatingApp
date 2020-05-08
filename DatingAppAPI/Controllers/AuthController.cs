using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Dtos;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace DatingAppAPI.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
         private readonly IConfiguration _config;
            private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo , IConfiguration config , IMapper mapper )
        {
              _repo = repo;
              _config = config;
              _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

        //    if(!ModelState.IsValid)
        //    return BadRequest(ModelState);

           userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
           if(await _repo.UserExists(userForRegisterDto.Username))
           return BadRequest("Username already exists!!");

           var userToCreate = _mapper.Map<User>(userForRegisterDto);

           var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

           var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

           return CreatedAtRoute("GetUser", new {Controller = "Users" , id = createdUser.Id } , userToReturn);
           
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

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            }
            );

           }
    }
}