using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace QuickProject.Model
{
    public enum TrxType
    {
        [DescriptionAttribute("Transfer")]
        Transfer,
        [DescriptionAttribute("Deposit")]
        Deposit,
        [DescriptionAttribute("CreateUsr")]
        CreateUsr
    }

    public static class TrxTypeEnum
    {
        public static string ToDescriptionString(this TrxType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    [Table("TransactionType")]
    internal class TransactionType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(50)]
        public TrxType Type { get; set; }

    }
}
