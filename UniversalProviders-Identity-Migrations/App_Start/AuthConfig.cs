using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Membership.OpenAuth;
using System.Xml.Serialization;
using System.IO;

namespace UniversalProviders_Identity_Migrations
{
    internal static class AuthConfig
    {
        public static void RegisterOpenAuth()
        {
            // See http://go.microsoft.com/fwlink/?LinkId=252803 for details on setting up this ASP.NET
            // application to support logging in via external services.

            //OpenAuth.AuthenticationClients.AddTwitter(
            //    consumerKey: "your Twitter consumer key",
            //    consumerSecret: "your Twitter consumer secret");

            //OpenAuth.AuthenticationClients.AddFacebook(
            //    appId: "your Facebook app id",
            //    appSecret: "your Facebook app secret");

            //OpenAuth.AuthenticationClients.AddMicrosoft(
            //    clientId: "your Microsoft account client id",
            //    clientSecret: "your Microsoft account client secret");

            //OpenAuth.AuthenticationClients.AddGoogle();

            var _dbContext = new ApplicationDbContext();

            var dbProfiles = _dbContext.Profiles.Select(x => x);

            foreach (var dbProfile in dbProfiles)
            {
                var stringReader = new StringReader(dbProfile.PropertyValueStrings);

                var profile = new XmlSerializer(typeof(ProfileInfo)).Deserialize(stringReader) as ProfileInfo;

                var id = dbProfile.UserId.ToString();
                var user = _dbContext.Users.Where(x => x.Id == id).FirstOrDefault();
                user.Profile = profile;
            }

            _dbContext.SaveChanges();
        }
    }
}