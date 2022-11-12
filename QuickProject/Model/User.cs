using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V105.Page;
using QuickProject;
using QuickProject.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

namespace ConsoleProject.Model
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public int ProfileId { get; set; }

        [ForeignKey(typeof(AccBalance))]
        public int BalanceId { get; set; }

        [ForeignKey(typeof(UserPwd))]
        public int PwdId { get; set; }

        public User()
        {

        }

        public static User InsertUser(User user)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}('{1}')", "InsertUser", user.ProfileId));
            try
            {
                MainProcess.sql.database.InsertWithChildren(user);
                MainProcess.sql.database.GetChildren<User>(user);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertTable", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return user;
        }

        public static void DisplayAllUser()
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "DisplayAllUser"));
            try
            {
                List<User> userList = new List<User>();
                userList = MainProcess.sql.database.Query<User>("select * from User");
                szTxt = String.Format("Query all from User Found [{0}]", userList.Count);
                foreach(var item in userList)
                {
                    szTxt = string.Format("\t\t\tFound id[{0}] ProfileId[{1}]", item.Id, item.ProfileId);
                    Console.WriteLine(szTxt);
                    var profile = UserProfile.DisplayAllUserProfile(item.ProfileId);
                    var balance = AccBalance.DisplayAllAccBalance(item.BalanceId);
                    Console.WriteLine("--------------------------------------------\n");
                    //if (profile != null && profile.Count > 0)
                    //{
                    //    foreach (var pro in profile)
                    //    {
                    //        szTxt = string.Format("\t\t\t\tProfileId[{0}] UserIban[{1}]", pro.Id, pro.UserIban);
                    //        Console.WriteLine(szTxt);
                    //        
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "DisplayAllUser", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
        }
    }

}
