using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SWE_573_RWPA
{
    class Logger
    {
        public static StreamWriter sw;
        public static void Log(string pid,string logMessage)
        {
            DateTime dt = DateTime.Now;
            //format date and append to log file
            sw.WriteLine(String.Format("{0:yyyymmdd HH/mm/ss/fff}", dt) + ",pid=" + pid + ", " + logMessage);
            Console.WriteLine("pid=" + pid + ", " + logMessage);
        }
    }
}
