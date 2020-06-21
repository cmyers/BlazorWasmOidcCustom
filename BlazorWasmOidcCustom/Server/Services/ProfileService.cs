using BlazorWasmOidcCustom.Server.Data;
using BlazorWasmOidcCustom.Server.Models;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorWasmOidcCustom.Server.Services
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

            if (context.RequestedClaimTypes.Any(x => x == JwtClaimTypes.Role))
            {
                //TODO: check user is a member of a role before adding, possibly utilising AspNetUserRoles
                claims.Add(new Claim(JwtClaimTypes.Role, "user"));
                claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            }

            context.IssuedClaims.AddRange(claims);
        }
    }
}
