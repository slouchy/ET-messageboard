using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MessageBoard.Tools
{
    public class LogTool
    {
        //public static DoLog()

        public static void DoErrorLog(string msg)
        {
            File.AppendAllText(HttpContext.Current.Server.MapPath($"~/Log/{DateTime.Now.ToString("yyyyMMdd")}.txt"), msg);
        }
    }
}