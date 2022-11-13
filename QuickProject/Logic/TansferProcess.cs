using ConsoleProject.Model;
using QuickProject.Model;
using QuickProject.Util;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuickProject.Util.IOandVer;

namespace QuickProject.Logic
{
    public class TansferProcess
    {
        public class TransferConfirm
        {
            public UserProfile sourceProfile { get; set; }
            public UserProfile DestProfile { get; set; }
            public AccBalance sourceBalance { get; set; }
            public AccBalance destBalance { get; set; }
            public TransferAmount transAmunt { get; set; }
        }

        public class TransferAmount
        {
            public double TotalAmount { get; set; }
            public double NetAmount { get; set; }
            public double FeeAmount { get; set; }

            public int FeeId { get; set; }

            public string FeeType { get; set; }
            public TransferAmount()
            {

            }
        }
        public static TransferConfirm ValidationTransferData(string szSourceIban, string szDestIban, string szAmount)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            MainProcess.log.AppendLog(string.Format("> {0}({1}, {2}, {3})", "ValidationTransferData", 
                szSourceIban, szDestIban, szAmount));
            UserProfile sourceProfile = null;
            UserProfile destProfile = null;
            IOandVer.VerifyResult verResult = IOandVer.VerifyResult.OK;
            
            TransferConfirm retunObl = null;
            TransferAmount tranAmount = null;
            try
            {
                if (szSourceIban == szDestIban)
                {
                    verResult = VerifyResult.ToAndSourceAccIsSame;
                }

                // valid source acc,
                if (verResult == IOandVer.VerifyResult.OK)
                {
                    verResult = IOandVer.VerIbanAccount(szSourceIban, ref sourceProfile, AccAction.SourceAcc);
                }

                // valid dest,
                if (verResult == IOandVer.VerifyResult.OK)
                {
                    verResult = IOandVer.VerIbanAccount(szDestIban, ref destProfile, AccAction.DestAcc);
                }

                // valid amount
                if (verResult == IOandVer.VerifyResult.OK)
                {
                    verResult = IOandVer.VerifyAmount(szAmount, TrxType.Transfer);
                    if (verResult == VerifyResult.OK)
                    {
                        // get fee
                        tranAmount = VerFeeAmount(szAmount);
                        if (tranAmount == null)
                        {
                            verResult = VerifyResult.InternalFeeError;
                        }
                    }
                }

                if(verResult == IOandVer.VerifyResult.OK)
                {
                    // confirm dest , amount
                    retunObl = new TransferConfirm();
                    retunObl.sourceProfile = sourceProfile;
                    retunObl.DestProfile = destProfile;
                    retunObl.transAmunt = tranAmount;
                    bDone = true;
                }
                else
                {
                    // else???
                    IOandVer.DisplayVerifyError(verResult);
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "ValidationTransferData", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1})]", "ValidationTransferData", bDone.ToString()));

