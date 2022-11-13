using ConsoleProject.Model;
using NLog.Fluent;
using QuickProject.Model;
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
                    Console.WriteLine("\t\t22. View all History");
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

                    case "22":

                        var acc = MainProcess.sql.database.Query<AccBalance>("select * from AccBalance");
                        foreach (var item in acc)
                        {
                            Console.WriteLine(string.Format("id[{0}] UserIban[{1}] Balance[[{2}]",
                                item.Id, item.UserIban, item.Balance));
                        }

                        var usrHis = MainProcess.sql.database.Query<CreateUsrHistory>("select * from CreateUsrHistory");
                        foreach(var item in usrHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] UserId[{1}] UsrProfileId[[{2}] AccBalanceId[{3}] CreateDateTime[{4}]", 
                                item.Id, item.UserId, item.UserProfileId, item.AccBalanceId, item.DateTime));
                        }

                        var tranHis = MainProcess.sql.database.Query<TransactionHistory>("select * from TransactionHistory");
                        foreach (var item in tranHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] TransferId[{1}] DepositId[[{2}] CreateUsrId[{3}] Type[{4}] DateTime[{5}]",
                                item.Id, item.TransferId, item.DepositId, item.CreateUsrId, item.Type, item.DateTime));
                        }
                        break;

                    case "3":
                        ProcessDeposit();
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

        public bool GetIBAN(ref string szIban)
        {
            bool bRet = true;
            bool iBanOk = false;
            while (!iBanOk && bRet)
            {
                szIban = DepositProcess.ReceiveInput();
                if (szIban == "X")
                {
                    bRet = false;
                }
                else
                {
                    var verResult = DepositProcess.VerifyIban(szIban);
                    if (verResult == DepositProcess.VerifyResult.OK)
                    {
                        iBanOk = true;
                    }
                    else
                    {
                        DepositProcess.DisplayVerifyError(verResult);
                    }
                }
            }

            return bRet;
        }

        public bool GetDepAmount(ref string szAmount, bool bInProcess)
        {
            bool iAmountOk = false;
            while (bInProcess && !iAmountOk)
            {
                szAmount = DepositProcess.ReceiveAmount();
                if(szAmount == "X")
                {
                    bInProcess = false;
                }
                else
                {
                    var verResult = DepositProcess.VerifyAmount(szAmount);
                    if(verResult == DepositProcess.VerifyResult.OK)
                    {
                        iAmountOk = true;
                    }
                }
            }
            return iAmountOk;
        }

        public void ProcessDeposit()
        {
            Console.Clear();
            bool bInProcess = true;
            string szIban = string.Empty;
            string szAmiunt = string.Empty;
            string szTxt = string.Empty;
            try
            {
                bInProcess = GetIBAN(ref szIban);
                bInProcess = GetDepAmount(ref szAmiunt, bInProcess);

                if (bInProcess)
                {
                    var confirmObj = DepositProcess.VerifyDepositData(szIban, szAmiunt);

                    if(confirmObj != null)
                    {
                        string szInput = string.Empty;
                        // Display and confirm
                        while (szInput != "YES" && szInput != "NO")
                        {
                            Console.Clear();

                            szTxt = string.Format("Confirm to deposit to IBAN [{0}] \n", confirmObj.szIban);
                            szTxt += string.Format("Total Amount [{0}] \n", confirmObj.depositAmount.TotalAmount);
                            szTxt += string.Format("Fee Amount [{0}] \n", confirmObj.depositAmount.FeeAmount);
                            szTxt += string.Format("Net Amount to Account [{0}] \n", confirmObj.depositAmount.NetAmount);
                        
                            Console.WriteLine(szTxt);
                            Console.Write("Enter [YES] for confirm [NO] for cancel:");
                            szInput = Console.ReadLine();
                        }

                        if(szInput == "YES")
                        {
                            // go to confirm
                            Console.WriteLine("OK");
                            ConfirmDeposit.TriggerConfirmDeposit(confirmObj);
                        }
                        else
                        {
                            Console.WriteLine("Cancel");
                        }
                        
                    }
                }
                else
                {
                    // Someting wrong
                    //Console.WriteLine(string.Format("Someting wrong"));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("{0}] EX:[{1}]", "ProcessDeposit", ex.Message));
            }
        }
    }
}
