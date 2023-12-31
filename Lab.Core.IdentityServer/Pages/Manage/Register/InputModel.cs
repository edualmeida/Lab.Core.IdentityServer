using System.ComponentModel.DataAnnotations;

namespace Lab.Core.IdentityServer.Pages.Manage.Register;

public class InputModel
{
    public string UserId { get; set; }
    
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    //[BindProperty]
    [Display(Name = "Role")]
    public string SelectedRole { get; set; }
}