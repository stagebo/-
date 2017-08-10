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
    public class UserBLL : BaseBLL
    {
        public List<User> SearchAllUser()
        {
            return new UserDAL().SelectAllUser();
        }
        public bool Test()
        {
            return new UserDAL().Test();
        }

    }
}
