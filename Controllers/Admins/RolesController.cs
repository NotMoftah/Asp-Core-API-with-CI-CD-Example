using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlsApi.Models;
using SlsApi.Responses;
using System.Net.Mime;

namespace SlsApi.Controllers
{
    [ApiController]
    [Route("api/v1/admin/[controller]")]
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
        public IEnumerable<ApplicationRole> Get()
        {
            return roleManager.Roles.ToList();
        }

        [HttpGet("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ApplicationRole>> GetById(string guid)
        {
            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
                return Ok(role);

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateNewRole([FromBody] NewRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roleExist = await roleManager.RoleExistsAsync(model.Name);

            if (roleExist == false)
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,
                });
            }

            var role = await roleManager.FindByNameAsync(model.Name);

            return Ok(new CreatedResponse
            { 
                Success = role != null,
                Name = model.Name,
                Id = role?.Id.ToString(),
                Location = role is null ? String.Empty : $"/api/v1/admin/roles/{role?.Id.ToString()}",
            });
        }
    }
}