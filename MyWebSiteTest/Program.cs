using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;

namespace MyWebSiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Log4NetUtils.Error("main","错误错误错误");
            int[] a = { 1,2,3};
            string s = string.Join("#",a);
            Console.WriteLine(s);
            Console.ReadKey(); ;
        }
    }
}
