using NLog;
using System;
using System.IO;

namespace QuickProject
{
    public class XUtl
    {
        #region "Logger"
        public static void InitLog()
        {
            var logConfig = new NLog.LogFactory();
            string szConfigPath = MainProcess.szNlogConfigPath;

            logConfig.LoadConfiguration(@szConfigPath);

            NLog.LogManager.ThrowConfigExceptions = true;
            NLog.LogManager.Configuration = logConfig.Configuration;
        }

        public static Logger GetCurrentLog()
        {
            return NLog.LogManager.GetCurrentClassLogger();
        }

        #endregion "Logger"

    }

    public class LogEventException : Exception
    {

    }
	public class LogEvents
	{
        public const string LogTypeEvent = "eventRule";

        public static Logger logger;

        string szNlogPath = MainProcess.szNlogPath;

        public LogEvents(string LogPath)
		{
            if (!Directory.Exists(szNlogPath))
			{
				Directory.CreateDirectory(szNlogPath);
			}
            
            //Init log
            XUtl.InitLog();
            logger = XUtl.GetCurrentLog();
		}

		public void AppendLog(string EventDesc)
		{
            string szTxt = string.Empty;

            try
			{
                logger = XUtl.GetCurrentLog();
                logger = LogManager.GetLogger(LogTypeEvent);

                logger.Trace(EventDesc);

                Console.WriteLine(EventDesc);

            }
			catch (Exception ex2)
			{
                szTxt = string.Format("NLog AppendApplication Exception [{0}]", ex2.Message);
                Console.WriteLine(szTxt);
                throw new Exception(string.Format("nLog Exception [{0}]", ex2.Message));
            }
		}
	}


}
