using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Data;
using server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public UserController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userObj.Email);
            if (user == null)
                return NotFound(new { Message = "User Not Found!" });
            if (!PasswordHasher.Verifypassword(userObj.Password, user.Password))
                return BadRequest(new { Message = "Password is Incorrect" });
            user.Token = CreateJwtToken(user);
            System.Diagnostics.Debug.WriteLine("hiiiiiiiiiiiiiiiiiiiiiiiiiiii");
            return Ok(new 
            {
                Token = user.Token,
                UserId = user.Id,
                Message = "Login Success!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            if(await CheckIfEmailExistAsync(userObj.Email))
                return BadRequest(new {Message="Email Already Exist"});
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Id = Guid.NewGuid();
            await _appDbContext.Users.AddAsync(userObj);
            await _appDbContext.SaveChangesAsync();
            return Ok(new {Message = "User Registered!"});
        }

        private Task<bool> CheckIfEmailExistAsync(string email)
            => _appDbContext.Users.AnyAsync(x=>x.Email == email);
        private string CreateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Email,user.Email),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }



        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u=>u.Id == id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        [HttpPut]
        [Route("edit/{id:Guid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, User updateUserRequest)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            user.Name = updateUserRequest.Name;
            user.Email = updateUserRequest.Email;
            await _appDbContext.SaveChangesAsync();
            return this.Ok(user);
        }
    }
}
