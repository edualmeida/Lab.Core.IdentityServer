using System.ComponentModel.DataAnnotations;

namespace Lab.Core.IdentityServer.Pages.Manage.DeletePersonalData;

public class InputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
            
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}