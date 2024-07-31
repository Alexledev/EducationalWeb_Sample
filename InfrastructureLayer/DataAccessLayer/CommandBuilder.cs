using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessLayer
{
    public interface IInsert
    {
        public string InsertInto(string tableName, IEnumerable<string> columns);
        public string InsertInto(string tableName, Dictionary<string, object> columnAndValuePair);
    }

    public interface IDelete
    {
        public string Delete(string tableName, string _id);
    }

    public interface IUpdate
    {
        public string Update(string tableName, IEnumerable<string> columns, string _id);
    }

    public class CommandBuilder : IInsert, IDelete, IUpdate
    {
        public string InsertInto(string tableName, IEnumerable<string> columns)
        {
            StringBuilder commandString = new();

            commandString.Append($"INSERT INTO {tableName}({string.Join(',', columns)})");
            commandString.Append($"VALUES ({ParamBuilder(columns)})");

            return commandString.ToString();
        }

        private string ParamBuilder(IEnumerable<string> parameters)
        {
            List<string> ps = new List<string>();
            foreach (string p in parameters)
            {
                ps.Add($"@{p}");
            }

            return string.Join(',', ps);        
        }

        public string InsertInto(string tableName, Dictionary<string, object> columnAndValuePair)
        {
            return $"INSERT INTO {tableName}({string.Join(',', columnAndValuePair.Keys)}) VALUES ({string.Join(',', columnAndValuePair.Values)}) ";
        }

        public string Delete(string tableName, string _id)
        {
            return $"DELETE FROM {tableName} WHERE {_id} = @{_id}";
        }

        public string Update(string tableName, IEnumerable<string> columns, string _id)
        {
            return $"UPDATE {tableName} SET {ParamUpdateBuilder(columns, _id)} WHERE {_id} = @{_id}";
        }

        private string ParamUpdateBuilder(IEnumerable<string> columns, string _id)
        {
            List<string> ps = new List<string>();

            foreach (string p in columns)
            {
                if (p == _id)
                {
                    continue;
                }

                ps.Add($"{p} = @{p}");
            }

            return string.Join(',', ps);
        }


    }
}
