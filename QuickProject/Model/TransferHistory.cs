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

namespace QuickProject.Model
{
    [Table("TransferHistory")]
    internal class TransferHistory
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(User))]
        public int From { get; set; }

        [ForeignKey(typeof(User))]
        public int To { get; set; }

        public double Amount { get; set; }

        [ForeignKey(typeof(Currency))]
        public int CurrencyId { get; set; }
    }
}
