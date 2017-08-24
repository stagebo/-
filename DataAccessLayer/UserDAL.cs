using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;
using Common.NHibernate;
using Model;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace DataAccessLayer
{
    public class UserDAL
    {
        public List<User> SelectAllUser()
        {
            /* NHibernate连接对象 */
            ISession session = null;
            //原生SQL语句
            StringBuilder sqlStringBuilder = new StringBuilder(string.Empty);
            sqlStringBuilder.Append(@"
                            SELECT T.* FROM t_user T
                    ");
            string sql = sqlStringBuilder.ToString();
            try
            {
                /* 获取连接池中的活动连接对象 */
                session = SessionManager.OpenSession();
                /* 加载SQL语句 */
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);
                iSQLQuery.AddEntity(typeof(User));

                /*查询结果并返回*/
                return iSQLQuery.List<User>().ToList<User>();
            }
            catch (Exception e)
            {
                LogUtils.Error(this, "根据条件查询用户信息失败", e);
                return null;
            }
            finally
            {
                /* 释放活动的连接对象回到连接池 */
                SessionManager.CloseSession(session);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Test()
        {
            /* NHibernate连接对象 */
            ISession session = null;
            ITransaction tran = null;
            try
            {
                session = SessionManager.OpenSession(new EmptyInterceptor());

                /*Session操作*/
                Guid id = Guid.Parse("56E4AAFE-D5AD-4D35-A041-797F00F612A8");
                ICriteria criteria = session.CreateCriteria(typeof(User));
                criteria.Add(Restrictions.IdEq(id));
                User u = criteria.UniqueResult<User>();

                User uNew = new User();
                //uNew.f_id = Guid.Parse("04DB4AEB-3DA5-4D57-9011-9ECE667AB9D5");
                //uNew.f_uid = "Session"+DateTime.Now.ToShortDateString();
                //uNew.f_reg_date = DateTime.Now;
                //uNew.f_pwd = "abc";
                //uNew.f_exist = 1;
                //session.Update(uNew);
                //session.Flush();


                /*Tran操作*/
                tran = session.BeginTransaction();
                criteria = session.CreateCriteria(typeof(User));
                criteria.Add(Restrictions.IdEq(id));
                u = criteria.UniqueResult<User>();

                uNew = new User();
                uNew.f_id = Guid.Parse("04DB4AEB-3DA5-4D57-9011-9ECE667AB9D5");
                uNew.f_uid = "Tran"+DateTime.Now.ToShortDateString();
                uNew.f_reg_date = DateTime.Now;
                uNew.f_pwd = "abc";
                uNew.f_exist = 1;
                session.Update(uNew);

                tran.Commit();


                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
   

}
