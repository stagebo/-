using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Model;
using Base;
using Common;

namespace BussinessLogicLayer
{
    public class RunningAccountBLL : BaseBLL
    {
        public List<object[]> SearchAccountInfoListByCondition(Dictionary<string, object> condition, List<string[]> orderList, int pageIndex, int pageSize)
        {
            return new RunningAccountDAL().SearchAccountInfoListByCondition(condition, orderList, pageIndex, pageSize);
        }

        public Dictionary<string, object> SearchBalanceInfoByCondition(Dictionary<string, object> condition)
        {
            List<object[]> balanceInfoList = new RunningAccountDAL().selectBalanceInfoByCondition(condition);
            if (balanceInfoList == null)
            {
                return null;
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            decimal money = 0;
            foreach (object[] o in balanceInfoList)
            {
                if (o.Length < 2)
                {
                    continue;
                }
                decimal count = decimal.Parse(o[1].ToString());
                int type = int.Parse(o[0].ToString());
                if (CommonEnum.RunningAccountType.INCOME == type)
                {
                    money += count;
                }
                else if (CommonEnum.RunningAccountType.OUTCOME == type)
                {
                    money -= count;
                }
            }
            result.Add("money", money);
            return result;
        }

        public Dictionary<int, decimal> SearchRunningAccountDataInfoListByCondition(Dictionary<string, object> condition)
        {
            List<object[]> raInfoList = new RunningAccountDAL().SelectRunningAccountDataInfoListByConditon(condition);
            if (raInfoList == null)
            {
                return null;
            }
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();
            foreach (object[] o in raInfoList)
            {
                int field = int.Parse(o[0].ToString());
                decimal money = decimal.Parse(o[1].ToString());
                if (!result.ContainsKey(field))
                {
                    result.Add(field,money);
                }

            }
            return result;
        }
    }
}
