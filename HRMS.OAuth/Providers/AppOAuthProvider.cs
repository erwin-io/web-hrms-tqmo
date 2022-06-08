using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using HRMS.OAuth.Models;
using HRMS.Facade.Interface;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using HRMS.OAuth.Helpers;

namespace HRMS.OAuth.Providers
{
    public class AppOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserAuthFacade _userAuthFacade;
        #region CONSTRUCTORS
        public AppOAuthProvider(IUserAuthFacade userAuthFacade)
        {
            this._userAuthFacade = userAuthFacade;
        }
        #endregion

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            SystemUserViewModel user = await Task.Run(() => this._userAuthFacade.Find(context.UserName, context.Password));

            if (user != null && !string.IsNullOrEmpty(user?.SystemUserId))
            {
                double refreshTokenLifeTime = GlobalVariables.goRefreshTokenLifeTime;
                var identity = new ClaimsIdentity("JWT");

                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("SystemUserId", user.SystemUserId));

                var props = new AuthenticationProperties(new Dictionary<string, string>());
                var authProperties = new Dictionary<string, string>
                {
                    {"as:clientRefreshTokenLifeTime", refreshTokenLifeTime.ToString()},
                    { "SystemUserId", user.SystemUserId },
                };
                props = new AuthenticationProperties(authProperties);


                AuthenticationTicket ticket = new AuthenticationTicket(identity, props);

                context.Validated(ticket);
            }
            else
            {
                context.Rejected();
                context.SetError("invalid_grant", "The user name or password is incorrect.");
            }
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
        }
    }
}