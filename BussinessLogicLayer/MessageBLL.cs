using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;
using DataAccessLayer;
using Model;


namespace BussinessLogicLayer
{
    public class MessageBLL : BaseBLL
    {
        public List<Message> SearchAllMessage()
        {
            return new MessageDAL().SelectAllMessage();
        }
    }
}
