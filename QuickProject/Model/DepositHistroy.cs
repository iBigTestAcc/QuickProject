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
    [Table("DepositHistroy")]
    internal class DepositHistroy
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Fee))]
        public int FeeId { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public int UserProfileId { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public string UserIban { get; set; }

        public double Amount { get;set; }

        public double NetAmount { get; set; }

        [ForeignKey(typeof(Currency))]
        public int CurrencyId { get; set; }

        public string DateTime {get; set; }
    }
}
