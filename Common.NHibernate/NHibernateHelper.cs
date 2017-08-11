using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Common.NHibernate
{
    public class NHibernateHelper
    {
        private static ISessionFactory sessionFactory=null;

        #region 初始化 生成SessionFactory，并配置上下文策略
        public static void Instance(string currentSessionContextClass)
        //public NHibernateHelper(string currentSessionContextClass)
        {
            Configuration cfg = new Configuration();//.Configure().SetProperty("current_session_context_class", currentSessionContextClass);
            string binPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string webFileName = binPath + @"bin\Config\NHibernate.cfg.xml";
            string appFileName = binPath + @"Config\NHibernate.cfg.xml";
            if (File.Exists(webFileName))
            {
                cfg.Configure(webFileName);
            }
            else if (File.Exists(appFileName))
            {
                cfg.Configure(appFileName);
            }
            else
            {
                throw new Exception("找不到NHibernate配置文件");
            }
            sessionFactory = cfg//new Configuration().Configure()
                .SetProperty("current_session_context_class", currentSessionContextClass)
                .BuildSessionFactory();
        } 
        #endregion

        #region Session在当前上下文的操作
        private static void BindContext()
        {
            if (!CurrentSessionContext.HasBind(sessionFactory))
            {
                CurrentSessionContext.Bind(sessionFactory.OpenSession());
            }
        }

        private static void UnBindContext()
        {
            if (CurrentSessionContext.HasBind(sessionFactory))
            {
                CurrentSessionContext.Unbind(sessionFactory);
            }
        }

        public static void CloseCurrentSession()
        {
            if (sessionFactory.GetCurrentSession().IsOpen)
            {
                sessionFactory.GetCurrentSession().Close();
            }
        }

        public static ISession GetCurrentSession()
        {
            BindContext();
            return sessionFactory.GetCurrentSession();
        } 
        #endregion

        #region 关闭SessionFactory（一般在应用程序结束时操作）
        public static void CloseSessionFactory()
        {
            UnBindContext();
            if (!sessionFactory.IsClosed)
            {
                sessionFactory.Close();
            }
        } 
        #endregion

        #region 打开一个新的Session
        public static ISession OpenSession()
        {
            return sessionFactory.OpenSession();
        } 
        #endregion

    }

}
