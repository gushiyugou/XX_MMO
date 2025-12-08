using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Singleton<T> where T : new()
    {
        private static T? instance;
        public static object lockObj = new object();
        public static T Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
                
            }
        }
    }
}
