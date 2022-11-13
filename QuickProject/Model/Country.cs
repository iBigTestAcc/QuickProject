using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ConsoleProject.Model
{
    public enum CountryTxt
    {
        [DescriptionAttribute("NL")]
        Netherlands = 0,

        [DescriptionAttribute("UNK")]
        Unknow = 999
    }
    public static class CountryEnum
    {
        public static string ToDescriptionString(this CountryTxt val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    [Table("Country")]
    public class Country
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(50)]
        public string Name { get; set; }

        [SQLite.MaxLength(3)]
        public string ShortName { get; set; }

    }
}
