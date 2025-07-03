using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DTOs.TaskDTO;
using WebApplication1.DTOs.UserDTO;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/auths")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IConfiguration configuration;

        public AuthController(UserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
        }

        //Only admin can create account
        [HttpPost("register")]
        [IsAdmin]
        public ActionResult CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                // Hash password of the input DTO
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.PasswordHash);

                // Create a new DTO with hashed password instead of using the original ( because its password is not hashed)
                CreateUserDTO dtoWithHashedPassword = new CreateUserDTO
                {
                    Email = createUserDTO.Email,
                    PasswordHash = hashedPassword,
                    UserName = createUserDTO.UserName
                };
                UserDTO newUser = userService.CreateUser(dtoWithHashedPassword);
                return Ok();
            } catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }
       
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDTO loginUserDTO)
        {
            try
            {
                User foundUser = userService.AuthenticateUser(loginUserDTO);
                if(foundUser != null)
                {
                    string token = GenerateToken(foundUser);
                    return Ok(token);
                }
                return Unauthorized(new { success = false, message = "Invalid username or password." });
            } catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { success = false, message = ex.Message});
            }
        }


        //This function is used to create access token which lasts 15 minutes

        private string GenerateToken(User foundUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()),
                new Claim(ClaimTypes.Name, foundUser.UserName),
                new Claim(ClaimTypes.Email, foundUser.Email),
                new Claim(ClaimTypes.Role, foundUser.Role?.Name ?? ""),
            };
            var token = new JwtSecurityToken(
               issuer: configuration["Jwt:Issuer"],
               audience: configuration["Jwt:Audience"],
               claims: claims,
               expires: DateTime.Now.AddMinutes(15),
               signingCredentials: credentials
           );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
