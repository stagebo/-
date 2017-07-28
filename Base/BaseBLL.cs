using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    public abstract class BaseBLL
    {
        public static class TransactionItemType
        {
            public static readonly string ADD = "add";
            public static readonly string REMOVE = "remove";
            public static readonly string MODIFY = "modify";
        }
        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="modelObjectList"></param>
        /// <returns></returns>
        public virtual bool ExecuteTransaction(List<object[]> modelObjectList)
        {
            List<object[]> transactionItem = new List<object[]>();
            foreach (object[] item in modelObjectList)
            {
                switch (item[0].ToString())
                {
                    case "add":
                        transactionItem.Add(new object[] { "insert", item[1] });
                        break;
                    case "remove":
                        transactionItem.Add(new object[] { "delete", item[1] });
                        break;
                    case "modify":
                        transactionItem.Add(new object[] { "update", item[1] });
                        break;
                }
            }
            return BaseDAL.Transaction(transactionItem);
        }
        #region
        /// <summary>
        /// 添加Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool AddModel<Model>(Model m) where Model : BaseModel
        {
            return new BaseDAL().Insert<Model>(m);
        }
        /// <summary>
        /// 删除Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool RemoveModel<Model>(Model m) where Model : BaseModel
        {
            return new BaseDAL().Delete(m);
        }

        /// <summary>
        /// 修改Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool ModifyModel<Model>(Model m) where Model : BaseModel
        {
            return new BaseDAL().Update(m);
        }
        /// <summary>
        /// 查询下一个自增ID
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public int? SearchNextIdentity<Model>() where Model : BaseModel
        {
            return new BaseDAL().SelectNextIdentity<Model>();
        }
        /// <summary>
        /// 根据条件查询Model数量
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public int? SearchModelObjectCountByCondition<Model>() where Model : BaseModel
        {
            return this.SearchModelObjectCountByCondition<Model>(null);
        }
        /// <summary>
        /// 根据条件查询Model数量
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <returns></returns>
        public int? SearchModelObjectCountByCondition<Model>(Dictionary<string, object> conditionDictionary)
            where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectCountByCondition<Model>(conditionDictionary);
        }
        /// <summary>
        /// 根据ID查询Model是否存在
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="modelObjectID"></param>
        /// <returns></returns>
        public bool? SearchModelObjectExistsByID<Model>(object modelObjectID) where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectExistsByID<Model>(modelObjectID);
        }
        /// <summary>
        /// 根据条件查询Model是否存在
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <returns></returns>
        public bool? SearchModelObjectExistsByCondition<Model>(Dictionary<string, object> conditionDictionary)
            where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectExistsByCondition<Model>(conditionDictionary);
        }
        /// <summary>
        /// 根据ID查询Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="modelObjectID"></param>
        /// <returns></returns>
        public Model SearchModelObjectByID<Model>(object modelObjectID) where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectByID<Model>(modelObjectID);
        }
        /// <summary>
        /// 根据条件查询第一个Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public Model SearchUniqueModelObjectByCondition<Model>() where Model : BaseModel
        {
            return this.SearchUniqueModelObjectByCondition<Model>(null, null);
        }
        /// <summary>
        /// 根据条件查询第一个Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <returns></returns>
        public Model SearchUniqueModelObjectByCondition<Model>(Dictionary<string, object> conditionDictionary) where Model : BaseModel
        {
            return this.SearchUniqueModelObjectByCondition<Model>(conditionDictionary, null);
        }
        /// <summary>
        /// 根据条件查询和排序顺序查询第一个Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public Model SearchUniqueModelObjectByCondition<Model>(Dictionary<string, object> conditionDictionary,
            List<string[]> orderList) where Model : BaseModel
        {
            return new BaseDAL().SelectUniqueModelObjectByCondition<Model>(conditionDictionary, orderList);
        }
        /// <summary>
        /// 根据条件查询ModelList
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public List<Model> SearchModelObjectListByCondition<Model>() where Model : BaseModel
        {
            return this.SearchModelObjectListByCondition<Model>(null, null);
        }
        /// <summary>
        /// 根据条件查询ModelList
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <returns></returns>
        public List<Model> SearchModelObjectListByCondition<Model>(Dictionary<string, object> conditionDictionary) where Model : BaseModel
        {
            return this.SearchModelObjectListByCondition<Model>(conditionDictionary, null);
        }
        /// <summary>
        /// 根据条件查询ModelList，并按照一定顺序排序
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public List<Model> SearchModelObjectListByCondition<Model>(Dictionary<string, object> conditionDictionary,
            List<string[]> orderList) where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectListByCondition<Model>(conditionDictionary, orderList);
        }
        /// <summary>
        /// 根据条件按一定条件排序分页查询
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="conditionDictionary"></param>
        /// <param name="orderList"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Model> SearchModelObjectListByPage<Model>(Dictionary<string, object> conditionDictionary,
            List<string[]> orderList, int pageIndex, int pageSize) where Model : BaseModel
        {
            return new BaseDAL().SelectModelObjectListByPage<Model>(conditionDictionary, orderList, pageIndex, pageSize);
        }

        #endregion
    }
}
