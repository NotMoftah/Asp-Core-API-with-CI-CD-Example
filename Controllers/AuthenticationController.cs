using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SlsApi.Models;
using SlsApi.Responses;
using SlsApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace SlsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly JwtTokenService jwtTokenService;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthenticationController(JwtTokenService jwtTokenService,
                                        UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Register([FromBody] UsernamePasswordModel model)
        {
            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new BaseResponse { Success = true });

            return StatusCode(StatusCodes.Status400BadRequest, new BaseResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description)
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Login([FromBody] UsernamePasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null) 
                return Unauthorized();

            var correntPassowrd = await userManager.CheckPasswordAsync(user, model.Password);

            if (correntPassowrd == false)
                return Unauthorized();

            var userRoles = await userManager.GetRolesAsync(user);
            var jwtToken = jwtTokenService.GenerateTokenWithRoles(user.Id, userRoles);

            return Ok(new TokenResponse
            {
                Success = true,
                Token = jwtToken
            });
        }      
    }
}
