using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuickProject;
using QuickProject.Util;
using static QuickProject.Util.IOandVer;
using QuickProject.Model;
using System.IO;
using static SQLite.SQLite3;
using QuickProject.Logic;

namespace QuickTest_Input
{
    [TestClass]
    public class TestInput_InputIBAN
    {
        [TestMethod]
        public void TestInputCaseEmpty()
        {
            string szInput = "";
            Console.SetIn(new System.IO.StringReader(szInput));

            string szIban = Console.ReadLine();
            var verifyResult = IOandVer.VerifyIban(szIban);
            Assert.AreEqual(VerifyResult.WrongFormat, verifyResult);
        }

        [TestMethod]
        public void TestInputCaseShort()
        {
            string szInput = "NL78RABO869486680";
            Console.SetIn(new System.IO.StringReader(szInput));

            string szIban = Console.ReadLine();
            var verifyResult = IOandVer.VerifyIban(szIban);
            Assert.AreEqual(VerifyResult.WrongFormat, verifyResult);
        }

        [TestMethod]
        public void TestInputCaseLong()
        {
            string szInput = "NL78RABO86948668011";
            Console.SetIn(new System.IO.StringReader(szInput));

            string szIban = Console.ReadLine();
            var verifyResult = IOandVer.VerifyIban(szIban);
            Assert.AreEqual(VerifyResult.WrongFormat, verifyResult);
        }

        [TestMethod]
        public void TestInputCaseWrongFormat_1()
        {
            string szInput = "TH78RABO86948668011";
            Console.SetIn(new System.IO.StringReader(szInput));

            string szIban = Console.ReadLine();
            var verifyResult = IOandVer.VerifyIban(szIban);
            Assert.AreEqual(VerifyResult.WrongFormat, verifyResult);
        }

        [TestMethod]
        public void TestInputCaseWrongFormat_2()
        {
            string szInput = "asfasdfasdfasdfasf";
            Console.SetIn(new System.IO.StringReader(szInput));

            string szIban = Console.ReadLine();
            var verifyResult = IOandVer.VerifyIban(szIban);
            Assert.AreEqual(VerifyResult.WrongFormat, verifyResult);
        }
    }

    [TestClass]
    public class TestInput_VerifyAmount
    {
        [TestMethod]
        public void TestInput_Case1()
        {
            string szInput = "";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);
            Assert.AreEqual(VerifyResult.WrongAmount, processResult);
        }

