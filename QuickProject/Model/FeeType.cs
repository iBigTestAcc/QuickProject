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
    public enum Fee
    {
        Percent,
        Amount
    }
    [Table("FeeType")]
    internal class FeeType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Fee Type { get; set; }

        public double FeeAmount { get; set; }

    }
}
