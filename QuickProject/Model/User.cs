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
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public int ProfileId { get; set; }

        [ForeignKey(typeof(UserPwd))]
        public int PwdId { get; set; }
    }
}
