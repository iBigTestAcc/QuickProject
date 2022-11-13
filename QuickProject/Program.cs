using ConsoleProject;
using ConsoleProject.Model;
using QuickProject.Logic;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickProject
{
    public class MainProcess
    {
        public static string szNlogConfigPath { get; set; }
        public static string szNlogPath { get; set; }

        public static LogEvents log = null;
        public static SqlHandle sql = null;

        // for test
        public static double dMaxDepAmount = Properties.Settings.Default.dMaxDepAmount;
        public static double dMinDepAmount = Properties.Settings.Default.dMinDepAmount;
        public static double dMaxTransAmount = Properties.Settings.Default.dMaxTransferAmount;
        public static double dMinTransAmount = Properties.Settings.Default.dMinTransferAmount;

        static void Main(string[] args)
        {
            try
            {
                Init();

                sql = new SqlHandle();

                MainMenu main = new MainMenu();
                main.StartMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("EX:{0}", ex.Message));
            }

        }

        public static void Init()
        {
            try
            {
                #region nLog
                szNlogPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "Log");
                if (!Directory.Exists(szNlogPath))
                {
                    Directory.CreateDirectory(szNlogPath);
                }
                szNlogConfigPath = (@".\Util\nLogConfig.xml");
                log = new LogEvents(szNlogPath);
                #endregion nLog


            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}] EX:[{1}]", "MainProcess.Init", ex.Message));
            }
        }

        
    }
}
