﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Lab.Core.IdentityServer.Configuration;
using Lab.Core.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lab.Core.IdentityServer.Pages.Manage.DeletePersonalData
{
    [Authorize(Roles = RoleNames.AdminRole)]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet(string userId = null)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                Input = new InputModel
                {
                    Email = user.Email
                };
            }
            
            RequirePassword = true;//await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Input.Email);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{Input.Email}'.");
            }

            RequirePassword = true; //await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                var userAdmin = await _userManager.GetUserAsync(User);
                if (userAdmin == null)
                {
                    return NotFound($"Unable to load admin user with ID '{_userManager.GetUserId(User)}'.");
                }
                
                if (!await _userManager.CheckPasswordAsync(userAdmin, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
            StatusMessage = $"User with email: '{Input.Email}', id: '{userId}' deleted.";
                
            return Redirect("~/");
        }
    }
}
