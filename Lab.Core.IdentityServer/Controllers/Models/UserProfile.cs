using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lab.Core.IdentityServer.Controllers.Models
{
    public class UserProfile
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
        [StringLength(20)]
        public string PostalCode { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string Phone1 { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string Phone2 { get; set; }
    }
}
