using Lab.Core.IdentityServer.Models;

namespace Lab.Core.IdentityServer.Pages.Manage.UserList;

public class ViewModel
{
    public PaginatedList<ApplicationUser> Users { get; set; }
}