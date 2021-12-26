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

        #region Roles In General

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public ActionResult<IEnumerable<ApplicationRole>> Get()
        {
            return roleManager.Roles.ToList();
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateRole([FromBody] NewRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roleExist = await roleManager.RoleExistsAsync(model.Name);

            if (roleExist == false)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,
                });

                if (result.Succeeded == false)
                    return Problem();
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

        [HttpDelete]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteRole([FromBody] NewRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await roleManager.FindByNameAsync(model.Name);

            if (role != null)
                await roleManager.DeleteAsync(role);

            return Ok();
        }

        #endregion


        #region Specific Role

        [HttpGet("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ApplicationRole>> GetById(string guid)
        {
            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
                return Ok(role);

            return NotFound();
        }

        [HttpPut("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ApplicationRole>> UpdateById(string guid, [FromBody] NewRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
            {
                role.Name = model.Name;

                await roleManager.UpdateAsync(role);

                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("{guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ApplicationRole>> DeleteById(string guid)
        {
            var role = await roleManager.FindByIdAsync(guid);

            if (role != null)
            {
                await roleManager.DeleteAsync(role);

                return Ok(role);
            }

            return NotFound();
        }

        #endregion

    }
}