            return retunObl;
        }

        public static TransferAmount VerFeeAmount(string szAmount)
        {
            string szTxt = string.Empty;
            bool bDone = false;
            MainProcess.log.AppendLog(string.Format("> {0}({1})", "TansferProcess.VerFeeAmount", szAmount));
            TransferAmount returnObj = null;
            try
            {
                // verify Fee in user.
                var feeList = FeeType.GetFee(TrxType.Transfer);
                if (feeList.FirstOrDefault() != null)
                {
                    var fee = feeList.FirstOrDefault();
                    double dAmount = double.Parse(szAmount);
                    double feeAmount = fee.FeeAmount;

                    if (fee.Type == FeeEnum.ToDescriptionString(Fee.Percent))
                    {
                        feeAmount = (fee.FeeAmount * dAmount) / 100;
                        string floor = string.Format("{0:0.00}", feeAmount);
                        feeAmount = double.Parse(floor);
                    }

                    returnObj = new TransferAmount();
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
                szTxt = string.Format("{0}] EX:[{1}]", "TansferProcess.VerFeeAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            MainProcess.log.AppendLog(string.Format("< {0}({1}) [{2}]", "TansferProcess.VerFeeAmount", returnObj.FeeAmount, bDone.ToString()));

            return returnObj;
        }

        public static VerifyResult CheckAmountWithBalance(TransferConfirm confirmObj, AccBalance balance)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "TransferConfirm.CheckAmountWithBalance()"));
            VerifyResult verReturn = VerifyResult.OK;
            bool bDoune = false;
            try
            {
                if(confirmObj.sourceProfile.UserIban == balance.UserIban)
                {
                    if(balance.Balance < confirmObj.transAmunt.TotalAmount)
                    {
                        verReturn = VerifyResult.NotEnoughBalance;
                    }
                }
                else
                {
                    verReturn = VerifyResult.Error;
                }
            }
            catch(Exception ex)
            {
                MainProcess.sql.database.Rollback();
                bDoune = false;
                szTxt = string.Format("{0}] EX:[{1}]", "TransferConfirm.CheckAmountWithBalance()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0}", "TransferConfirm.CheckAmountWithBalance() [{1}]", bDoune.ToString()));

            return verReturn;
        }
        public static VerifyResult SaveTransactionRecord(TransferConfirm confirmObj)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "TransferConfirm.SaveTransactionRecord(From[{0}] To[{1}] TotalAmount[{2}] Net[{3}])",
                confirmObj.sourceProfile.UserIban,
                confirmObj.DestProfile.UserIban,
                confirmObj.transAmunt.TotalAmount,
                confirmObj.transAmunt.NetAmount));
            bool bNotFoundIban = false;
            VerifyResult verReturn = VerifyResult.OK;

            try
            {
                MainProcess.sql.database.BeginTransaction();

                var minusSourceBalance = confirmObj.sourceBalance.Balance - confirmObj.transAmunt.TotalAmount;
                confirmObj.sourceBalance.Balance = minusSourceBalance;
                MainProcess.sql.database.UpdateWithChildren(confirmObj.sourceBalance);

                var pulsDestBalance = confirmObj.destBalance.Balance + confirmObj.transAmunt.NetAmount;
                confirmObj.destBalance.Balance = pulsDestBalance;
                MainProcess.sql.database.UpdateWithChildren(confirmObj.destBalance);

                TransferHistory transferHis = new TransferHistory
                {
                    CurrencyId = 1, // HARD CODE for euro
                    Amount = confirmObj.transAmunt.TotalAmount,
                    NetAmount = confirmObj.transAmunt.NetAmount,
                    FeeId = confirmObj.transAmunt.FeeId,
                    From = confirmObj.sourceProfile.Id,
                    To = confirmObj.destBalance.Id,
                    DestIban = confirmObj.DestProfile.UserIban,
                    SourceIban = confirmObj.sourceProfile.UserIban,
                    DateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss fff")
                };
                MainProcess.sql.database.InsertWithChildren(transferHis);

                TransactionHistory transHis = new TransactionHistory
                {
                    DepositId = 0,
                    TransferId = transferHis.Id,
                    CreateUsrId = 0,
                    DateTime = transferHis.DateTime,
                    Type = TrxTypeEnum.ToDescriptionString(TrxType.Transfer)
                };
                MainProcess.sql.database.InsertWithChildren(transHis);
                MainProcess.sql.database.Commit();
            }
            catch(Exception ex)
            {
                verReturn = VerifyResult.Error;
                MainProcess.sql.database.Rollback();
                bNotFoundIban = false;
                szTxt = string.Format("{0}] EX:[{1}]", "TransferConfirm.SaveTransactionRecord()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0}", "TransferConfirm.SaveTransactionRecord() [{1}]", bNotFoundIban.ToString()));
            return verReturn;
        }
        public static VerifyResult TriggerConfirmTransfer(TransferConfirm confirmObj)
        {
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "TransferConfirm.TriggerConfirmTransfer()"));
            bool bNotFoundIban = false;
            VerifyResult verReturn = VerifyResult.OK;
            try
            {
                string sql = string.Format("select * from AccBalance where (UserIban = '{0}') or (UserIban = '{1}')", 
                    confirmObj.sourceProfile.UserIban,
                    confirmObj.DestProfile.UserIban);

                var existingBalance = MainProcess.sql.database.Query<AccBalance>(sql);
                
                var sourceBalance = (from queryResult in existingBalance
                                   where queryResult.UserIban == confirmObj.sourceProfile.UserIban
                                   select queryResult).FirstOrDefault();

                var destBalance = (from queryResult in existingBalance
                                    where queryResult.UserIban == confirmObj.DestProfile.UserIban
                                    select queryResult).FirstOrDefault();

                if (sourceBalance != null && destBalance != null)
                {
                    verReturn = CheckAmountWithBalance(confirmObj, sourceBalance);

                    if (verReturn == VerifyResult.OK)
                    {
                        confirmObj.destBalance = destBalance;
                        confirmObj.sourceBalance = sourceBalance;

                        verReturn = SaveTransactionRecord(confirmObj);
                    }
                }
                else
                {
                    verReturn = VerifyResult.Error;
                }

            }
            catch (Exception ex)
            {
                MainProcess.sql.database.Rollback();
                bNotFoundIban = false;
                szTxt = string.Format("{0}] EX:[{1}]", "TransferConfirm.TransferConfirm()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0}", "TransferConfirm.TransferConfirm() [{1}]", bNotFoundIban.ToString()));
            return verReturn;
        }
    }
}
