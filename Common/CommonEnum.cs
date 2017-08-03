using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonEnum
    {
        /// <summary>
        /// ajax数据返回状态
        /// </summary>
        public static class AjaxResult
        {
            public static int SUCCESS= 1;
            public static int ERROR = 0;
        }
        /// <summary>
        /// 数据库数据逻辑存在状态
        /// </summary>
        public static class DataExist
        {
            public static int EXIST = 1;
            public static int NOT_EXIST = 0;
        }
        /// <summary>
        /// 用户注册id状态
        /// </summary>
        public static class UserIDState
        {
            public static int LEGAL = 0;
            public static int EXIST = 1;
            public static int UNLEGAL = 2;
        }
        /// <summary>
        /// 用户性别
        /// </summary>
        public static class UserGender
        {
            public static int FAMALE = 0;
            public static int MALE = 1;
        }
        /// <summary>
        /// 用户信息字段默认值
        /// </summary>
        public static class UserDefaultParas
        {
            public static string DEFAULT_PASSWORD = "000000";
        }
    }
}
