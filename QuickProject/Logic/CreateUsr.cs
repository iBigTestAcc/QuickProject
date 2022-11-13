using ConsoleProject.Model;
using QuickProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace QuickProject.Logic
{
    internal class CreateUsr
    {
        internal UserProfile profile { get; set; }

        internal UserPwd pwd { get; set; } // Out of scope


        public CreateUsr()
        {

        }

        // todo. get iBan.
        // verify with DB
        // insert if unique
        public bool Create()
        {
            string szTxt = string.Format("> {0}", "CreateUsr.Create()");
            MainProcess.log.AppendLog(szTxt);
            int loopCount = 0;
            int maxCount = 3;
            bool bNotFoundIban = false;
            try
            {
                string szIban = String.Empty;
                while (!bNotFoundIban && loopCount < maxCount)
                {
                    szIban = RandomIban.GetIban();
                    // check Iban in DB
                    bNotFoundIban = MainProcess.sql.CheckNotHaveIban(szIban);
                    loopCount++;
                }

                if(bNotFoundIban)
                {
                    int countryId =  MainProcess.sql.GetCountryID(szIban.Substring(0, 2));

                    MainProcess.sql.database.BeginTransaction();
                    UserProfile usrPro = new UserProfile
                    {
                        UserIban = szIban,
                        CoutryId = countryId
                    };
                    usrPro = UserProfile.InsertUserProfile(usrPro);

                    // Create balance
                    var balance = new AccBalance
                    {
                        Balance = 0,
                        UserIban = usrPro.UserIban
                    };
                    balance = AccBalance.InsertAccBalance(balance);

                    User usr = new User
                    {
                        ProfileId = usrPro.Id,
                        BalanceId = balance.Id
                    };
                    usr = User.InsertUser(usr);

                    CreateUsrHistory createHis = new CreateUsrHistory
                    {
                         UserId = usr.Id,
                         UserProfileId = usrPro.Id,
                         AccBalanceId = balance.Id,
                         DateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss fff")
                    };
                    createHis = CreateUsrHistory.InsertCreateUsrHistory(createHis);

                    TransactionHistory transHis = new TransactionHistory
                    {
                        DepositId = 0,
                        TransferId = 0,
                        CreateUsrId = createHis.Id,
                        DateTime = createHis.DateTime,
                        Type = TrxTypeEnum.ToDescriptionString(TrxType.CreateUsr)
                    };
                    transHis = TransactionHistory.InsertTransferHistory(transHis);

                    MainProcess.sql.database.Commit();
                    szTxt = string.Format("Insert done UserIban[{0}] ProfileId[{1}] UserId[{2}]", 
                        usrPro.UserIban,
                        usrPro.Id,
                        usr.Id);
                    MainProcess.log.AppendLog(szTxt);
                }
            }
            catch(Exception ex)
            {
                MainProcess.sql.database.Rollback();
                bNotFoundIban = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateUsr.Create()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            szTxt = string.Format("< {0}", "CreateUsr.Create() [{1}]", bNotFoundIban.ToString(), bNotFoundIban);
            MainProcess.log.AppendLog(szTxt);
            return bNotFoundIban;
        }
    }
}
