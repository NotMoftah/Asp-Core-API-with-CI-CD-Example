using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlsApi.Models;
using SlsApi.Responses;
using System.Net.Mime;

namespace SlsApi.Controllers
{
    [ApiController]
    [Route("api/v1.0/admin/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public RolesController(ILogger<RolesController> logger,
                               UserManager<ApplicationUser> userManager,
                               RoleManager<ApplicationRole> roleManager)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public ActionResult<IEnumerable<ApplicationRole>> Get()
        {
            return roleManager.Roles.ToList();
        }

        [HttpGet("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ApplicationRole>> GetById([FromRoute] string guid)
        {
            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
                return Ok(role);

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateRole([FromBody] NewRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // cehck role with the same name doesnt exist, then create it
            if (await roleManager.RoleExistsAsync(model.Name))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,
                });
            }

            // check no database shit hass accured before sending ok.
            var role = await roleManager.FindByNameAsync(model.Name);

            if (role is null)
                return Problem();

            return Ok(new CreatedResponse
            { 
                Success = true,
                Name = model.Name,
                Id = role?.Id.ToString()
            });
        }

        [HttpPut("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ApplicationRole>> UpdateRole([FromRoute] string guid)
        {
            var role = await roleManager.FindByIdAsync(guid);

            if (role is null)
                return NotFound();

            return BadRequest($"Role {role.Name} can not be updated!");
        }

        [HttpDelete("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteRole([FromQuery] string guid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
                await roleManager.DeleteAsync(role);

            return Ok();
        }

    }
}