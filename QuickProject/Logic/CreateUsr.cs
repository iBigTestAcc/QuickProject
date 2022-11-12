using ConsoleProject.Model;
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
            string szTxt = string.Empty;
            MainProcess.log.AppendLog(string.Format("> {0}", "CreateUsr.Create()"));
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

                    UserProfile usrPro = new UserProfile
                    {
                        UserIban = szIban,
                        CoutryId = countryId
                    };
                    usrPro = UserProfile.InsertUserProfile(usrPro);

                    User usr = new User
                    {
                        ProfileId = usrPro.Id
                    };

                    usr = User.InsertUser(usr);

                    szTxt = string.Format("Insert done UserIban[{0}] ProfileId[{1}] UserId[{2}]", 
                        usrPro.UserIban,
                        usrPro.Id,
                        usr.Id);
                }
            }
            catch(Exception ex)
            {
                bNotFoundIban = false;
                szTxt = string.Format("{0}] EX:[{1}]", "CreateUsr.Create()", ex.Message);
                MainProcess.log.AppendLog(szTxt);
            }
            MainProcess.log.AppendLog(string.Format("< {0}", "CreateUsr.Create() [{1}]", bNotFoundIban.ToString()));
            return bNotFoundIban;
        }
    }
}
