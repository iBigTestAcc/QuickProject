using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Diagnostics;
using SQLite;
using ConsoleProject.Model;
using System.Collections;
using SQLiteNetExtensions.Extensions;
using SQLitePCL;
using QuickProject.Model;
using NLog.Fluent;
using System.Security.Policy;

namespace QuickProject
{

    public class SqlHandle
    {
        string szConString { get; set; }

        public SQLiteConnection database;

        public SqlHandle()
        {
            string szTxt = string.Empty;

            try
            {
                string Path = string.Format(@"{0}\{1}",Environment.CurrentDirectory, @"DB\TestDB.sqlite");
                if (!File.Exists(Path))
                {
                    if(!Directory.Exists(string.Format(@"{0}\{1}", Environment.CurrentDirectory, @"DB")))
                    {
                        Directory.CreateDirectory(string.Format(@"{0}\{1}", Environment.CurrentDirectory, @"DB"));
                    }
                    MainProcess.log.AppendLog("Database not found First time setting.", true);
                    CreateDB(Path);
                }
                else
                {
                    database = new SQLiteConnection(Path);
                }

                //Test(Path);
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "SqlHandle", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
        }

        public void Test(string szPath)
        {
            try
            {
                // test query
                var testC = database.Query<Country>("select * from Country");

                // test update query
                database.Query<Country>("update Country set name='lol' where id='1'");
                testC = database.Query<Country>("select * from Country");

                // update with obj
                database.UpdateWithChildren(new Country { Id = 1, Name = "Netherlands" });
                testC = database.Query<Country>("select * from Country");

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}] EX:[{1}]", "Test", ex.Message));
            }
        }
        public void CreateDB(string szPath)
        {
            string szTxt = string.Empty;
            bool bDone = true;
            MainProcess.log.AppendLog(string.Format("> {0}", "CreateDB"), true);

            try
            {
                database = new SQLiteConnection(szPath);

                CreateTable();

                CreateMandatoryData();
            }
            catch(Exception ex)
            {
                bDone = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateDB", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0} [{1}]", "CreateDB", bDone.ToString()), true);

        }

        public void CreateTable()
        {
            bool bDone = true;
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "CreateTable"), true);

            try
            {
                szTxt = string.Format("Create [{0}]", "Country");
                database.CreateTable<Country>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "Currency");
                database.CreateTable<Currency>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "DepositHistroy");
                database.CreateTable<DepositHistroy>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "FeeType");
                database.CreateTable<FeeType>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "TransactionHistory");
                database.CreateTable<TransactionHistory>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "TransactionType");
                database.CreateTable<TransactionType>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "TransferHistory");
                database.CreateTable<TransferHistory>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "User");
                database.CreateTable<User>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "UserProfile");
                database.CreateTable<UserProfile>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "UserPwd");
                database.CreateTable<UserPwd>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "AccBalance");
                database.CreateTable<AccBalance>();
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Create [{0}]", "CreateUsrHistory");
                database.CreateTable<CreateUsrHistory>();
                MainProcess.log.AppendLog(szTxt);
                
            }
            catch (Exception ex)
            {
                bDone = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateTable", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0} [{1}]", "CreateTable", bDone));
        }

        public void CreateMandatoryData()
        {
            string szTxt = string.Empty;
            bool bDone = true;
            MainProcess.log.AppendLog(string.Format("> {0}", "CreateMandatoryData"), true);

            try
            {
                // Country
                szTxt = string.Format("Add [{0}]", "Country");

                var c1 = new Country
                {
                    Name = "UNKNOW",
                    ShortName = "UNK"
                };
                database.Insert(c1);
                MainProcess.log.AppendLog(szTxt);

                c1 = new Country
                {
                    Name = "Netherlands",
                    ShortName = "NL"
                };
                database.Insert(c1);
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Add [{0}]", "Currency");
                var currency = new Currency
                {
                    CurrencyName = "EURO",
                    Symbol = "€"
                };
                database.Insert(currency);
                MainProcess.log.AppendLog(szTxt);

                szTxt = string.Format("Add [{0}]", "FeeType");
                var fee = new FeeType
                {
                    FeeAmount = 0.1,
                    Type = FeeEnum.ToDescriptionString(Fee.Percent)
                };
                database.Insert(fee);

                szTxt = string.Format("Add [{0}]", "FeeType");
                fee = new FeeType
                {
                    FeeAmount = 0,
                    Type = FeeEnum.ToDescriptionString(Fee.Amount)
                };
                database.Insert(fee);

                MainProcess.log.AppendLog(szTxt);

            }
            catch (Exception ex)
            {
                bDone = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateMandatoryData", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0} [{1}]", "CreateMandatoryData", bDone.ToString()));
        }
        public bool CheckNotHaveIban(string szIban)
        {
            bool bReturn = false;
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "CheckNotHaveIban", szIban));
            try
            {
                string szSql = string.Format("select id from UserProfile where UserIban = '{0}'", szIban);
                var usrProfile = database.Query<UserProfile>(szSql);

                if ((usrProfile == null) || usrProfile.Count == 0)
                {
                    bReturn = true;
                }
                else
                {
                    bReturn = false;
                }
                MainProcess.log.AppendLog(string.Format("Total profile found {0})", usrProfile.Count));
            }
            catch (Exception ex)
            {
                bReturn = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CheckNotHaveIban", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        public int GetCountryID(string szShortName)
        {
            int bReturn = 1;
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "GetCountryID", szShortName));
            try
            {
                string szSql = string.Format("select id from Country where ShortName = '{0}'", szShortName);
                var usrProfile = database.Query<UserProfile>(szSql);

                if ((usrProfile == null) || usrProfile.Count == 0)
                {
                    bReturn = 1;
                }
                else
                {
                    bReturn = usrProfile.FirstOrDefault().Id;
                }
                MainProcess.log.AppendLog(string.Format("Total Country found {0})", usrProfile.Count));
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetCountryID", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        

        
    }
}
