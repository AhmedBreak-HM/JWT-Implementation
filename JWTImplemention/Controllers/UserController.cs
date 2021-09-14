using JWTImplemention.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace JWTImplemention.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        List<User> users;
        public UserController()
        {
            users = new List<User>()
            {
                new User(){Id = 1,Name="ahmed",Password="123456"},
                new User(){Id = 2,Name="muhamed",Password="564342"},
                new User(){Id = 3,Name="hazeem",Password="233456y67"},
                new User(){Id = 4,Name="amer",Password="123456"}
            };
        }
        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            return Ok(users);
        }
        [HttpGet("GetById")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            return user;
        }
        [HttpPost]
        public ActionResult CreateUser([FromBody]User user)
        {
            users.Add(user);
            return CreatedAtAction("GetById",new { id = user.Id},user);
        }
    }
}
