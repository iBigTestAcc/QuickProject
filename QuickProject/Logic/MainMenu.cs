using ConsoleProject.Model;
using NLog.Fluent;
using QuickProject.Model;
using QuickProject.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                    Console.WriteLine("\t\t\t220. View all CreateUsrRecord");
                    Console.WriteLine("\t\t\t221. View all DepositRecord");
                    Console.WriteLine("\t\t\t222. View all TransferRecord");
                    Console.WriteLine("\t3. Depoist");
                    Console.WriteLine("\t\t30. View all Acc balance");
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

        internal void SwitchCaseMenu(string szIn)
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

                        var tranHis = MainProcess.sql.database.Query<TransactionHistory>("select * from TransactionHistory");
                        foreach (var item in tranHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] TransferId[{1}] DepositId[[{2}] CreateUsrId[{3}] Type[{4}] DateTime[{5}]",
                                item.Id, item.TransferId, item.DepositId, item.CreateUsrId, item.Type, item.DateTime));
                        }
                        break;

                    case "220":
                        var createusrHis = MainProcess.sql.database.Query<CreateUsrHistory>("select * from CreateUsrHistory");
                        foreach (var item in createusrHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] UserId[{1}] UsrProfileId[[{2}] AccBalanceId[{3}] CreateDateTime[{4}]",
                                item.Id, item.UserId, item.UserProfileId, item.AccBalanceId, item.DateTime));
                        }
                        break;
                    case "221":
                        var depHis = MainProcess.sql.database.Query<DepositHistroy>("select * from DepositHistroy");
                        foreach (var item in depHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] UserIban[{1}] UserProfileId[[{2}] Amount[{3}] DateTime[{4}]",
                                item.Id, item.UserIban, item.UserProfileId, item.Amount, item.DateTime));
                        }
                        break;

                    case "222":
                        var trnsHis = MainProcess.sql.database.Query<TransferHistory>("select * from TransferHistory");
                        foreach (var item in trnsHis)
                        {
                            Console.WriteLine(string.Format("id[{0}] From[{1}] To[[{2}] Amount[{3}] NetAmount[{4}] DateTime[{5}]",
                                item.Id, item.From, item.To, item.Amount, item.NetAmount, item.DateTime));
                        }
                        break;
                    case "3":
                        ProcessDeposit();
                        break;

                    case "30":
                        var acc = MainProcess.sql.database.Query<AccBalance>("select * from AccBalance");
                        foreach (var item in acc)
                        {
                            Console.WriteLine(string.Format("id[{0}] UserIban[{1}] Balance[[{2}]",
                                item.Id, item.UserIban, item.Balance));
                        }
                        break;

                    case "4":
                        // todo
                        // get source acc
                        // get dest acc
                        // get amount
                        // ver source acc, dest, amount ,show balance
                        // confirm dest , amount
                        // else???

                        ProcessTransfer();

                        break;

                    case "999":
                        //var fee = new FeeType
                        //{
                        //    FeeAmount = 0,
                        //    Type = FeeEnum.ToDescriptionString(Fee.Amount)
                        //};
                        //MainProcess.sql.database.Insert(fee);
                        //MainProcess.sql.database.DropTable<TransferHistory>();
                        //MainProcess.sql.database.CreateTable<TransferHistory>();

                        //var tmp = MainProcess.sql.database.Query<TransferHistory>("select * from TransferHistory");

                        //var asdf = 0;

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

        internal void ProcessTransfer()
        {
            Console.Clear();
            bool bInProcess = true;
            string szSourceIban = string.Empty;
            string szDestIban = string.Empty;
            string szAmount = string.Empty;
            string szTxt = string.Empty;

            IOandVer.VerifyResult result = IOandVer.VerifyResult.OK;
            try
            {
                bInProcess = IOandVer.GetIBAN(ref szSourceIban, "Source account");
                if (bInProcess)
                {
                    bInProcess = IOandVer.GetIBAN(ref szDestIban, "Destination account");
                }
                bInProcess = IOandVer.GetAmount(ref szAmount, bInProcess);

                if (bInProcess)
                {
                    var tranferObj = TansferProcess.ValidationTransferData(szSourceIban, szDestIban, szAmount);

                    if(tranferObj != null)
                    {
                        // confirm
                        string szInput = string.Empty;
                        // Display and confirm
                        while (szInput != "YES" && szInput != "NO")
                        {
                            Console.Clear();

                            szTxt = string.Format("Confirm to transer from IBAN [{0}] \n", tranferObj.sourceProfile.UserIban);
                            szTxt += string.Format("To IBAN [{0}] \n", tranferObj.DestProfile.UserIban);
                            szTxt += string.Format("Total Amount [{0}] \n", tranferObj.transAmunt.TotalAmount);
                            szTxt += string.Format("Fee Amount [{0}] \n", tranferObj.transAmunt.FeeAmount);
                            szTxt += string.Format("Net Amount to Account [{0}] \n", tranferObj.transAmunt.NetAmount);

                            Console.WriteLine(szTxt);
                            Console.Write("Enter [YES] for confirm [NO] for cancel:");
                            szInput = Console.ReadLine();
                        }

                        if (szInput == "YES")
                        {
                            // go to confirm
                            Console.WriteLine("OK");
                            result = TansferProcess.TriggerConfirmTransfer(tranferObj);
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

                if(result != IOandVer.VerifyResult.OK)
                {
                    IOandVer.DisplayVerifyError(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}] EX:[{1}]", "ProcessTransfer", ex.Message));
            }
        }

        

        internal void ProcessDeposit()
        {
            Console.Clear();
            bool bInProcess = true;
            string szIban = string.Empty;
            string szAmiunt = string.Empty;
            string szTxt = string.Empty;
            try
            {
                bInProcess = IOandVer.GetIBAN(ref szIban, "Deposit");
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess);

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
