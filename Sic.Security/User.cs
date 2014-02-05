using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Data.Models.Security;
using Sic.Data.DbConnection;

namespace Sic.Security
{    
    public abstract class User
    {
        public static bool ExistEmail(string email)
        {
            DbContextManagerService service = new DbContextManagerService();
            return service.Users.Get(p => p.Contact.Email == email).FirstOrDefault()!=null;
        }

        public static bool LoginAudit(int userId,int roleId)
        {
            try
            {
                UserLoginAuditBase audit = new UserLoginAuditBase();
                audit.UserId = userId;
                audit.RoleId = roleId;
                audit.LoginDate = DateTime.Now.ToUniversalTime();

                //TimeZoneInfo.GetSystemTimeZones();
                //TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                //audit.LoginDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), nzTimeZone);

                DbContextManagerService service = new DbContextManagerService();
                service.UserLoginAudits.Insert(audit);
                service.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string GenerateRandomPassword()
        {
            int PasswordLength = 8;
            string _allowedChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Byte[] randomBytes = new Byte[PasswordLength];
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                Random randomObj = new Random(DateTime.Now.Millisecond);
                randomObj.NextBytes(randomBytes);
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }

        public static bool ResetPassword(UserBase user)
        {
            DbContextManagerService service = new DbContextManagerService();
            user.DecodePassword = GenerateRandomPassword();
            user.Password = Authentication.GetEncodePassword(user.LogonName, user.DecodePassword);                

            UserBase userUpdate = service.Users.GetByID(user.UserId);
            userUpdate.DecodePassword = user.DecodePassword;
            userUpdate.Password = user.Password;
            
            service.Users.Update(userUpdate);
            service.Save();
            return true;
        }

        public static bool CreateUser(UserBase user,int organizationId)
        {
            string originalPassword = user.Password;

            try
            {
                if (user.UserId == 0)
                {
                    DbContextManagerService service = new DbContextManagerService();
                    RoleBase role = service.Roles.Get(p => p.OrganizationId == organizationId && p.IsDefault).FirstOrDefault();

                    UserRoleBase userRole = new UserRoleBase();
                    userRole.Role = role;
                    userRole.User = user;

                    user.Password = Sic.Security.Cryptography.EncodePassword(originalPassword);

                    service.Users.Insert(user);
                    service.UserRoles.Insert(userRole);
                    service.Save();

                    return true;
                }
            }
            catch (Exception ex)
            {
                user.Password = originalPassword;
                throw ex;
            }
            return false;
        }       
    }
}
