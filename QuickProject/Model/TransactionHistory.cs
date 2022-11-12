using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

namespace QuickProject.Model
{
    [Table("TransactionHistory")]
    internal class TransactionHistory
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(TransactionType))]
        public string Type { get; set; }

        [ForeignKey(typeof(TransferHistory))]
        public int TransferId { get; set; }

        [ForeignKey(typeof(DepositHistroy))]
        public int DepositId { get; set; }

        [ForeignKey(typeof(CreateUsrHistory))]
        public int CreateUsrId { get; set; }

        public string DateTime { get; set; }

        public static TransactionHistory InsertTransferHistory(TransactionHistory createObj)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}('{1}')", "InsertTransactionHistory", createObj.Id));
            try
            {
                MainProcess.sql.database.InsertWithChildren(createObj);
                MainProcess.sql.database.GetChildren<TransactionHistory>(createObj);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "InsertTransactionHistory", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return createObj;
        }
    }
}
