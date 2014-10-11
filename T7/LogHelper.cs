using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.IO;
using log4net.Config;
using System.Threading;

namespace CommonSuite
{
    internal class LogHelper
    {
        static Thread logThread;
        static ILog suiteLog = LogManager.GetLogger("SuiteLog");
        static TrionicCANLib.Log.LogQueue<LogEntry> logQueue;
        static bool isRunning = false;

        static LogHelper()
        {
            XmlConfigurator.Configure(new MemoryStream(T7.Properties.Resources.log4net_config));
            logQueue = new TrionicCANLib.Log.LogQueue<LogEntry>();
            logThread = new Thread(LogMain);
            logThread.Priority = ThreadPriority.BelowNormal;
            logThread.Start();
        }

        static void LogMain()
        {
            while (true)
            {
                var logItem = logQueue.Dequeue();
                if (!isRunning)
                {
                    lock (logThread)
                    {
                        XmlConfigurator.Configure(new MemoryStream(T7.Properties.Resources.log4net_config));
                        isRunning = true;
                    }
                }
                
                if (logItem != null)
                {
                    suiteLog.Info(logItem);
                }
            }
        }

        internal static void Log(string item)
        {
            logQueue.Enqueue(new LogEntry { msg = item });
        }

        private class LogEntry
        {
            public string msg;
            DateTime time;

            public LogEntry()
            {
                time = DateTime.Now;
            }

            public override string ToString()
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff} - {1}", time, msg);
            }
        }

        internal static void Flush()
        {
            lock (logThread)
            {
                LogManager.Shutdown();
                isRunning = false;
            }
        }
    }
}
