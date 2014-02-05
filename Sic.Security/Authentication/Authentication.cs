using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Data.DbConnection;
using Sic.Data.Models.Security;

namespace Sic.Security
{
    public class Authentication
    {
        public static string GetEncodePassword(string logonName,string password)
        {
            string passwordCombine = string.Concat(logonName, password, logonName);
            string encryptedPassword = Sic.Security.Cryptography.EncodePassword(passwordCombine);
            return encryptedPassword;
        }

        public static bool Validity(string logonNameOrEmail, string password)
        {
            DbContextManagerService context = new DbContextManagerService();

            UserBase user = context.Users.Get(p => p.Active && (p.LogonName == logonNameOrEmail || p.Contact.Email == logonNameOrEmail)).SingleOrDefault();

            if (user != null)
            {
                string encryptedPassword = GetEncodePassword(user.LogonName,password);

                if (user.Password == encryptedPassword)
                    return true;
            }

            return false;
        }

        public static bool ChangePassword(string logonNameOrEmail, string password, string newPassword)
        {
            DbContextManagerService context = new DbContextManagerService();

            UserBase user = context.Users.Get(p => (p.LogonName == logonNameOrEmail || p.Contact.Email == logonNameOrEmail)).SingleOrDefault();

            if (user != null)
            {
                string encryptedPassword = GetEncodePassword(user.LogonName, password);

                if (user.Password == encryptedPassword)
                {                    
                    user.Password = GetEncodePassword(user.LogonName, newPassword);
                    context.Users.Update(user);
                    context.Save();
                    return true;
                }
            }

            return false;
        }    
    }
}
