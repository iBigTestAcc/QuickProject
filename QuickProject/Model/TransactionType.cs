using System;
using System.Collections;
using System.Collections.Generic;
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
        Transfer,
        Deposit
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
