using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickProject.Logic
{
    internal class RandomIban
    {
        public static string GetIban()
        {
            int timeout = 0;
            var options = new ChromeOptions();
            string szReturn = string.Empty;
            try
            {
                options.AddArgument("--window-position=-32000,-32000");
                using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options))
                {
                    driver.Navigate().GoToUrl(@"http://randomiban.com/?country=Netherlands");
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    while (driver.FindElements(By.Id("gen_button")).Count == 0 && timeout < 500)
                    {
                        Thread.Sleep(1);
                        timeout++;
                    }
                    IWebElement iBan = driver.FindElement(By.Id("demo"));
                    szReturn = iBan.Text;
                    driver.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0} EX:[{1}]", "GetIban", ex.Message));
            }
            return szReturn;
        }
    }
}
