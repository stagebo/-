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
using Base;
namespace DataAccessLayer
{
    public class RunningAccountDAL : BaseDAL
    {
        public List<object[]> SearchAccountInfoListByCondition(Dictionary<string, object> condition)
        {
            if (condition.ContainsKey("userID,Eq"))
            {
                return null;
            }
            ISession session = null;
            string sql = $@"
                        SELECT R.*,A.*,U.* FROM
                        T_USER AS U
                        INNER JOIN t_running_account AS R
                        ON U.F_ID = R.f_user_id

                        INNER JOIN t_account_purpose AS A
                        ON A.F_ID = R.f_purpose_id

                        WHERE  R.F_EXIST =1
                        AND R.F_USER_ID = '{condition["userID,Eq"].ToString()}'
                        ";
            try
            {
                session = SessionManager.OpenSession();
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);
                iSQLQuery.AddEntity(typeof(RunningAccount));
                iSQLQuery.AddEntity(typeof(AccountPurpose));
                iSQLQuery.AddEntity(typeof(User));
                return iSQLQuery.List<object[]>().ToList<object[]>();
            }
            catch (Exception ex)
            {
                Log4NetUtils.Error(this, "查询流水账，查询失败！",ex);
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public List<object[]> selectBalanceInfoByCondition(Dictionary<string, object> condition)
        {
            if (!condition.ContainsKey("userID,Eq"))
            {
                return null;
            }
            ISession session = null;
            string sql = $@"
                         SELECT F_TYPE AS TYPE
                         ,SUM(F_MONEY) AS MONEY
                         FROM T_RUNNING_ACCOUNT
                         WHERE F_USER_ID = '{condition["userID,Eq"].ToString()}'
                         GROUP BY F_TYPE 
                        ";
            try
            {
                session = SessionManager.OpenSession();
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);
                iSQLQuery.AddScalar("TYPE", NHibernateUtil.Int32)
                   .AddScalar("MONEY", NHibernateUtil.Decimal);
                return iSQLQuery.List<object[]>().ToList<object[]>();
            }
            catch (Exception ex)
            {
                Log4NetUtils.Error(this, "查询流水账，查询失败！",ex);
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }
    }


}
