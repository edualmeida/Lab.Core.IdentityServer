using AutoMapper;
using Lab.Core.IdentityServer.Controllers.Models;
using Lab.Core.IdentityServer.Models;

namespace Lab.Core.IdentityServer.Controllers.Mapping
{
    public class MainProfile : Profile
    {
        public MainProfile() 
        {
            CreateMap<ApplicationUser, UserProfile>();
        }
    }
}
