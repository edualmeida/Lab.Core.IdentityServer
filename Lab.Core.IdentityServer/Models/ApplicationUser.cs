// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Lab.Core.IdentityServer.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [StringLength(300)]
    public string FullName { get; set; }
    [PersonalData]
    public DateTime BirthDate { get; set; }
    [PersonalData]
    [StringLength(50)]
    public string Address1 { get; set; }
    [PersonalData]
    [StringLength(50)]
    public string Address2 { get; set; }
    [PersonalData]
    [StringLength(25)]
    public string City { get; set; }
    [PersonalData]
    [StringLength(25)]
    public string County { get; set; }
    [PersonalData]
    [StringLength(10)]
    public string PostalCode { get; set; }
}
