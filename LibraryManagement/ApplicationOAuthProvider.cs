using Microsoft.Owin.Security.OAuth;
using Models;
using MongoDB.Driver;
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
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            LoginDetails currentUser;
            UserDetails user = null;
            
            try
            {
                var database = LibManagementConnection.GetConnection();
                var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);

                var logins = await loginCollection.FindAsync(x => x.UserName == context.UserName && x.Password == context.Password);
                var loginsList = await logins.ToListAsync();
                currentUser = loginsList.FirstOrDefault();
                var users = await userCollection.FindAsync(x => x.UserID == currentUser.UserID);
                var usersList = await users.ToListAsync();
                user = usersList.FirstOrDefault();

                ////var builders = Builders<LoginDetails>.Filter.And(Builders<LoginDetails>.Filter.Where(x => x.UserName == context.UserName && x.Password == context.Password));
                ////currentUser = todoTaskCollection.FindAsync(builders);
                ////currentUser = entity.Logins.FirstOrDefault(x => x.Username == context.UserName && x.Password == context.Password).ID;

                ////user = entity.Users.FirstOrDefault(x => x.ID == currentUser);

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
                identity.AddClaim(new Claim("UserId", user.UserID));
                identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                context.Validated(identity);
            }
            else
                return;
        }
    }
}