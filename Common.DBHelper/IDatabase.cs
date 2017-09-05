using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DBHelper
{
    interface IDatabase:IDisposable
    {
        int ExecuteSql(string sql);

        DataSet Query(string sql);

        DataTable QueryTable(string sql);
        
    }
}
