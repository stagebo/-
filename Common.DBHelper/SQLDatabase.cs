using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.SqlCommand;

namespace Common.DBHelper
{
    public class SQLDatabase : IDatabase
    {
        #region 构造和析构
        public SQLDatabase()
        {

        }
        public void Dispose()
        {
            this.Dispose();
        }

        #endregion

        #region public method
        public int ExecuteSql(string sql)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectstring()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet Query(string sql)
        {
            throw new NotImplementedException();
        }

        public DataTable QueryTable(string sql)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectstring()))
            {
                DataTable table = new DataTable();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(sql, connection);
                    command.Fill(table);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return table;
            }
        }
        #endregion public method

        #region private method
        public string GetConnectstring()
        {
            return "Data Source=127.0.0.1;Initial Catalog=BlogSystem;Persist Security Info=True;User ID=sa;pwd =st";
        }

        #endregion private method

        #region field
        public IDbConnection conn;
        #endregion 
    }
}
