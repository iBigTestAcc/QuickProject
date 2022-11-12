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
    [Table("TransactionHistory")]
    internal class TransactionHistory
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(TransactionType))]
        public int Type { get; set; }

        [ForeignKey(typeof(TransferHistory))]
        public int TransferId { get; set; }

        [ForeignKey(typeof(DepositHistroy))]
        public int DepositId { get; set; }
    }
}
