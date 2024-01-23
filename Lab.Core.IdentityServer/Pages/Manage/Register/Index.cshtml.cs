using System.Text;
using System.Text.Encodings.Web;
using Lab.Core.IdentityServer.Configuration;
using Lab.Core.IdentityServer.Models;
using Lab.Core.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

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
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public SelectList Roles { get; set; }
        [BindProperty]
        public bool IsEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string userId = null)
        {
            ReturnUrl = returnUrl;
            IsEdit = !string.IsNullOrEmpty(userId);
            SetInputForView(false, null, _roleManager.Roles.FirstOrDefault()?.Name);
            if (IsEdit)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                SetInputForView(IsEdit, user, (await _userManager.GetRolesAsync(user)).FirstOrDefault());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            IdentityResult result = null;

            if (ModelState.IsValid)
            {
                if (IsEdit)
                {
                    var editUser = await _userManager.FindByIdAsync(Input.UserId);
                    if (editUser == null)
                    {
                        return NotFound($"Unable to load user with ID '{Input.UserId}'.");
                    }

                    SetUserFields(editUser);

                    result = await _userManager.UpdateAsync(editUser);
                    if (result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(editUser, _roleManager.Roles.ToList().Select(x => x.Name));
                        await _userManager.AddToRoleAsync(editUser, Input.SelectedRole);

                        StatusMessage = $"User saved successfully";
                        SetInputForView(IsEdit, editUser, Input.SelectedRole);
                        return Page();
                    }
                }
                else
                {
                    var user = CreateUser();

                    SetUserFields(user);

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    result = await _userManager.CreateAsync(user);

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

                        StatusMessage = $"User email: '{Input.Email}', id: '{userId}' registered successfully";
                        return LocalRedirect("/Manage/UserList/Index");
                    }
                }

                SetResultErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void SetResultErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private void SetInputForView(bool isEdit, ApplicationUser user, string role)
        {
            Roles = new SelectList(_roleManager.Roles, "NormalizedName", "Name");
            if (isEdit)
            {
                Input = new InputModel
                {
                    Email = user.Email,
                    SelectedRole = role,
                    UserId = user.Id,
                    FullName = user.UserName,
                    BirthDate = user.BirthDate,
                    Address1 = user.Address1,
                    Address2 = user.Address2,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    County = user.County,
                    Phone1 = user.Phone1,
                    Phone2 = user.Phone2,
                };
            }
            else
            {
                Input = new InputModel
                {
                    SelectedRole = _roleManager.Roles.FirstOrDefault()?.Name,
                    BirthDate = DateTime.Now
                };
            }
        }

        private void SetUserFields(ApplicationUser user)
        {
            user.FullName = Input.FullName;
            user.BirthDate = Input.BirthDate;
            user.Address1 = Input.Address1;
            user.Address2 = Input.Address2;
            user.City = Input.City;
            user.County = Input.County;
            user.PostalCode = Input.PostalCode;
            user.Phone1 = Input.Phone1;
            user.Phone2 = Input.Phone2;
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
