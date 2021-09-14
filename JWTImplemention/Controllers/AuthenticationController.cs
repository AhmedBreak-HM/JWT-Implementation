using JWTImplemention.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace JWTImplemention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private static List<User> users = new List<User>() {

            new User() { Id = 1, Name = "ahmed", Password = "123456" },
            new User() { Id = 2, Name = "muhamed", Password = "564342" },
            new User() { Id = 3, Name = "hazeem", Password = "233456y67" },
            new User() { Id = 4, Name = "amer", Password = "123456" }
       };
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("register")]
        public ActionResult<User> CreateUser(User user)
        {
            user.Name = user.Name.ToLower();
            var userName = users.Find(u => u.Name == user.Name);
            if (userName != null) return BadRequest("this Name is Use");
            user.Password = BC.HashPassword(user.Password);
            users.Add(user);
            return Ok("the user is Register");
        }

        [HttpPost("logIn")]
        public ActionResult<User> LogIn(string name, string password)
        {
            name = name.ToLower();
            var user = users.Find(u => u.Name == name);
            if (user == null) return BadRequest("this user is not found");

            if (!BC.Verify(password, user.Password)) return 
                    BadRequest("this Password  is not True");

            // Create Token

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name)
            };

            // genreated key and convert to bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:TokenKey").Value));

            // genreated Credentials by hashing key
            var Creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // genreate Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = Creds
            };

            // genreate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });

        }
    }
}
