using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Model;
using Base;
namespace BussinessLogicLayer
{
    public class RunningAccountBLL : BaseBLL
    {
        public List<object[]> SearchAccountInfoListByCondition(Dictionary<string, object> condition)
        {
            return new RunningAccountDAL().SearchAccountInfoListByCondition(condition);
        }
    }
}
