using ConsoleProject.Model;
using QuickProject.Model;
using QuickProject.Util;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleProject.Model.UserProfile;
using static QuickProject.Util.IOandVer;

namespace QuickProject.Logic
{
    public class DepositProcess
    {
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
                if(feeList.FirstOrDefault() != null)
                {
                    var fee = feeList.FirstOrDefault();
                    double dAmount = double.Parse(szAmount);
                    double feeAmount = fee.FeeAmount;

                    
                    if(fee.Type == FeeEnum.ToDescriptionString(Fee.Percent))
                    {
                        feeAmount = (fee.FeeAmount * dAmount) / 100;
                        // Normally use only 2 decimal digit.
                        // but not in spec
                        //string floor = string.Format("{0:0.00}", feeAmount);
                        //feeAmount = feeAmount;
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
            UserProfile usrProfile = null;
            try
            {
                var verResult = VerIbanAccount(szIban, ref usrProfile, AccAction.DepAcc);
                if (verResult == VerifyResult.OK)
                {
                    if (usrProfile != null)
                    {
                        // ver fee
                        returnObj = new ConfirmDeposit();
                        returnObj.UserProfileId = usrProfile.Id;
                        returnObj.szIban = szIban;
                        returnObj.depositAmount = VerFeeAmount(szAmount);
                    }
                }
                else
                {
                    IOandVer.DisplayVerifyError(verResult);
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
