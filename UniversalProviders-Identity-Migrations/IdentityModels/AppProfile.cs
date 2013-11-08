using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversalProviders_Identity_Migrations.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Xml.Serialization;
using System.IO;

namespace UniversalProviders_Identity_Migrations.IdentityModels
{
    public class AppProfile
    {
        private static ApplicationDbContext _dbContext;

        private string username;

        static AppProfile()
        {
            _dbContext = new ApplicationDbContext();
        }

        public AppProfile()
        {
            username = HttpContext.Current.User.Identity.Name;
        }

        public AppProfile(string userName)
        {
            username = userName;
        }

        private ProfileInfo _profileInfo;
        public ProfileInfo ProfileInfo
        {
            get
            {
                if (_profileInfo == null)
                {
                    _profileInfo = GetProfileInfoFromDb(); // return from Db based on current user
                }

                return _profileInfo;
            }
        }

        public static AppProfile GetProfile()
        {
            return new AppProfile();
        }

        public static AppProfile GetProfile(string userName)
        {
            // get profile for user from db
            // serialize data and return
            return new AppProfile(userName);
        }

        public void Save()
        {
            StoreProfileInfoInDb(_profileInfo);
        }

        private ProfileInfo GetProfileInfoFromDb()
        {
            var manager = new UserManager();

            var user = manager.FindByName(username);
            var userIdGuid = Guid.Parse(user.Id);

            var dbProfile = _dbContext.Profiles.Where(x => x.UserId == userIdGuid).FirstOrDefault<UserDbProfile>();

            if (dbProfile == null)
            {
                return new ProfileInfo();
            }

            var stringReader = new StringReader(dbProfile.PropertyValueStrings);

            return new XmlSerializer(typeof(ProfileInfo)).Deserialize(stringReader) as ProfileInfo;
        }

        private void StoreProfileInfoInDb(ProfileInfo profileInfo)
        {
            var manager = new UserManager();

            var user = manager.FindByName(username);

            var stringWriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(ProfileInfo));

            serializer.Serialize(stringWriter, profileInfo);

            var userIdGuid = Guid.Parse(user.Id);

            var dbProfile = _dbContext.Profiles.Where(x => x.UserId == userIdGuid).FirstOrDefault<UserDbProfile>();

            if (dbProfile == null)
            {
                dbProfile = new UserDbProfile() { UserId = Guid.Parse(user.Id), PropertyValueStrings = stringWriter.ToString(), 
                    PropertyNames = "", PropertyValueBinary = new byte[1],LastUpdatedDate=DateTime.Now };
                _dbContext.Profiles.Add(dbProfile);

            }
            else
            {
                dbProfile.PropertyValueStrings = stringWriter.ToString();
                dbProfile.LastUpdatedDate = DateTime.Now;
            }

            _dbContext.SaveChanges();
        }
    }
}