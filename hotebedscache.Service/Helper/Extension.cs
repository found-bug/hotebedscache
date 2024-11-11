using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Hotebedscache.Service.Helper
{
    public static class Extension
    {
        public static string DataTableToJson(DataTable dataTable)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {                        
                    if (col.DataType == typeof(int))
                        row.Add(col.ColumnName, dr[col] == DBNull.Value ? 0 : dr[col]);
                    else 
                        row.Add(col.ColumnName, dr[col] == DBNull.Value ? "" : dr[col]);
                }
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }
    }
}
