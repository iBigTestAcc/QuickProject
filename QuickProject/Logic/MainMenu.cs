using ConsoleProject.Model;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickProject.Logic
{
    internal class MainMenu
    {
        LogEvents log = null;
        public MainMenu()
        {
            log = MainProcess.log;
        }

        public void StartMenu()
        {
            try
            {
                string szIn = string.Empty;

                while (szIn != "X")
                {
                    Console.WriteLine("Backend");
                    Console.WriteLine("\t1. Create token --> if have time will move all this logic to web API with token authen");
                    Console.WriteLine("\t2. Create Account");
                    Console.WriteLine("\t\t20. View all User");
                    Console.WriteLine("\t\t21. View all Profile");
                    Console.WriteLine("\t3. Depoist");
                    Console.WriteLine("\t4. Transfer");
                    Console.WriteLine("\tX. Exit");

                    Console.Write("Enter number:");
                    szIn = Console.ReadLine();

                    SwitchCaseMenu(szIn);

                    //Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}] EX:[{1}]", "StartMenu", ex.Message));
            }
        }

        public void SwitchCaseMenu(string szIn)
        {
            string szTxt = string.Empty;
            try
            {
                szTxt = string.Format(">SwitchCaseMenu({0})", szIn);
                log.AppendLog(szTxt);
                switch (szIn)
                {
                    case "2":
                        CreateUsr usr = new CreateUsr();
                        usr.Create();
                        
                        break;
                    case "20":
                        User.DisplayAllUser();

                        break;
                    case "21":
                        UserProfile.DisplayAllUserProfile();

                        break;
                    default:
                        // unknow. do nothing
                        break;
                }
                szTxt = string.Format("< SwitchCaseMenu({0})", szIn);
                log.AppendLog(szTxt);
            }
            catch (Exception ex)
            {

                Console.WriteLine(string.Format("{0}] EX:[{1}]", "MainProcess", ex.Message));
            }
        }
    }
}
