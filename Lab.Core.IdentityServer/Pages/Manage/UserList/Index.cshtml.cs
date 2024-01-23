using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Lab.Core.IdentityServer.Configuration;
using Lab.Core.IdentityServer.Models;
using Lab.Core.IdentityServer.Pages.Grants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace Lab.Core.IdentityServer.Pages.Manage.UserList;

[SecurityHeaders]
[Authorize(Roles = RoleNames.AdminRole)]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public Index(UserManager<ApplicationUser> userManager
    )
    {
        _userManager = userManager;
    }

    [TempData]
    public string StatusMessage { get; set; }
    public ViewModel View { get; set; }
    public string NameSort { get; set; }
    public string CurrentFilter { get; set; }
    public string CurrentSort { get; set; }
    
    public async Task OnGet(string sortOrder, string currentFilter, string searchString, int? pageIndex)
    {
        CurrentSort = sortOrder;
        NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        
        if (searchString != null)
        {
            pageIndex = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        CurrentFilter = searchString;
       
        var users = _userManager.Users;
        
        if (!String.IsNullOrEmpty(searchString))
        {
            users = users.Where(s => s.UserName.Contains(searchString));
        }
        
        switch (sortOrder)
        {
            case "name_desc":
                users = users.OrderByDescending(s => s.UserName);
                break;
            default:
                users = users.OrderBy(s => s.UserName);
                break;
        }
        
        int pageSize = 3;
        View = new ViewModel
        {
            Users = await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex ?? 1, pageSize),
        };
    }

    public async Task<IActionResult> OnPost()
    {

        return RedirectToPage("/Grants/Index");
    }
}