        [TestMethod]
        public void TestInput_Case2()
        {
            string szInput = "10";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.WrongAmount, processResult);
        }

        [TestMethod]
        public void TestInput_Case3()
        {
            string szInput = "20.2";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.WrongAmount, processResult);
        }

        [TestMethod]
        public void TestInput_Case4()
        {
            string szInput = "30.30";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.OK, processResult);
        }

        [TestMethod]
        public void TestInput_Case5()
        {
            string szInput = "1";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.WrongAmount, processResult);
        }

        [TestMethod]
        public void TestInput_Case6()
        {
            string szInput = "9.99";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.WrongDepositAmount, processResult);
        }

        [TestMethod]
        public void TestInput_Case7()
        {
            string szInput = "9999999.99";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.OK, processResult);
        }

        [TestMethod]
        public void TestInput_Case8()
        {
            string szInput = "19999999.99";
            var processResult = IOandVer.VerifyAmount(szInput, QuickProject.Model.TrxType.Deposit);

            Assert.AreEqual(VerifyResult.WrongDepositAmount, processResult);
        }
    }

    [TestClass]
    public class TestInput_InputAmountDeposit
    {
        public TestInput_InputAmountDeposit()
        {
            MainProcess.Init();
            MainProcess.sql = new SqlHandle();
        }

        [TestMethod]
        public void TestInputAmount_Empty()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongDepositAmount),
                    MainProcess.dMaxDepAmount,
                    MainProcess.dMinDepAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.X";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "XX.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.0X";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_4()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "asfasdf!#@!#@#";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_MoreThanMax()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "99999999";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongDepositAmount),
                   MainProcess.dMaxDepAmount,
                   MainProcess.dMinDepAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_MoreThanMax_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10000000.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongDepositAmount),
                   MainProcess.dMaxDepAmount,
                   MainProcess.dMinDepAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "9999999.99";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_LessThanMin_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "9.99";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongDepositAmount),
                   MainProcess.dMaxDepAmount,
                   MainProcess.dMinDepAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }

        }
        [TestMethod]
        public void TestInputAmount_LessThanMin_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "0.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Deposit);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongDepositAmount),
                   MainProcess.dMaxDepAmount,
                   MainProcess.dMinDepAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_FeeGood_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmount = "100.00";
                double dExpectFeeAmount = 0.1;
                double dExpectNetAmount = 99.9;
                var returnObj = DepositProcess.VerFeeAmount(szAmount);

                Assert.IsNotNull(returnObj);
                Assert.AreEqual(FeeEnum.ToDescriptionString(Fee.Percent), returnObj.FeeType);
                Assert.AreEqual(dExpectFeeAmount, returnObj.FeeAmount);
                Assert.AreEqual(dExpectNetAmount, returnObj.NetAmount);
            }
        }

        [TestMethod]
        public void TestInputAmount_FeeGood_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmount = "1000.00";
                double dExpectFeeAmount = 1;
                double dExpectNetAmount = 999;
                var returnObj = DepositProcess.VerFeeAmount(szAmount);

                Assert.IsNotNull(returnObj);
                Assert.AreEqual(FeeEnum.ToDescriptionString(Fee.Percent), returnObj.FeeType);
                Assert.AreEqual(dExpectFeeAmount, returnObj.FeeAmount);
                Assert.AreEqual(dExpectNetAmount, returnObj.NetAmount);
            }
        }
    }

    [TestClass]
    public class TestInput_InputAmountTransfer
    {
        public TestInput_InputAmountTransfer()
        {
            MainProcess.Init();
            MainProcess.sql = new SqlHandle();
        }

        [TestMethod]
        public void TestInputAmount_Empty()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                    MainProcess.dMaxTransAmount,
                    MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.X";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "XX.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.0X";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_WrongAmount_4()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "asfasdf!#@!#@#";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongAmount));
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_MoreThanMax()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "99999999";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                   MainProcess.dMaxTransAmount,
                   MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_MoreThanMax_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10000000.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                   MainProcess.dMaxTransAmount,
                   MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "9999999.99";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "10.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_4()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "00.01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_Good_5()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = ".01";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsTrue(bInProcess);
            }
        }

        [TestMethod]
        public void TestInputAmount_LessThanMin_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "0.001";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                   MainProcess.dMaxTransAmount,
                   MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }

        }

        [TestMethod]
        public void TestInputAmount_LessThanMin_2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = "0.000001";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                   MainProcess.dMaxTransAmount,
                   MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_LessThanMin_3()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmiunt = string.Empty;
                bool bInProcess = true;

                string szInput = ".000001";
                Console.SetIn(new System.IO.StringReader(szInput));
                bInProcess = IOandVer.GetAmount(ref szAmiunt, bInProcess, TrxType.Transfer);

                Assert.IsFalse(bInProcess);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.WrongTranferAmount),
                   MainProcess.dMaxTransAmount,
                   MainProcess.dMinTransAmount);
                string expected = string.Format("Enter Amount, [X] for exit:{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_FeeGood_1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szAmount = "100.00";
                double dExpectFeeAmount = 0.0;
                double dExpectNetAmount = 100.00;
                var returnObj = TansferProcess.VerFeeAmount(szAmount);

                Assert.IsNotNull(returnObj);
                Assert.AreEqual(FeeEnum.ToDescriptionString(Fee.Amount), returnObj.FeeType);
                Assert.AreEqual(dExpectFeeAmount, returnObj.FeeAmount);
                Assert.AreEqual(dExpectNetAmount, returnObj.NetAmount);
            }
        }
    }


    [TestClass]
    public class TestInput_InputIbanTransfer
    {
        public TestInput_InputIbanTransfer()
        {
            MainProcess.Init();
            MainProcess.sql = new SqlHandle();
        }

        [TestMethod]
        public void TestInputAmount_SameIban()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szSourceIban = "NL46INGB9955442484";
                string szDestIban = "NL46INGB9955442484";
                string szAmount = "0.01";

                var returnResult = TansferProcess.ValidationTransferData(szSourceIban, szDestIban, szAmount);

                Assert.IsNull(returnResult);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.ToAndSourceAccIsSame));
                string expected = string.Format("{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_SourceNotFound()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szSourceIban = "NL46INGB9955442480";
                string szDestIban = "NL81ABNA7243913512";
                string szAmount = "0.01";

                var returnResult = TansferProcess.ValidationTransferData(szSourceIban, szDestIban, szAmount);

                Assert.IsNull(returnResult);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.SourceAccountNotFound));
                string expected = string.Format("{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_DestNotFound()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szSourceIban = "NL46INGB9955442484";
                string szDestIban = "NL81ABNA7243913510";
                string szAmount = "0.01";

                var returnResult = TansferProcess.ValidationTransferData(szSourceIban, szDestIban, szAmount);

                Assert.IsNull(returnResult);

                string szExpect = string.Format(VerifyResultEnum.ToDescriptionString(VerifyResult.ToAccountNotFound));
                string expected = string.Format("{0}{1}",
                    szExpect, Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void TestInputAmount_NotEnoughtBalance()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string szSourceIban = "NL46INGB9955442484";
                string szDestIban = "NL81ABNA7243913512";
                string szAmount = "10000.00";

                // step 1 ver data
                var returnResult = TansferProcess.ValidationTransferData(szSourceIban, szDestIban, szAmount);

                // Step 2 confirm
                var result = TansferProcess.TriggerConfirmTransfer(returnResult);

                Assert.AreEqual(result, VerifyResult.NotEnoughBalance);
            }
        }

    }

    
}
