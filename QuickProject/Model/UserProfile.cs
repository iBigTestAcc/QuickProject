using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickProject.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ConsoleProject.Model
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(18)]
        public string UserIban { get; set; }

        [ForeignKey(typeof(Country))]
        public int CoutryId { get; set; }

    }
}
