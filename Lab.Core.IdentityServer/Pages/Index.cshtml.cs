using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Core.IdentityServer.Pages.Home;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    public string Version;
    [TempData]
    public string StatusMessage { get; set; }
    public void OnGet()
    {
        Version = typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();
    }
}