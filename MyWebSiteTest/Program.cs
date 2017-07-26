using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebSiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] a = { 1,2,3};
            string s = string.Join("#",a);
            Console.WriteLine(s);
            Console.ReadKey(); ;
        }
    }
}
