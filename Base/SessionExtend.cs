using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;
using Common.NHibernate;
using NHibernate;
using NHibernate.Criterion;

namespace Base
{
    /// <summary>
    /// ISession 扩展方法
    /// </summary>
    public static class SessionExtend
    {
        /// <summary>
        ///  Object Extend Method, Add Model To Database.
        /// </summary>
        /// <typeparam name="Model">实体类</typeparam>
        /// <param name="m">实体参数</param>
        /// <returns></returns>
        public static bool Add<Model>(this object model, Model m) where Model:BaseModel
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                session.Save(m);
                session.Flush();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }
        public static bool Add<Model>(this Model model, Model m) {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                session.Save(m);
                session.Flush();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }
        /// <summary>
        /// 添加Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool AddModel<Model>(this ISession session, Model m) where Model : BaseModel
        {
            try
            {
                session.Save(m);
                session.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据条件查询和排序顺序查询第一个Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public static Model SearchUniqueModelObjectByCondition<Model>(this ISession session,
            Dictionary<string, object> conditionDictionary,List<string[]> orderList) where Model : BaseModel
        {
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = session._createConditionCriteria(criteria, conditionDictionary);
                }
                if (orderList != null && orderList.Count > 0)
                {
                    criteria = session._createOrderCriteria(criteria, orderList);
                }
                return criteria.SetFirstResult(0)
                    .SetMaxResults(1)
                    .UniqueResult<Model>();
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                   "ISession",
                    "查询实体信息失败，实体类型：" + typeof(Model).FullName + "，" +
                        "查询条件：" + //Common.SerializeJsonString(conditionDictionary) + "，" +
                        "排序集合：",//+ Common.SerializeJsonString(orderList),
                    exception
                );
                return default(Model);
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }




       public static ICriteria _createConditionCriteria(this ISession session,ICriteria criteria, Dictionary<string, object> conditionDictionary)

        {
            foreach (KeyValuePair<string, object> conditionItem in conditionDictionary)
            {
                string[] splitStringArray = conditionItem.Key.Split(',');
                switch (splitStringArray[1])
                {
                    case "Between":
                        criteria.Add(Restrictions.Between(splitStringArray[0], ((object[])conditionItem.Value)[0], 
                            ((object[])conditionItem.Value)[1]));
                        break;
                    case "NotBetween":
                        criteria.Add(Restrictions.Not(Restrictions.Between(splitStringArray[0], 
                            ((object[])conditionItem.Value)[0], ((object[])conditionItem.Value)[1])));
                        break;
                    case "Eq":
                        criteria.Add(Restrictions.Eq(splitStringArray[0], conditionItem.Value));
                        break;
                    case "NotEq":
                        criteria.Add(Restrictions.Not(Restrictions.Eq(splitStringArray[0], conditionItem.Value)));
                        break;
                    case "Ge":
                        criteria.Add(Restrictions.Ge(splitStringArray[0], conditionItem.Value));
                        break;
                    case "Gt":
                        criteria.Add(Restrictions.Gt(splitStringArray[0], conditionItem.Value));
                        break;
                    case "Le":
                        criteria.Add(Restrictions.Le(splitStringArray[0], conditionItem.Value));
                        break;
                    case "Lt":
                        criteria.Add(Restrictions.Lt(splitStringArray[0], conditionItem.Value));
                        break;
                    case "IDEq":
                        criteria.Add(Restrictions.IdEq(conditionItem.Value));
                        break;
                    case "IDNotEq":
                        criteria.Add(Restrictions.Not(Restrictions.IdEq(conditionItem.Value)));
                        break;
                    case "In":
                        criteria.Add(Restrictions.In(splitStringArray[0], conditionItem.Value is object[] ? ((object[])conditionItem.Value) : ((ICollection)conditionItem.Value)));
                        break;
                    case "NotIn":
                        criteria.Add(Restrictions.Not(Restrictions.In(splitStringArray[0], conditionItem.Value is object[] ? ((object[])conditionItem.Value) : ((ICollection)conditionItem.Value))));
                        break;
                    case "IsNull":
                        criteria.Add(Restrictions.IsNull(splitStringArray[0]));
                        break;
                    case "IsNotNull":
                        criteria.Add(Restrictions.IsNotNull(splitStringArray[0]));
                        break;
                    case "Like":
                        criteria.Add(Restrictions.Like(splitStringArray[0], conditionItem.Value));
                        break;
                }
            }
            return criteria;
        }

        public static ICriteria _createOrderCriteria(this ISession session,ICriteria criteria, List<string[]> orderList)
        {
            foreach (string[] orderItem in orderList)
            {
                criteria.AddOrder(new Order(orderItem[0], orderItem[1].ToLower().Equals("asc")));
            }
            return criteria;
        }
    }
    
}
