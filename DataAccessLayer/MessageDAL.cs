using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;
using Common.NHibernate;
using Model;
using NHibernate;

namespace DataAccessLayer
{
    public class MessageDAL
    {
        public List<Message> SelectAllMessage()
        {
            /* NHibernate连接对象 */
            ISession session = null;
            //原生SQL语句
            StringBuilder sqlStringBuilder = new StringBuilder(string.Empty);
            sqlStringBuilder.Append(@"
                            SELECT T.* FROM t_message T
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
                return iSQLQuery.List<Message>().ToList<Message>();
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
    }
}
