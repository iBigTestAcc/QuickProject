using ConsoleProject.Model;
using QuickProject.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleProject.Model.UserProfile;

namespace QuickProject.Logic
{
    public class DepositProcess
    {
        public enum VerifyResult
        {
            OK,
            WrongLength,
            WrongFormat,
            ToAccountNotFound,
            Else,
            Error
        }

        public static string ReceiveAmount()
        {
            string szTxt = string.Empty;
            string szInAmount = string.Empty;
            try
            {
                Console.Write("Enter Amount, [X] for exit:");
                szInAmount = Console.ReadLine();
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "ReceiveAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            return szInAmount;
        }
        public static string ReceiveInput()
        {
            string szTxt = string.Empty;
            string szInBan = string.Empty;
            try
            {
                Console.Write("Enter IBAN [X] for exit:");
                szInBan = Console.ReadLine();
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "ReceiveInput", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            return szInBan;
        }

        public static VerifyResult VerifyIban(string szIban)
        {
            VerifyResult bReturn = VerifyResult.Error;
            string szTxt = string.Empty;
            try
            {
                // length.
                // else ?
                if(szIban.Length != 18)
                {
                    bReturn = VerifyResult.WrongLength;
                }
                else
                {
                    bReturn = VerifyResult.OK;
                }
            }
            catch(Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerifyIban", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        public static VerifyResult VerifyAmount(string szAmount)
        {
            VerifyResult bReturn = VerifyResult.Error;
            string szTxt = string.Empty;
            try
            {
                // is it decimal.
                // else ?
                double dResult = 0.0;
                
                if (double.TryParse(szAmount, out dResult))
                {
                    bReturn = VerifyResult.OK;
                }
                else
                {
                    bReturn = VerifyResult.WrongFormat;
                    DisplayVerifyError(bReturn);
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerifyAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        public static void DisplayVerifyError(VerifyResult result)
        {
            string szTxt = string.Empty;
            switch(result)
            {
                case VerifyResult.WrongLength:
                    szTxt = string.Format("Wrong IBAN format. Please re-enter.");
                    Console.WriteLine(szTxt);
                    break;

                case VerifyResult.WrongFormat:
                    szTxt = string.Format("Wrong Amount format. Please re-enter.");
                    Console.WriteLine(szTxt);
                    break;

                case VerifyResult.ToAccountNotFound:
                    szTxt = string.Format("Enter IBAN is not exists in system.");
                    Console.WriteLine(szTxt);
                    break;
                default:

                    szTxt = string.Format("Unexpect error");
                    Console.WriteLine(szTxt);
                    break;
            }
        }
        public static UserProfile VerIbanAccount(string szIban)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "VerIbanAccount", szIban));
            VerifyResult bVerResult = VerifyResult.OK;
            UserProfile bReturnObj = null;

            try
            {
                // verify IBAN in user.
                var usrProfileList = UserProfile.GetUsrProfile(szIban);
                var usrProfile = UserProfile.VerifyUsrProfile(usrProfileList);

                if (usrProfile != null)
                {
                    bReturnObj = usrProfile;
                    bDone = true;
                }
                else
                {
                    bVerResult = VerifyResult.ToAccountNotFound;
                    DisplayVerifyError(bVerResult);
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerIbanAccount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "VerIbanAccount", szIban, bDone.ToString()));

            return bReturnObj;
        }

        public static DepositAmount VerFeeAmount(string szAmount)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "VerFeeAmount", szAmount));
            DepositAmount returnObj = null;
            try
            {
                // verify Fee in user.
                var feeList = FeeType.GetFee(TrxType.Deposit);
                if(feeList != null)
                {
                    var fee = feeList.FirstOrDefault();
                    double dAmount = double.Parse(szAmount);
                    double feeAmount = fee.FeeAmount;

                    if(fee.Type == FeeEnum.ToDescriptionString(Fee.Percent))
                    {
                        feeAmount = (fee.FeeAmount * dAmount) / 100;
                        string floor = string.Format("{0:0.00}", feeAmount);
                        feeAmount = double.Parse(floor);
                    }

                    returnObj = new DepositAmount();
                    returnObj.FeeAmount = feeAmount;
                    returnObj.TotalAmount = dAmount;
                    returnObj.NetAmount = (dAmount - feeAmount);
                    returnObj.FeeId = fee.Id;
                    returnObj.FeeType = fee.Type;
                }
                else
                {
                    szTxt = String.Format("Cannot found fee amount Exception:{0}", FeeType.ExceptionList.FeeNotFound.ToString());
                    throw new Exception(szTxt);
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerFeeAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "VerFeeAmount", returnObj.FeeAmount, bDone.ToString()));

            return returnObj;
        }
        public static ConfirmDeposit VerifyDepositData(string szIban, string szAmount)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            MainProcess.log.AppendLog(string.Format("> {0}({1},{2})", "VerifyDepositData", szIban, szAmount));
            ConfirmDeposit returnObj = null;
            try
            {
                var usrProfile = VerIbanAccount(szIban);
                if (usrProfile != null)
                {
                    // ver fee
                    returnObj = new ConfirmDeposit();
                    returnObj.UserProfileId = usrProfile.Id;
                    returnObj.szIban = szIban;
                    returnObj.depositAmount = VerFeeAmount(szAmount);
                }
            }
            catch(Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerifyDepositData", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            if (returnObj != null)
            {
                MainProcess.log.AppendLog(string.Format("< {0}({1},{2}) [{3}]", "VerifyDepositData", 
                    returnObj.szIban, returnObj.depositAmount.NetAmount, bDone.ToString()));
            }
            else
            {
                MainProcess.log.AppendLog(string.Format("< {0}({1},{2}) [{3}]", "VerifyDepositData", "NULL", "NULL", bDone.ToString()));
            }

            return returnObj;
        }
    }

