// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Lab.Core.IdentityServer.Configuration;
using Lab.Core.IdentityServer.Models;
using Lab.Core.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Lab.Core.IdentityServer.Pages.Manage.Register
{
    [Authorize(Roles = RoleNames.AdminRole)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailNotifier _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailNotifier emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }        
        public SelectList Roles { get; set; }
        public bool IsEdit { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string userId = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Roles = new SelectList(_roleManager.Roles, "NormalizedName", "Name");
            
            Input = new InputModel
            {
                SelectedRole = _roleManager.Roles.FirstOrDefault()?.Name,
            };
            
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                var roles = await _userManager.GetRolesAsync(user);

                Input = new InputModel
                {
                    Email = user.Email,
                    SelectedRole = roles.FirstOrDefault(),
                    UserId = userId
                };
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var t = Input.SelectedRole;
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Input.UserId))
                {
                    var editUser = await _userManager.FindByIdAsync(Input.UserId);
                    if (editUser == null)
                    {
                        return NotFound($"Unable to load user with ID '{Input.UserId}'.");
                    }
                    
                    await _userStore.SetUserNameAsync(editUser, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(editUser, Input.Email, CancellationToken.None);
                    await _userManager.RemoveFromRolesAsync(editUser, _roleManager.Roles.ToList().Select(x=>x.Name));
                    await _userManager.AddToRoleAsync(editUser, Input.SelectedRole);
                    
                    StatusMessage = $"User email: '{Input.Email}', id: '{Input.UserId}' saved successfully";
                    return Page();
                }
                
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, Input.SelectedRole);
                    
                    var userId = await _userManager.GetUserIdAsync(user);
                    var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    confirmationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail/Index",
                        pageHandler: null,
                        values: new { userId, code = confirmationCode, returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        StatusMessage = $"User email: '{Input.Email}', id: '{userId}' registered successfully";
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
