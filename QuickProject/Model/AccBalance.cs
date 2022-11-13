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
    [Table("AccBalance")]
    public class AccBalance
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(18)]
        public string UserIban { get; set; }
        public double Balance { get; set; }

        public AccBalance()
        { }

        public static AccBalance InsertAccBalance(AccBalance balance)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}('{1}')", "InsertAccBalance", balance.UserIban));
            try
            {
                MainProcess.sql.database.InsertWithChildren(balance);
                MainProcess.sql.database.GetChildren<AccBalance>(balance);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertAccBalance", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return balance;
        }

        public static AccBalance UpdateAccBalance(AccBalance balance)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}('{1}')", "UpdateAccBalance", balance.UserIban));
            try
            {
                MainProcess.sql.database.UpdateWithChildren(balance);
                MainProcess.sql.database.GetChildren<AccBalance>(balance);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertAccBalance", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return balance;
        }

        public static List<AccBalance> DisplayAllAccBalance(int? id = null)
        {
            string szTxt = string.Empty;
            List<AccBalance> userList = new List<AccBalance>();
            if (!id.HasValue)
            {
                MainProcess.log.AppendLog(string.Format("> {0}", "DisplayAllAccBalance"));
            }
            try
            {
                string sql = "select * from AccBalance";
                if (id.HasValue)
                {
                    sql = string.Format("select * from AccBalance where id = '{0}'", id.Value);
                }
                userList = MainProcess.sql.database.Query<AccBalance>(sql);
                szTxt = String.Format("Query all from AccBalance Found [{0}]", userList.Count);
                foreach (var item in userList)
                {
                    szTxt = string.Format("\t\t\tFound id[{0}] Balance[{1}]", item.Id, item.Balance);
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
    }
}
