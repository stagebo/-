using System;
using System.Collections.Generic;
using System.Linq;
using BussinessLogicLayer;
using Common.NHibernate;
using Model;
using NHibernate;
using NHibernate.Linq;
using System.Text;
using System.Linq.Expressions;
namespace MyWebSiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ISession session = SessionManager.OpenSession();
            AccountPurpose ap = new AccountPurpose();
            ap.f_id = Guid.NewGuid();
            ap.f_name = "吃饭";
            ap.f_type = 1;
            ap.f_descript="吃饭用度";
            session.Save(ap);
            session.Flush();

            RunningAccount ra = new RunningAccount();
            ra.f_id = Guid.NewGuid();
            ra.f_purpose_id = ap.f_id;
            ra.f_time = DateTime.Now;
            ra.f_year = DateTime.Now.Year;
            ra.f_month = DateTime.Now.Month;
            ra.f_day = DateTime.Now.Day;
            ra.f_exist = 1;
            ra.f_address = "tianjin.";
            ra.f_money = 12.2M;
            ra.f_remark = "测试";
            session.Save(ra);
            session.Flush();

            //User us=session.Query<User>().SingleOrDefault(u => u.f_id == Guid.Parse("3488B4A2-7D2D-4553-B3FF-8A4C4278DD47")&&u.f_pwd=="123");
            //List<User> uList = session.Query<User>().Where<User>(u => u.f_exist == 1).ToList<User>() ;
            // int uCount = session.Query<User>().Where<User>(u => u.f_exist == 1).ToList<User>().Count;

            //IEnumerable<Message> msList = from ms in SessionManager.OpenSession().Query<Message>().
            //                              Where<Message>(m => m.f_message_exist == 1).ToList<Message>()
            //                              orderby ms.f_common_date descending
            //                              select ms;

            //session.Query<User>().Where<User>(ux=>ux.f_exist==1).OrderByDescending<User>("")
            //NHibernateHelper.Instance("web");
            //ISession se = NHibernateHelper.GetCurrentSession();
            //User uu = se.Query<User>().SingleOrDefault<User>(ux=>"123".Equals(ux.f_pwd));

            Console.ReadKey(); ;

        }
    }
}
