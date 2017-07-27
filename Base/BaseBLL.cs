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

        public virtual bool ExecuteTransaction(List<object[]> modelObjectList)
        {
            List<object[]> transactionItem = new List<object[]>();
            foreach (object[] item in modelObjectList)
            {
                if (item[0].Equals(TransactionItemType.ADD))
                {
                    transactionItem.Add(new object[] { BaseDAL<BaseModel>.TransactionItemSQLType.INSERT, item[1] });
                }
                else if(item[0].Equals(TransactionItemType.REMOVE))
                {
                    transactionItem.Add(new object[] { BaseDAL<BaseModel>.TransactionItemSQLType.DELETE, item[1] });
                }
                else if(item[0].Equals(TransactionItemType.MODIFY))
                {
                    transactionItem.Add(new object[] { BaseDAL<BaseModel>.TransactionItemSQLType.UPDATE, item[1] });
                }
            }
            return BaseDAL<BaseModel>.Transaction(transactionItem);
        }
    }
}
