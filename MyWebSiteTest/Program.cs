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
            string sql = "select * from t_message";
            ISession session=SessionManager.OpenSession();
            session = SessionManager.OpenSession();
            //加载SQL语句
            ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);

            //查询结果并返回
            var t= iSQLQuery.List<object>().ToList<object>();
            int[] a = { 1,2,3};
            string s = string.Join("#",a);
            Console.WriteLine(s);
            Console.ReadKey(); ;
        }
    }
}
