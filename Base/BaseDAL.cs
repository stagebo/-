using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Common.Log4Net;
using System.Collections;
using NHibernate;
using Common.NHibernate;
using NHibernate.Criterion;

namespace Base
{
    public  class BaseDAL<Model> where Model : BaseModel
    {
        public bool Insert(Model modelObject)
        {
            Type modelObjectType = modelObject.GetType();
            string tableName = null;
            List<Dictionary<string, string>> entityPropertyList = null;
            try
            {
                Dictionary<string, object> entityTableMetaInfo = SessionManager.GetEntityTableMetaInfo(modelObjectType.Name);
                if (entityTableMetaInfo == null)
                {
                    throw new Exception("获取实体表结构信息失败，Model类型：" + modelObjectType.FullName);
                }
                tableName = entityTableMetaInfo["tableName"].ToString();
                entityPropertyList = (List<Dictionary<string, string>>)entityTableMetaInfo["entityPropertyList"];
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "insert实体对象失败，Model类型：" + modelObjectType.FullName + "，Model内容：" + modelObject.ToJsonString(), exception);
                return false;
            }
            object modelObjectID = null;
            try
            {
                modelObjectID = modelObjectType.GetProperty(entityPropertyList[0]["name"]).GetValue(modelObject, null);
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "insert实体对象失败，Model类型：" + modelObjectType.FullName + "，Model内容：" + modelObject.ToJsonString(), exception);
                return false;
            }
            ISession session = null;
            if (modelObjectID is Int32 && modelObjectID.Equals(default(Int32)) || modelObjectID is Guid)
            {
                try
                {
                    session = SessionManager.OpenSession();
                    session.Save(modelObject);
                    session.Flush();
                    return true;
                }
                catch (Exception exception)
                {
                    Log4NetUtils.Error(this, "insert实体对象失败，Model类型：" + modelObjectType.FullName + "，Model内容：" + modelObject.ToJsonString(), exception);
                    return false;
                }
                finally
                {
                    SessionManager.CloseSession(session);
                }
            }
            else
            {
                ITransaction transaction = null;
                try
                {
                    session = SessionManager.OpenSession();
                    transaction = session.BeginTransaction();
                    session.CreateSQLQuery("SET IDENTITY_INSERT [" + tableName + "] ON").ExecuteUpdate();
                    string columnString = "[" + entityPropertyList[0]["column"] + "]";
                    string valueString = ":" + entityPropertyList[0]["column"];
                    for (int i = 1; i < entityPropertyList.Count; i++)
                    {
                        columnString += "," + "[" + entityPropertyList[i]["column"] + "]";
                        valueString += ",:" + entityPropertyList[i]["column"];
                    }
                    ISQLQuery insert = session.CreateSQLQuery("INSERT INTO [" + tableName + "](" + columnString + ") VALUES(" + valueString + ")");
                    foreach (Dictionary<string, string> property in entityPropertyList)
                    {
                        string type = property["type"].ToLower();
                        object value = modelObjectType.GetProperty(property["name"]).GetValue(modelObject, null);
                        if (type.Equals("int"))
                        {
                            if (value == null)
                            {
                                insert.SetParameter(property["column"], new Nullable<int>());
                            }
                            else
                            {
                                insert.SetInt32(property["column"], Convert.ToInt32(value));
                            }
                        }
                        else if (type.Equals("long"))
                        {
                            if (value == null)
                            {
                                insert.SetParameter(property["column"], new Nullable<long>());
                            }
                            else
                            {
                                insert.SetInt64(property["column"], Convert.ToInt64(value));
                            }
                        }
                        else if (type.Equals("decimal"))
                        {
                            if (value == null)
                            {
                                insert.SetParameter(property["column"], new Nullable<decimal>());
                            }
                            else
                            {
                                insert.SetDecimal(property["column"], Convert.ToDecimal(value));
                            }
                        }
                        else if (type.Equals("string"))
                        {
                            insert.SetString(property["column"], value as string);
                        }
                        else if (type.Equals("datetime"))
                        {
                            if (value == null)
                            {
                                insert.SetParameter(property["column"], new Nullable<DateTime>());
                            }
                            else
                            {
                                insert.SetDateTime(property["column"], Convert.ToDateTime(value));
                            }
                        }
                        else if (type.Equals("guid"))
                        {
                            if (value == null)
                            {
                                insert.SetParameter(property["column"], new Nullable<Guid>());
                            }
                            else
                            {
                                insert.SetGuid(property["column"], Guid.Parse(value.ToString()));
                            }
                        }
                        else
                        {
                            throw new Exception("未处理的字段数据类型");
                        }
                    }
                    insert.ExecuteUpdate();
                    session.CreateSQLQuery("SET IDENTITY_INSERT [" + tableName + "] OFF").ExecuteUpdate();
                    transaction.Commit();
                    return true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    Log4NetUtils.Error(this, "insert实体对象失败，Model类型：" + modelObjectType.FullName + "，Model内容：" + modelObject.ToJsonString(), exception);
                    return false;
                }
                finally
                {
                    SessionManager.DisposeTransaction(transaction);
                    SessionManager.CloseSession(session);
                }
            }
        }

        public bool Delete(object modelObject)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                session.Delete(modelObject);
                session.Flush();
                return true;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "删除实体失败，Model类型：" + modelObject.GetType().FullName, exception);
                return false;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public bool Update(object modelObject)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                session.Update(modelObject);
                session.Flush();
                return true;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "更新实体失败，Model类型：" + modelObject.GetType().FullName, exception);
                return false;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public static bool Transaction(List<object[]> modelObjectList)
        {
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                session = SessionManager.OpenSession();
                transaction = session.BeginTransaction();
                foreach (object[] item in modelObjectList)
                {
                    IList objectList = null;
                    if (item[1] is IList)
                    {
                        objectList = item[1] as IList;
                    }
                    else
                    {
                        objectList = new List<object>() { item[1] };
                    }
                    if (item[0].Equals("insert") && objectList.Count > 0)
                    {
                        Type modelObjectType = objectList[0].GetType();
                        Dictionary<string, object> entityTableMetaInfo = SessionManager.GetEntityTableMetaInfo(modelObjectType.Name);
                        if (entityTableMetaInfo == null)
                        {
                            throw new Exception("获取实体表结构信息失败，Model类型：" + modelObjectType.FullName);
                        }
                        string tableName = entityTableMetaInfo["tableName"].ToString();
                        List<Dictionary<string, string>> entityPropertyList = (List<Dictionary<string, string>>)entityTableMetaInfo["entityPropertyList"];
                        string columnString = "[" + entityPropertyList[0]["column"] + "]";
                        string valueString = ":" + entityPropertyList[0]["column"];
                        for (int i = 1; i < entityPropertyList.Count; i++)
                        {
                            columnString += "," + "[" + entityPropertyList[i]["column"] + "]";
                            valueString += ",:" + entityPropertyList[i]["column"];
                        }
                        string insertSQL = "INSERT INTO [" + tableName + "](" + columnString + ") VALUES(" + valueString + ")";
                        foreach (object modelObject in objectList)
                        {
                            object modelObjectID = modelObjectType.GetProperty(entityPropertyList[0]["name"]).GetValue(modelObject, null);
                            if (modelObjectID is Int32 && modelObjectID.Equals(default(Int32)) || modelObjectID is Guid)
                            {
                                session.Save(modelObject);
                            }
                            else
                            {
                                session.CreateSQLQuery("SET IDENTITY_INSERT [" + tableName + "] ON").ExecuteUpdate();
                                ISQLQuery insert = session.CreateSQLQuery(insertSQL);
                                foreach (Dictionary<string, string> property in entityPropertyList)
                                {
                                    string type = property["type"].ToLower();
                                    object value = modelObjectType.GetProperty(property["name"]).GetValue(modelObject, null);
                                    if (type.Equals("int"))
                                    {
                                        if (value == null)
                                        {
                                            insert.SetParameter(property["column"], new Nullable<int>());
                                        }
                                        else
                                        {
                                            insert.SetInt32(property["column"], Convert.ToInt32(value));
                                        }
                                    }
                                    else if (type.Equals("long"))
                                    {
                                        if (value == null)
                                        {
                                            insert.SetParameter(property["column"], new Nullable<long>());
                                        }
                                        else
                                        {
                                            insert.SetInt64(property["column"], Convert.ToInt64(value));
                                        }
                                    }
                                    else if (type.Equals("decimal"))
                                    {
                                        if (value == null)
                                        {
                                            insert.SetParameter(property["column"], new Nullable<decimal>());
                                        }
                                        else
                                        {
                                            insert.SetDecimal(property["column"], Convert.ToDecimal(value));
                                        }
                                    }
                                    else if (type.Equals("string"))
                                    {
                                        insert.SetString(property["column"], value == null ? null : value.ToString());
                                    }
                                    else if (type.Equals("datetime"))
                                    {
                                        if (value == null)
                                        {
                                            insert.SetParameter(property["column"], new Nullable<DateTime>());
                                        }
                                        else
                                        {
                                            insert.SetDateTime(property["column"], Convert.ToDateTime(value));
                                        }
                                    }
                                    else if (type.Equals("guid"))
                                    {
                                        if (value == null)
                                        {
                                            insert.SetParameter(property["column"], new Nullable<Guid>());
                                        }
                                        else
                                        {
                                            insert.SetGuid(property["column"], Guid.Parse(value.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("未处理的字段数据类型");
                                    }
                                }
                                insert.ExecuteUpdate();
                                session.CreateSQLQuery("SET IDENTITY_INSERT [" + tableName + "] OFF").ExecuteUpdate();
                            }
                        }
                    }
                    else if (item[0].Equals("delete") && objectList.Count > 0)
                    {
                        foreach (object modelObject in objectList)
                        {
                            session.Delete(modelObject);
                        }
                    }
                    else if (item[0].Equals("update") && objectList.Count > 0)
                    {
                        foreach (object modelObject in objectList)
                        {
                            session.Update(modelObject);
                        }
                    }
                    session.Flush();
                }
                transaction.Commit();
                return true;
            }
            catch (Exception exception)
            {
                transaction.Rollback();
                Log4NetUtils.Error("Common.BaseDAL", "事务执行失败，modelList：");// + Common.SerializeJsonString(modelObjectList), exception);
                return false;
            }
            finally
            {
                SessionManager.DisposeTransaction(transaction);
                SessionManager.CloseSession(session);
            }
        }

        public int? SelectNextIdentity()
        {
            ISession session = null;
            try
            {
                Dictionary<string, object> entityTableMetaInfo = SessionManager.GetEntityTableMetaInfo(typeof(Model).Name);
                if (entityTableMetaInfo == null)
                {
                    throw new Exception("查询实体表结构信息失败，Model类型：" + typeof(Model).FullName);
                }
                string tableName = entityTableMetaInfo["tableName"].ToString();
                string sql = "SELECT IDENT_CURRENT('" + tableName + "') + IDENT_INCR ('" + tableName + "')";
                session = SessionManager.OpenSession();
                int result = Convert.ToInt32(session.CreateSQLQuery(sql).UniqueResult<object>());
                return result;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "查询自增长主键ID失败，Model类型：" + typeof(Model).FullName, exception);
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public int? SelectModelObjectCountByCondition()
        {
            return this.SelectModelObjectCountByCondition(null);
        }

        public int? SelectModelObjectCountByCondition(Dictionary<string, object> conditionDictionary)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = this._createConditionCriteria(criteria, conditionDictionary);
                }
                return Convert.ToInt32(criteria.SetProjection(Projections.RowCount()).UniqueResult());
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(this, "查询实体数量失败，实体类型：" + typeof(Model).FullName, exception);
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public bool? SelectModelObjectExistsByID(object modelObjectID)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                criteria.Add(Restrictions.IdEq(modelObjectID));
                return Convert.ToInt32(criteria.SetProjection(Projections.RowCount()).UniqueResult()) > 0;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体ID是否存在失败，实体类型：" + typeof(Model).FullName + "，" +
                        "主键：" + modelObjectID.ToString(),
                    exception
                );
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public bool? SelectModelObjectExistsByCondition(Dictionary<string, object> conditionDictionary)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = this._createConditionCriteria(criteria, conditionDictionary);
                }
                return Convert.ToInt32(criteria.SetProjection(Projections.RowCount()).UniqueResult()) > 0;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体是否存在失败，实体类型：" + typeof(Model).FullName + "，" +
                        "查询条件：",// + Common.SerializeJsonString(conditionDictionary),
                    exception
                );
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public Model SelectModelObjectByID(object modelObjectID)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                criteria.Add(Restrictions.IdEq(modelObjectID));
                return criteria.UniqueResult<Model>();
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体信息失败，实体类型：" + typeof(Model).FullName + "，" +
                        "主键：" + modelObjectID.ToString(),
                    exception
                );
                return default(Model);
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public Model SelectUniqueModelObjectByCondition()
        {
            return this.SelectUniqueModelObjectByCondition(null, null);
        }

        public Model SelectUniqueModelObjectByCondition(Dictionary<string, object> conditionDictionary)
        {
            return this.SelectUniqueModelObjectByCondition(conditionDictionary, null);
        }

        public Model SelectUniqueModelObjectByCondition(Dictionary<string, object> conditionDictionary, List<string[]> orderList)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = this._createConditionCriteria(criteria, conditionDictionary);
                }
                if (orderList != null && orderList.Count > 0)
                {
                    criteria = this._createOrderCriteria(criteria, orderList);
                }
                return criteria.SetFirstResult(0)
                    .SetMaxResults(1)
                    .UniqueResult<Model>();
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体信息失败，实体类型：" + typeof(Model).FullName + "，" +
                        "查询条件：" + //Common.SerializeJsonString(conditionDictionary) + "，" +
                        "排序集合：" ,//+ Common.SerializeJsonString(orderList),
                    exception
                );
                return default(Model);
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public List<Model> SelectModelObjectListByCondition()
        {
            return this.SelectModelObjectListByCondition(null, null);
        }

        public List<Model> SelectModelObjectListByCondition(Dictionary<string, object> conditionDictionary)
        {
            return this.SelectModelObjectListByCondition(conditionDictionary, null);
        }

        public List<Model> SelectModelObjectListByCondition(Dictionary<string, object> conditionDictionary, List<string[]> orderList)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = this._createConditionCriteria(criteria, conditionDictionary);
                }
                if (orderList != null && orderList.Count > 0)
                {
                    criteria = this._createOrderCriteria(criteria, orderList);
                }
                return criteria
                    .List<Model>()
                    .ToList<Model>();
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体集合信息失败，实体类型：" + typeof(Model).FullName + "，" +
                        "查询条件："+// + Common.SerializeJsonString(conditionDictionary) + "，" +
                        "排序集合：",// + Common.SerializeJsonString(orderList),
                    exception
                );
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        public List<Model> SelectModelObjectListByPage(Dictionary<string, object> conditionDictionary, List<string[]> orderList, int pageIndex, int pageSize)
        {
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ICriteria criteria = session.CreateCriteria(typeof(Model));
                if (conditionDictionary != null && conditionDictionary.Count > 0)
                {
                    criteria = this._createConditionCriteria(criteria, conditionDictionary);
                }
                if (orderList != null && orderList.Count > 0)
                {
                    criteria = this._createOrderCriteria(criteria, orderList);
                }
                return criteria
                    .SetFirstResult((pageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize)
                    .List<Model>()
                    .ToList<Model>();
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error(
                    this,
                    "查询实体分页信息失败，实体类型：" + typeof(Model).FullName + "，" +
                        "查询条件：" +// Common.SerializeJsonString(conditionDictionary) + "，" +
                        "排序集合：" + //Common.SerializeJsonString(orderList) + "，" +
                        "分页信息：" + pageIndex + "，" + pageSize,
                    exception
                );
                return null;
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }

        protected virtual ICriteria _createConditionCriteria(ICriteria criteria, Dictionary<string, object> conditionDictionary)

        {
            foreach (KeyValuePair<string, object> conditionItem in conditionDictionary)
            {
                string[] splitStringArray = conditionItem.Key.Split(',');
                switch (splitStringArray[1])
                {
                    case "Between":
                        criteria.Add(Restrictions.Between(splitStringArray[0], ((object[])conditionItem.Value)[0], ((object[])conditionItem.Value)[1]));
                        break;
                    case "NotBetween":
                        criteria.Add(Restrictions.Not(Restrictions.Between(splitStringArray[0], ((object[])conditionItem.Value)[0], ((object[])conditionItem.Value)[1])));
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

        protected virtual ICriteria _createOrderCriteria(ICriteria criteria, List<string[]> orderList)
        {
            foreach (string[] orderItem in orderList)
            {
                criteria.AddOrder(new Order(orderItem[0], orderItem[1].ToLower().Equals("asc")));
            }
            return criteria;
        }
    }
}
