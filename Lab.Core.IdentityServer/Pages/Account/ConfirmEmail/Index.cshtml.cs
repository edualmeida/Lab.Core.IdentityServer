﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab.Core.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Lab.Core.IdentityServer.Pages.Account.ConfirmEmail
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel  
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;
        
        public ConfirmEmailModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public bool FailedToConfirm { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl)
        {
            if (userId == null || code == null)
            {
                return NotFound($"Unable to load user with ID '{userId}' or code: '{code}'.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            
            string coded  = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, coded);
            if (!result.Succeeded)
            {
                StatusMessage = "Error confirming your email.";
                _logger.LogError(result.Errors.FirstOrDefault()?.ToString());
                FailedToConfirm = true;
            }
            
            Input = new InputModel()
            {
                UserId = userId,
                ReturnUrl = returnUrl
            };

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.RemovePasswordAsync(user);
            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            
            StatusMessage = "Thank you for confirming your email.";
            
            return LocalRedirect(Input.ReturnUrl);
        }
    }
}
