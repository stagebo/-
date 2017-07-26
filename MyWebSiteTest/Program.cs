using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;
using Common.NHibernate;
using NHibernate;

namespace MyWebSiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ISession v=SessionManager.OpenSession();
            int[] a = { 1,2,3};
            string s = string.Join("#",a);
            Console.WriteLine(s);
            Console.ReadKey(); ;
        }
    }
}
