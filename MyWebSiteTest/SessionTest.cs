using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.NHibernate;
using Model;
using NHibernate;

namespace MyWebSiteTest
{
    public class SessionTest
    {
        private int N = 1000;
        public long SameSession()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();


            ISession session = SessionManager.OpenSession();
            for (int i = 0; i < N; i++)
            {
                User u = new User();
                u.f_id = Guid.NewGuid();
                u.f_pwd = "123";
                u.f_uid = "www";
                u.f_reg_date = DateTime.Now;
                u.f_exist = 1;

                session.Save(u);
                session.Flush();

                session.Delete(u);
                session.Flush();

            }

            SessionManager.CloseSession(session);

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public long DefferentSession()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < N; i++) {
                ISession session = SessionManager.OpenSession();
                User u = new User();
                u.f_id = Guid.NewGuid();
                u.f_pwd = "123";
                u.f_uid = "www";
                u.f_reg_date = DateTime.Now;
                u.f_exist = 1;

                session.Save(u);
                session.Flush();

                session.Delete(u);
                session.Flush();

            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public void Run()
        {
            long sT = SameSession();
            long dT = DefferentSession();

            Console.WriteLine($"同一个session插入1000条数据用时{sT}ms");
            Console.WriteLine($"每次都用不同session插入1000条数据用时{dT}ms");
        }
    }
}
