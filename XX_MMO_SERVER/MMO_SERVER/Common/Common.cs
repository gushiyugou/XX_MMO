using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class tempClass { }




    #region 弃用,日志系统思路
    //public class Log
    //{
    //    public const int DEBUG = 0;
    //    public const int INFO = 1;
    //    public const int WARN = 2;
    //    public const int ERROR = 3;


    //    public static int Level = DEBUG;
    //    private static string[] LevelName = { "DEBUG", "INFO", "WARN", "ERROR" };

    //    public delegate void PrintCallback(string text);
    //    public static event PrintCallback Print;

    //    static Log()
    //    {
    //        Log.Print += (text) =>
    //        {
    //            Console.WriteLine(text);
    //        };
    //    }

    //    private static  void WriteLine(int lev,string text, params object?[]? args)
    //    {
    //        if(Level <= lev)
    //        {
    //            text = String.Format(text, args);
    //            text = String.Format("[{0}]_:{1}", LevelName[lev], text);
    //            Print?.Invoke(text);
    //        }
    //    }

    //    public static void Debug(string text,params object?[]? args)
    //    {
    //        WriteLine(0, text);
    //    }



    //    public static void Info(string text, params object?[]? args)
    //    {
    //        WriteLine(1, text);
    //    }


    //    public static void Warn(string text, params object?[]? args)
    //    {
    //        WriteLine(2, text);
    //    }


    //    public static void Error(string text, params object?[]? args)
    //    {
    //        WriteLine(3, text);
    //    }


    //}

    #endregion
}
