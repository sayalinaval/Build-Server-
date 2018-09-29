///////////////////////////////////////////////////////////////////////
// Logger.cs - Sending log to Repository Server                      //
// ver 1.0                                                           //
//                                                                   //
// Author: Sayali Naval, snaval@syr.edu                              //
// Source: Dr. Jim Fowcett                                           //
// Application: CSE681 Project 4-Build Server                        //
// Environment: C# console                                           //
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class logger
    {
        private string sLogFormat;
        private string sErrorTime;
        string path;

        //-----write result in log file
        public logger(string p)
        {
            path = p;
            sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            sErrorTime = sYear + sMonth + sDay;
        }

        //makes an entry in the log life
        public void ErrorLog(string sErrMsg)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(sLogFormat + sErrMsg);
            sw.Flush();
            sw.Close();
        }

        static void Main(string[] args)
        {
            Console.Title = "Logger";
        }
    }
}
