using Common.Log4Net;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NHibernate.SqlCommand;

namespace Common.NHibernate
{
    public class SessionManager01
    {
        private static ISessionFactory _sessionFactory;

        static SessionManager01()
        {
            try
            {
                Configuration cfg = new Configuration();
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
                _sessionFactory = cfg.BuildSessionFactory();
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.NHibernate.SessionManager01", "数据库连接失败", exception);
            }
        }

        private static ISession m_session = null;
        public static ISession OpenSession()
        {
            try
            {
                if (m_session == null)
                    m_session = _sessionFactory.OpenSession();
                if (!m_session.IsOpen)
                    m_session.Reconnect();
                return m_session;
            }
            catch (Exception ex)
            {
                return null;
            }
            //try
            //{
            //    return _sessionFactory.OpenSession(new ShowSQLInterceptor());
            //}
            //catch (Exception exception)
            //{
            //    LogUtils.Error("Common.NHibernate.SessionManager01", "NHibernate Session打开失败", exception);
            //    return null;
            //}
        }
        public static ISession OpenSession(IInterceptor r)
        {
            try
            {
                if (m_session == null)
                    m_session = _sessionFactory.OpenSession(r);
                if (!m_session.IsOpen)
                    m_session.Reconnect();
                return m_session;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void CloseSession(ISession session)
        {
            if (session == null)
            {
                return;
            }
            try
            {
                session.Close();
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.NHibernate.SessionManager01", "NHibernate Session关闭失败", exception);
            }
        }

        public static void DisposeTransaction(ITransaction transaction)
        {
            if (transaction == null)
            {
                return;
            }
            try
            {
                transaction.Dispose();
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.NHibernate.SessionManager01", "NHibernate Transaction关闭失败", exception);
            }
        }

        public static Dictionary<string, object> GetEntityTableMetaInfo(string entityName)
        {
            string binPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string webFileName = binPath + @"bin\Mapping\" + entityName + ".hbm.xml";
            string appFileName = binPath + @"Mapping\" + entityName + ".hbm.xml";
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists(webFileName))
                {
                    xmlDoc.Load(webFileName);
                }
                else if (File.Exists(appFileName))
                {
                    xmlDoc.Load(appFileName);
                }
                else
                {
                    return null;
                }
                XmlElement element = (XmlElement)xmlDoc.GetElementsByTagName("class")[0];
                string tableName = element.GetAttribute("table");

                List<Dictionary<string, string>> entityPropertyList = new List<Dictionary<string, string>>();
                element = (XmlElement)xmlDoc.GetElementsByTagName("id")[0];
                entityPropertyList.Add(new Dictionary<string, string>()
                {
                    {"name", element.GetAttribute("name")},
                    {"column", element.GetAttribute("column")},
                    {"type", element.GetAttribute("type")}
                });
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("property");
                foreach (XmlNode node in nodeList)
                {
                    element = (XmlElement)node;
                    entityPropertyList.Add(new Dictionary<string, string>()
                    {
                        {"name", element.GetAttribute("name")},
                        {"column", element.GetAttribute("column")},
                        {"type", element.GetAttribute("type")}
                    });
                }

                return new Dictionary<string, object>()
                {
                    {"tableName", tableName},
                    {"entityPropertyList", entityPropertyList}
                };
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.NHibernate.SessionManager01", "获取实体对应数据表结构信息失败，entityName：" + entityName, exception);
                return null;
            }
        }
    }
} 