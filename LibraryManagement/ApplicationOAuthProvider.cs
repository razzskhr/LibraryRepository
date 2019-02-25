using Common.EncryptionRepository;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace LibraryManagement
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private IPasswordRepository passwordRepository;
        public ApplicationOAuthProvider()
        {

        }
        public ApplicationOAuthProvider(IPasswordRepository passwordRepository)
        {
            this.passwordRepository = passwordRepository;
        }
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            LoginDetails currentUser;
            UserDetails user = null;
            PasswordRepository passwordRepository = new PasswordRepository();
            string encryptedPassword= await passwordRepository.GetEncryptedPassword(context.Password);
            try
            {
                var database = LibManagementConnection.GetConnection();
                var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);

                var logins = await loginCollection.FindAsync(x => x.UserName.ToLower() == context.UserName.ToLower() && x.Password == encryptedPassword);
                var loginsList = await logins.ToListAsync();
                currentUser = loginsList.FirstOrDefault();
                var users = await userCollection.FindAsync(x => x.UserID == currentUser.UserID);
                var usersList = await users.ToListAsync();
                user = usersList.FirstOrDefault();                

            }
            catch (Exception e)
            {
                return;
            }


            if (user != null)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Email", user.Email));
                identity.AddClaim(new Claim("FirstName", user.FirstName));
                identity.AddClaim(new Claim("LastName", user.LastName));
                identity.AddClaim(new Claim("UserName", user.UserName));
                identity.AddClaim(new Claim("UserId", user.UserID ?? string.Empty));
                identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.RoleType.ToString()));
                var additionalData = new AuthenticationProperties(new Dictionary<string, string> { 
                {
                    "role", JsonConvert.SerializeObject(user.RoleType.ToString())
                }});
                var token = new AuthenticationTicket(identity, additionalData);
                context.Validated(identity);
            }
            else
                return;
        }
    }
}