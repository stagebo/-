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
        public List<object[]> SearchAccountInfoListByCondition(Dictionary<string, object> condition,List<string[]> orderList,int pageIndex,int pageSize)
        {
            if (!condition.ContainsKey("userID,Eq"))
            {
                return null;
            }
            ISession session = null;
            StringBuilder sqlStringBuilder =
                new StringBuilder().Append(
                $@"SELECT R.*,A.F_NAME,A.F_ID AS PID
                FROM t_running_account AS R

                INNER JOIN t_account_purpose AS A
                ON A.F_ID = R.f_purpose_id

                WHERE  R.F_EXIST =1
                ");
            /*添加筛选条件*/
            if (condition.ContainsKey("userID,Eq")) {
                sqlStringBuilder.Append(" AND R.F_USER_ID = :userID ");
            }
            if (orderList.Count > 0)
            {
                sqlStringBuilder.Append(" ORDER BY ");
            }
            /*添加排序条件*/
            bool isStart = true;
            foreach (string[] order in orderList) {
                sqlStringBuilder.Append(isStart?"":",");
                isStart = false;
                sqlStringBuilder.Append(" R."+order[0]+" "+order[1]);
            }
            

            try
            {
                session = SessionManager.OpenSession();
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sqlStringBuilder.ToString());
                /*添加参数*/
                if (condition.ContainsKey("userID,Eq"))
                {
                    iSQLQuery.SetGuid("userID",Guid.Parse(condition["userID,Eq"].ToString()));
                }

                iSQLQuery.AddEntity(typeof(RunningAccount))
                    .AddScalar("PID",NHibernateUtil.Guid)
                    .AddScalar("F_NAME",NHibernateUtil.String);
                return iSQLQuery
                    .SetFirstResult((pageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize).List<object[]>().ToList<object[]>();
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

        public List<object[]> SelectRunningAccountDataInfoListByConditon(Dictionary<string, object> condition)
        {
            ISession session = null;
            string sql = $@"
                        SELECT
                        {condition["field,Eq"]} as FIELD, SUM(F_MONEY) AS MONEY
                        FROM T_RUNNING_ACCOUNT 
                        WHERE F_TIME > :time
                        AND F_TYPE = :type
                        GROUP BY  {condition["field,Eq"]}
                        ";
            try
            {
                session = SessionManager.OpenSession();
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);
                iSQLQuery.SetInt32("type",(int)condition["type,Eq"])
                    .SetString("time",condition["datetime,Gt"].ToString());

                iSQLQuery.AddScalar("field",NHibernateUtil.Int32)
                    .AddScalar("money",NHibernateUtil.Decimal);

                return iSQLQuery.List<object[]>().ToList<object[]>();
            }
            catch (Exception ex)
            {
                Log4NetUtils.Error(this, "查询流水账，查询失败！", ex);
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
