using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleProject.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

namespace QuickProject.Model
{
    [Table("CreateUsrHistory")]
    internal class CreateUsrHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [ForeignKey(typeof(User))]
        public int UserId { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public int UserProfileId { get; set; }

        [ForeignKey(typeof(AccBalance))]
        public int AccBalanceId { get; set; }

        public string DateTime { get; set; }

        public static CreateUsrHistory InsertCreateUsrHistory(CreateUsrHistory createObj)
        {
            string szTxt = string.Format("> {0}('{1}')", "InsertCreateUsrHistory", createObj.Id);
            MainProcess.log.AppendLog(szTxt);
            try
            {
                MainProcess.sql.database.InsertWithChildren(createObj);
                MainProcess.sql.database.GetChildren<CreateUsrHistory>(createObj);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertCreateUsrHistory", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return createObj;
        }
    }
}
