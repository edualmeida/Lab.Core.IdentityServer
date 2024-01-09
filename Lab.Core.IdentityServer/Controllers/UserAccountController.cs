using AutoMapper;
using Lab.Core.IdentityServer.Controllers.Exceptions;
using Lab.Core.IdentityServer.Controllers.Models;
using Lab.Core.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Core.IdentityServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserAccountController(
            ILogger<UserProfileController> logger,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Post([FromRoute] string userId, [FromBody] ChangePassword changePassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors.Select(x => new RequestErrorDetail(x.Code, x.Description)).ToList();
                return BadRequest(new RequestError(errors));
            }

            return Ok();
        }
    }
}
