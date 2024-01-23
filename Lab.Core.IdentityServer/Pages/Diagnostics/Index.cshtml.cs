using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Lab.Core.IdentityServer.Pages.Grants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Lab.Core.IdentityServer.Pages.Diagnostics
{
    [SecurityHeaders]
    [Authorize]
    public class IndexModel : PageModel
    {
        public ViewModel View { get; set; }

        public async Task<IActionResult> OnGet()
        {
            //var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };
            //if (!localAddresses.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            //{
            //    return NotFound();
            //}

            View = new ViewModel(await HttpContext.AuthenticateAsync());

            return Page();
        }
    }
}
