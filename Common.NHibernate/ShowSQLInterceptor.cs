using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log4Net;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace Common.NHibernate
{
    public class ShowSQLInterceptor : IInterceptor
    {
        #region
        public void AfterTransactionBegin(ITransaction tx)
        {

        }

        public void AfterTransactionCompletion(ITransaction tx)
        {

        }

        public void BeforeTransactionCompletion(ITransaction tx)
        {

        }

        public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            return null;
        }

        public object GetEntity(string entityName, object id)
        {
            return null;
        }

        public string GetEntityName(object entity)
        {
            return null;
        }

        public object Instantiate(string entityName, EntityMode entityMode, object id)
        {
            return null;
        }

        public bool? IsTransient(object entity)
        {
            return null;
        }

        public void OnCollectionRecreate(object collection, object key)
        {

        }

        public void OnCollectionRemove(object collection, object key)
        {

        }

        public void OnCollectionUpdate(object collection, object key)
        {

        }

        public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {

        }

        public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            return true;
        }

        public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return true;
        }




        public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return true;

        }

        public void PostFlush(ICollection entities)
        {

        }

        public void PreFlush(ICollection entities)
        {

        }

        public void SetSession(ISession session)
        {

        }
        #endregion
        /// <summary>
        /// Debug状态下展示每一次sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlString OnPrepareStatement(SqlString sql)
        {
            System.Diagnostics.Debug.WriteLine($"Show SQL-{DateTime.Now.ToString()}:【{sql}】");
            return sql;
        }
    }
}
