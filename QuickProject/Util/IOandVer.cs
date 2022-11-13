using ConsoleProject.Model;
using QuickProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuickProject.Logic.DepositProcess;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static QuickProject.Util.IOandVer;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;

namespace QuickProject.Util
{
    public static class VerifyResultEnum
    {
        public static string ToDescriptionString(this VerifyResult val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
    public class IOandVer
    {
        public enum AccAction
        {
            [DescriptionAttribute("DepAcc")]
            DepAcc,
            [DescriptionAttribute("SourceAcc")]
            SourceAcc,
            [DescriptionAttribute("DestAcc")]
            DestAcc
        }

        public enum VerifyResult
        {
            [DescriptionAttribute("OK.")]
            OK,
            [DescriptionAttribute("Wroung Data Length.")]
            WrongLength,
            [DescriptionAttribute("Incorrect Length or Format. Support only for 'NL'")]
            WrongFormat,
            [DescriptionAttribute("Destination account not found.")]
            ToAccountNotFound,
            [DescriptionAttribute("Source account not found.")]
            SourceAccountNotFound,
            [DescriptionAttribute("Source account and Destination account canot same.")]
            ToAndSourceAccIsSame,
            [DescriptionAttribute("Internal Error. Fee not found.")]
            InternalFeeError,
            [DescriptionAttribute("Current account balance is not enough for transfer.")]
            NotEnoughBalance,
            [DescriptionAttribute("Wrong deposit amount. Support maximun[{0}] minimum[{1}] with 2 point decimal digit")]
            WrongDepositAmount,
            [DescriptionAttribute("Wrong Tranfer amount. Support maximun[{0}] minimum[{1}] with 2 point decimal digit")]
            WrongTranferAmount,
            [DescriptionAttribute("Wrong amount enter. Support 2 point decimal digit")]
            WrongAmount,
            [DescriptionAttribute("Internal Error.")]
            Else,
            [DescriptionAttribute("Internal Error.")]
            Error
        }


        public static bool GetIBAN(ref string szIban, string szAction)
        {
            bool bRet = true;
            bool iBanOk = false;
            while (!iBanOk && bRet)
            {
                szIban = string.Empty;
                szIban = IOandVer.GetInput(szAction);
                if (szIban == "X")
                {
                    bRet = false;
                }
                else
                {
                    var verResult = IOandVer.VerifyIban(szIban);
                    if (verResult == IOandVer.VerifyResult.OK)
                    {
                        iBanOk = true;
                    }
                    else
                    {
                        IOandVer.DisplayVerifyError(verResult);
                    }
                }
            }

            return bRet;
        }

        public static bool GetAmount(ref string szAmount, bool bInProcess, TrxType trxType)
        {
            bool iAmountOk = false;
            while (bInProcess && !iAmountOk)
            {
                szAmount = IOandVer.GetAmount();
                if (szAmount == "X")
                {
                    bInProcess = false;
                }
                else
                {
                    var verResult = IOandVer.VerifyAmount(szAmount, trxType);
                    if (verResult == IOandVer.VerifyResult.OK)
                    {
                        iAmountOk = true;
                    }
                    else
                    {
                        IOandVer.DisplayVerifyError(verResult);
                        bInProcess = false;
                    }

                }
            }
            return iAmountOk;
        }
        public static string GetInput(string szAction)
        {
            string szTxt = string.Empty;
            string szInBan = string.Empty;
            try
            {
                szTxt = string.Format("Enter {0} IBAN [X] for exit:", szAction);
                Console.Write(szTxt);
                szInBan = Console.ReadLine();
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetInput", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            return szInBan;
        }

        public static string GetAmount2Digit()
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
                szTxt = string.Format("{0}] EX:[{1}]", "GetAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            return szInAmount;
        }
        public static string GetAmount()
        {
            string szTxt = string.Empty;
            string szInAmount = string.Empty;
            try
            {
                Console.Write("Enter Amount, [X] for exit:");
                szInAmount = Console.ReadLine();
                if (!string.IsNullOrEmpty(szInAmount))
                {
                    decimal val = 0;
                    if (decimal.TryParse(szInAmount, out val))
                    {
                        szInAmount = val.ToString("0.00");
                    }
                }
                else
                {
                    szInAmount = "0.00";
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "GetAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            return szInAmount;
        }

        public static VerifyResult VerifyIban(string szIban)
        {
            VerifyResult bReturn = VerifyResult.Error;
            string szTxt = string.Empty;
            try
            {
                // length.
                // else ?
                if ((!string.IsNullOrEmpty(szIban)) && (szIban.StartsWith("NL") && (szIban.Length == 18)))
                {
                    bReturn = VerifyResult.OK;
                }
                else
                {
                    bReturn = VerifyResult.WrongFormat;
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerifyIban", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        public static VerifyResult VerifyAmount(string szAmount, TrxType trxType)
        {
            VerifyResult bReturn = VerifyResult.Error;
            string szTxt = string.Empty;
            try
            {
                // is it decimal.
                // else ?
                double dResult = 0.0;

                var regex = new Regex(@"^\d+\.\d{2}?$"); // ^\d+(\.|\,)\d{2}?$ use this incase your dec separator can be comma or decimal.
                var flg = regex.IsMatch(szAmount);
                if (flg)
                {
                    if (double.TryParse(szAmount, out dResult))
                    {
                        if (trxType == TrxType.Deposit)
                        {
                            // check min max for dep amount
                            if (dResult > Properties.Settings.Default.dMaxDepAmount || dResult < Properties.Settings.Default.dMinDepAmount)
                            {
                                bReturn = VerifyResult.WrongDepositAmount;
                            }
                            else
                            {
                                bReturn = VerifyResult.OK;
                            }
                        }
                        else
                        {
                            // check min for trans
                            if (dResult > Properties.Settings.Default.dMaxTransferAmount || dResult < Properties.Settings.Default.dMinTransferAmount)
                            {
                                bReturn = VerifyResult.WrongTranferAmount;
                            }
                            else
                            {
                                bReturn = VerifyResult.OK;
                            }
                        }
                    }
                    else
                    {
                        bReturn = VerifyResult.WrongAmount;
                    }
                }
                else
                {
                    bReturn = VerifyResult.WrongAmount;
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerifyAmount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }

            return bReturn;
        }

        public static VerifyResult VerIbanAccount(string szIban, ref UserProfile usrProfile, AccAction accAction)
        {
            string szTxt = string.Format("> {0}({1})", "VerIbanAccount", szIban);
            bool bDone = false;
            MainProcess.log.AppendLog(szTxt);
            IOandVer.VerifyResult bVerResult = IOandVer.VerifyResult.OK;
            UserProfile bReturnObj = null;

            try
            {
                // verify IBAN in user.
                var usrProfileList = UserProfile.GetUsrProfile(szIban);
                usrProfile = UserProfile.VerifyUsrProfile(usrProfileList);

                if (usrProfile != null)
                {
                    bReturnObj = usrProfile;
                    bDone = true;
                }
                else
                {
                    switch(accAction)
                    {
                        case AccAction.SourceAcc:
                            bVerResult = IOandVer.VerifyResult.SourceAccountNotFound;
                            break;
                        case AccAction.DepAcc:
                            bVerResult = IOandVer.VerifyResult.ToAccountNotFound;
                            break;
                        case AccAction.DestAcc:
                            bVerResult = IOandVer.VerifyResult.ToAccountNotFound;
                            break;
                        default:
                            bVerResult = IOandVer.VerifyResult.Error;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                szTxt = string.Format("{0}] EX:[{1}]", "VerIbanAccount", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            szTxt = string.Format("< {0}({1}) [{2}]", "VerIbanAccount", szIban, bDone.ToString());
            MainProcess.log.AppendLog(szTxt);

            return bVerResult;
        }

        public static void DisplayVerifyError(VerifyResult result)
        {
            string szTxt = string.Empty;


            switch(result)
            {
                case VerifyResult.WrongDepositAmount:
                    szTxt = String.Format(VerifyResultEnum.ToDescriptionString(result), Properties.Settings.Default.dMaxDepAmount, Properties.Settings.Default.dMinDepAmount);
                break;

                case VerifyResult.WrongTranferAmount:
                    szTxt = String.Format(VerifyResultEnum.ToDescriptionString(result), Properties.Settings.Default.dMaxTransferAmount, Properties.Settings.Default.dMinTransferAmount);
                    break;

                default:
                    szTxt = string.Format(VerifyResultEnum.ToDescriptionString(result));
                break;
            }
            MainProcess.log.AppendLog(string.Format(szTxt), true);

        }
    }
}
