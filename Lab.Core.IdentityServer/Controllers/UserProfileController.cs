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
    public class UserProfileController : ControllerBase
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserProfileController(
            ILogger<UserProfileController> logger,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<UserProfile> Get([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            //var roles = await _userManager.GetRolesAsync(user);

            return _mapper.Map<UserProfile>(user);
        }

        [HttpPost("{userId}")]
        public async Task Post([FromRoute] string userId, [FromBody] UserProfile updateProfile)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user.Address1 = updateProfile.Address1;
            user.Address2 = updateProfile.Address2;
            user.Phone2 = updateProfile.Phone2;
            user.Phone1 = updateProfile.Phone1;
            user.FullName = updateProfile.FullName;
            user.PostalCode = updateProfile.PostalCode;
            user.County = updateProfile.County;
            user.City = updateProfile.City;
            user.BirthDate = updateProfile.BirthDate;

            var resultEdit = await _userManager.UpdateAsync(user);

            if (!resultEdit.Succeeded)
            {
                throw new UserUpdateException(string.Join(";", resultEdit.Errors.Select(x=> x.Code + "-" + x.Description)));
            }
        }
    }
}
