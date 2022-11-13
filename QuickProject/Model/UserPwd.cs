using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ConsoleProject.Model
{
    [Table("UserPwd")]
    public class UserPwd
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(UserProfile))]
        public int UserProfileId { get; set; }

        public int UsrPwd { get; set; }
    }
}
