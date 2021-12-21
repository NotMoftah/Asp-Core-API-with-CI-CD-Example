using Application.Identity;
using SlsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SlsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly SymmetricSecurityKey symmetricSecurityKey;

        public AuthenticationController(IConfiguration configuration,
                                        UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;

            symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Check if the Email already resigtred
            if (await userManager.FindByEmailAsync(model.Email) != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new RegisterResponse
                                  {
                                      Success = false,
                                      Status = "ERROR",
                                      Message = "User already exists!"
                                  });
            }

            var user = new ApplicationUser()
            {
                UserName = "user",
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new RegisterResponse
                {
                    Success = true,
                    Status = "Registred",
                    Message = "User Registred Successfully."
                });

            return StatusCode(StatusCodes.Status500InternalServerError,
            new RegisterResponse
            {
                Success = false,
                Status = "ERROR",
                Message = string.Join(Environment.NewLine, result.Errors.Select(x => "Code " + x.Code + " Description" + x.Description))
            }); ;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null) 
                return Unauthorized();

            var validPassword = await userManager.CheckPasswordAsync(user, model.Password);

            if (validPassword == false) 
                return Unauthorized();

            var jwtToken = GenerateJwtWithClaims(user.Id, null);

            return StatusCode(StatusCodes.Status200OK,
                              new LoginResponse
                              {
                                  ServerId = user.Id.ToString(),
                                  JWTToken = jwtToken
                              });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("upgrade")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Upgrade([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return Unauthorized();

            var validPassword = await userManager.CheckPasswordAsync(user, model.Password);

            if (validPassword == false)
                return Unauthorized();

            // Generate claims
            var claims = new List<Claim> { new Claim("Role", Application.Roles.User)};
            var jwtToken = GenerateJwtWithClaims(user.Id, claims);

            return StatusCode(StatusCodes.Status200OK,
                              new LoginResponse
                              {
                                  ServerId = user.Id.ToString(),
                                  JWTToken = jwtToken
                              });
        }


        private string GenerateJwtWithClaims(Guid uid, List<Claim>? userClaims)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, uid.ToString())
            };

            if (userClaims?.Count > 0)
                claims.AddRange(userClaims);

            var token = new JwtSecurityToken
            (
                claims: claims,
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
