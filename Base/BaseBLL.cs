using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    public abstract class BaseBLL
    {
        public bool ModifyModel<T>(T t )where T :BaseModel
        {
            return true;
        }
        public static class TransactionItemType
        {
            public static readonly string ADD = "add";
            public static readonly string REMOVE = "remove";
            public static readonly string MODIFY = "modify";
        }

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
            return BaseDAL<BaseModel>.Transaction(transactionItem);
        }
        public virtual bool AddModel<T>(T t) where T : BaseModel
        {
            return new BaseDAL<T>().Insert(t);
            //return new Common.BaseDAL().Insert(t);
        }
    }
}
