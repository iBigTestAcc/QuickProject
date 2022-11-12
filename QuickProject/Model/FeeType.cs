using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleProject.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace QuickProject.Model
{
    public enum Fee
    {
        [DescriptionAttribute("Percent")]
        Percent,
        [DescriptionAttribute("Amount")]
        Amount
    }

    public static class FeeEnum
    {
        public static string ToDescriptionString(this Fee val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    [Table("FeeType")]
    internal class FeeType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Type { get; set; }

        public double FeeAmount { get; set; }

        public enum ExceptionList
        {
            FeeNotFound
        }
        public static List<FeeType> GetFee(TrxType transactionType)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            List<FeeType> feeList = new List<FeeType>();
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "GetFee", transactionType));

            try
            {
                if (transactionType == TrxType.Deposit)
                {
                    // ALL DEP IS 1% fee
                    string sql = string.Format("select * from FeeType where FeeAmount = '{0}' AND Type = '{1}'", 0.1, FeeEnum.ToDescriptionString(Fee.Percent));
                    //string sql = string.Format("select * from FeeType");
                    feeList = MainProcess.sql.database.Query<FeeType>(sql);
                    szTxt = String.Format("Query all from Fee Found [{0}]", feeList.Count);
                    bDone = true;
                }
                else
                {
                    // transfer no fee
                    string sql = string.Format("select * from FeeType where FeeAmount = '{0}' AND Type = '{1}'", 0, Fee.Amount);
                    feeList = MainProcess.sql.database.Query<FeeType>(sql);
                    szTxt = String.Format("Query all from Fee Found [{0}]", feeList.Count);
                    bDone = true;
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetFee", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "GetFee", transactionType, bDone.ToString()));

            return feeList;
        }
    }
}
