using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Lab.Core.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace Lab.Core.IdentityServer.Services.Account;

public class CustomProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CustomProfileService> Logger;
    
    public CustomProfileService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        ILogger<CustomProfileService> logger)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
        Logger = logger;
    }
    
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject?.GetSubjectId();
        if (sub == null) throw new Exception("No sub claim present");
    
        var user = await _userManager.FindByIdAsync(sub);
        if (user == null)
        {
            Logger?.LogWarning("No user found matching subject Id: {0}", sub);
        }
        else
        {
            var principal = await _claimsFactory.CreateAsync(user);
            if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");
            context.IssuedClaims.AddRange(principal.Claims);

            var customClaims = new List<Claim>
            {
                new Claim("FullName", user.FullName),
                new Claim("Address1", user.Address1),
                new Claim("Address2", user.Address2),
                new Claim("Address2", user.Address2),
                new Claim("Address2", user.Address2),
                new Claim("Address2", user.Address2),
                new Claim("Address2", user.Address2),
                new Claim("Address2", user.Address2),
            };

            context.IssuedClaims.AddRange(customClaims);
        }
    }
    
    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject?.GetSubjectId();
        if (sub == null) throw new Exception("No subject Id claim present");
    
        var user = await _userManager.FindByIdAsync(sub);
        if (user == null)
        {
            Logger?.LogWarning("No user found matching subject Id: {0}", sub);
        }
    
        context.IsActive = user != null;
    }
}