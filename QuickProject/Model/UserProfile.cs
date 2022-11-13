using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using QuickProject;
using QuickProject.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

namespace ConsoleProject.Model
{
    [Table("UserProfile")]
    public class UserProfile
    {
        public enum ExceptionList
        {
            DuplicateIBAN
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(18)]
        public string UserIban { get; set; }

        [ForeignKey(typeof(Country))]
        public int CoutryId { get; set; }

        public UserProfile()
        { }

        public static UserProfile InsertUserProfile(UserProfile userProfile)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}('{1}')", "InsertUserProfile", userProfile.UserIban));
            try
            {
                MainProcess.sql.database.InsertWithChildren(userProfile);
                MainProcess.sql.database.GetChildren<UserProfile>(userProfile);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertUserProfile", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return userProfile;
        }

        public static List<UserProfile> DisplayAllUserProfile(int? id = null)
        {
            string szTxt = string.Empty;
            List<UserProfile> userList = new List<UserProfile>();
            if (!id.HasValue)
            {
                MainProcess.log.AppendLog(string.Format("> {0}", "DisplayAllUserProfile"));
            }
            try
            {
                string sql = "select * from UserProfile";
                if(id.HasValue)
                {
                    sql = string.Format("select * from UserProfile where id = '{0}'", id.Value);
                }
                userList = MainProcess.sql.database.Query<UserProfile>(sql);
                szTxt = String.Format("Query all from User Found [{0}]", userList.Count);
                foreach (var item in userList)
                {
                    szTxt = string.Format("\t\t\tFound id[{0}] UserIban[{1}]", item.Id, item.UserIban);
                    Console.WriteLine(szTxt);
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "DisplayAllUser", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return userList;
        }

        public static List<UserProfile> GetUsrProfile(string szIban)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            List<UserProfile> userList = new List<UserProfile>();
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "GetUsrProfile", szIban));

            try
            {
                string sql = string.Format("select * from UserProfile where UserIban = '{0}'", szIban);
                userList = MainProcess.sql.database.Query<UserProfile>(sql);
                szTxt = String.Format("Query all from User Found [{0}]", userList.Count);
                bDone = true;
            }
            catch(Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetUsrProfile", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "GetUsrProfile", szIban, bDone.ToString()));

            return userList;
        }

        public static UserProfile VerifyUsrProfile(List<UserProfile> profileList)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            UserProfile bRet = null;
            MainProcess.log.AppendLog(string.Format("> {0}(Count{1})", "VerifyUsrProfile", profileList.Count));

            try
            {
                if(profileList.Count > 1)
                {
                    szTxt = String.Format("Found more than 1 IBAN Exception:{0}", ExceptionList.DuplicateIBAN.ToString());
                    throw new Exception(szTxt);
                }
                else
                {
                    bRet = profileList.FirstOrDefault();
                }
                bDone = true;
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetUsrProfile", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            if (bRet != null)
            {
                MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "GetUsrProfile", bRet.UserIban, bDone.ToString()));
            }
            else
            {
                MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "GetUsrProfile", "NULL", bDone.ToString()));
            }
            return bRet;
        }

    }
}
