using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace QuickProject.Model
{
    [Table("Currency")]
    internal class Currency
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(50)]
        public string CurrencyName { get; set; }

        [SQLite.MaxLength(5)]
        public string Symbol { get; set; }
    }
}