    public class ConfirmDeposit
    {
        public string szIban { get; set; }
        
        public int UserProfileId { get; set; }
        public DepositAmount depositAmount { get; set; }

        
        public ConfirmDeposit() { }

        public static void TriggerConfirmDeposit(ConfirmDeposit confirmObj)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "ConfirmDeposit.TriggerConfirmDeposit()"));
            bool bNotFoundIban = false;
            try
            {

                MainProcess.sql.database.BeginTransaction();

                string sql = string.Format("select * from AccBalance where UserIban = '{0}'", confirmObj.szIban);
                var existingBalance = MainProcess.sql.database.Query<AccBalance>(sql);
                if (existingBalance != null)
                {
                    var obj = existingBalance.FirstOrDefault();
                    if (obj != null)
                    {
                        AccBalance balance = new AccBalance
                        {
                            UserIban = obj.UserIban,
                            Id = obj.Id,
                            Balance = obj.Balance + confirmObj.depositAmount.NetAmount
                        };
                        MainProcess.sql.database.UpdateWithChildren(balance);

                        DepositHistroy depHis = new DepositHistroy
                        {
                            Amount = confirmObj.depositAmount.TotalAmount,
                            CurrencyId = 1, // HARD CODE for euro
                            NetAmount = confirmObj.depositAmount.NetAmount,
                            FeeId = confirmObj.depositAmount.FeeId,
                            UserIban = confirmObj.szIban,
                            UserProfileId = confirmObj.UserProfileId,
                            DateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss fff")
                        };
                        MainProcess.sql.database.InsertWithChildren(depHis);

                        TransactionHistory transHis = new TransactionHistory
                        {
                            DepositId = depHis.Id,
                            TransferId = 0,
                            CreateUsrId = 0,
                            DateTime = depHis.DateTime,
                            Type = TrxTypeEnum.ToDescriptionString(TrxType.Deposit)
                        };
                        MainProcess.sql.database.InsertWithChildren(transHis);
                    }
                }

                MainProcess.sql.database.Commit();
            }
            catch (Exception ex)
            {
                MainProcess.sql.database.Rollback();
                bNotFoundIban = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateUsr.Create()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0}", "CreateUsr.Create() [{1}]", bNotFoundIban.ToString()));
        }
    }
    public class DepositAmount
    {
        public double TotalAmount { get; set; }
        public double NetAmount { get; set; }

        public double FeeAmount { get; set; }

        public int FeeId { get; set; }

        public string FeeType { get; set; }
        public DepositAmount()
        {

        }
    }
}
