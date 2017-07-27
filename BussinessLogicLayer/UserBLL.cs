using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Model;

namespace BussinessLogicLayer
{
    public class UserBLL
    {
        public List<User> SearchAllUser()
        {
            return new UserDAL().SelectAllUser();
        }
    }
}
