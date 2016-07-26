using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ContactManager.Models;
using Newtonsoft.Json.Linq;

namespace ContactManager.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {            
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            // internal check with local db
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            // reading json file
            string jsonString = System.IO.File.ReadAllText(@"G:\users.json");
            // parsing the json data            
            JArray jsonVal = JArray.Parse(jsonString) as JArray;
            dynamic users = jsonVal;
            // flag to track a valid user
            Boolean validUser = false;
            // looping through each entry of array list in the json
            foreach (dynamic user_data in users)
            {
                // getting the username from the file
                string username = user_data.Username.ToString();
                // checking if the user name is a match, if yes proceed
                if ((context.UserName).Equals(username))
                {
                    // reading hashed password from the file
                    string hash_password = user_data.Password.ToString();
                    // using BCrypt library to verify the hashed password by providing user inputted password
                    if (BCrypt.Net.BCrypt.Verify(context.Password, hash_password))
                    {
                        // if password match then set boolean to true and break the loop
                        validUser = true;
                        break;
                    }
                }
            }

            // satisfying both internal db and json file check
            if (!validUser && user != null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}