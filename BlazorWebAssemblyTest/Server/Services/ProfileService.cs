using BlazorWebAssemblyTest.Server.Data;
using BlazorWebAssemblyTest.Server.Models;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorWebAssemblyTest.Server.Services
{
    public class AuthProfileService : ProfileService<ApplicationUser>
    {
        protected UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public AuthProfileService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);
            var user = await _userManager.GetUserAsync(context.Subject);
            var claims = new List<Claim>();

            if (context.RequestedClaimTypes.Any(x => x == "Resource.API.Test.access.level"))
            {
                //TODO: check user access here before adding the claims required
                claims.Add(new Claim("access_level", "auth.admin")); 
            }

            context.IssuedClaims.AddRange(claims);
        }
    }
}
