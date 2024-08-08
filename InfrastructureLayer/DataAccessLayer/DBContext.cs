using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessLayer
{
    public class ConnectionStringManager
    {
        private static string connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ApplicationException("connectionString can't be null");
                }

                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }
    }

    public class DBContext<T> : IDisposable where T : class, new()
    {
        public static string connectionString = ConnectionStringManager.ConnectionString;

        private readonly MySqlConnection connection;
        private readonly string tableName;

        public DBContext(string tableName)
        {
            this.connection = new MySqlConnection(connectionString);
            this.tableName = tableName;
        }

        public async Task<List<T>> GetCollectionData(IQuery query)
        {
            List<T> dataCollection = new List<T>();

            await connection.OpenAsync();

            using (MySqlCommand cmd = new MySqlCommand(query.QueryString, connection))
            {
                if (!string.IsNullOrEmpty(query.WhereConditionColVal.Key))
                {
                    cmd.Parameters.AddWithValue($"@{query.WhereConditionColVal.Key}", query.WhereConditionColVal.Value);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dataCollection.Add(DataHelpers.CreateValueObject<T>(reader, query.SelectedColumns));
                    }
                }
            }

            await connection.CloseAsync();

            return dataCollection;
        }
      
        
        public async Task<(List<T> items, Int64 totalItemCount)> GetCollectionDataWithCount(IQuery queryData, IQuery scalarQuery)
        {
            List<T> dataCollection = new List<T>();

            await connection.OpenAsync();

            using (MySqlCommand cmd = new MySqlCommand(queryData.QueryString, connection))
            {
                if (!string.IsNullOrEmpty(queryData.WhereConditionColVal.Key))
                {
                    cmd.Parameters.AddWithValue($"@{queryData.WhereConditionColVal.Key}", queryData.WhereConditionColVal.Value);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dataCollection.Add(DataHelpers.CreateValueObject<T>(reader, queryData.SelectedColumns));
                    }
                }
            }

            Int64 totalRowCount = 0;

            using (MySqlCommand cmd = new MySqlCommand(scalarQuery.QueryString, connection))
            {               

                if (!string.IsNullOrEmpty(scalarQuery.WhereConditionColVal.Key))
                {
                    cmd.Parameters.AddWithValue($"@{scalarQuery.WhereConditionColVal.Key}", scalarQuery.WhereConditionColVal.Value);
                }
                
                var rowCount = await cmd.ExecuteScalarAsync();
                totalRowCount = Convert.ToInt64(rowCount);
            }

            await connection.CloseAsync();

            return (dataCollection, totalRowCount);
        }

        public async Task<IDictionary<object, object>> GetCount(IEnumerable<IQuery> queryCollection)
        {
            Dictionary<object, object> dataCollection = new();

            await connection.OpenAsync();           

            foreach (var scalarQuery in queryCollection)
            {
                using (MySqlCommand cmd = new MySqlCommand(scalarQuery.QueryString, connection))
                {                   

                    if (!string.IsNullOrEmpty(scalarQuery.WhereConditionColVal.Key))
                    {
                        cmd.Parameters.AddWithValue($"@{scalarQuery.WhereConditionColVal.Key}", scalarQuery.WhereConditionColVal.Value);
                    }

                    var rowCount = await cmd.ExecuteScalarAsync();

                    dataCollection.Add(scalarQuery.WhereConditionColVal.Value!, rowCount!);
                }
            }

            await connection.CloseAsync();

            return dataCollection;
        }

        public async Task InsertData<T>(T entity) where T : class, new()
        {
            var propNames = DataHelpers.GetPropertyTuple(entity);

            IInsert command = new CommandBuilder();

            string commandString = command.InsertInto(tableName, propNames.Keys);

            await connection.OpenAsync();

            using (MySqlCommand cmd = new MySqlCommand(commandString, connection))
            {
                foreach (var item in propNames)
                {
                    cmd.Parameters.Add($"@{item.Key}", item.Value.valueType).Value = item.Value.value;
                }

                await cmd.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();

            return;
        }

        public async Task UpdateData<T>(T entity, string idColumnName) where T : class, new()
        {
            var propNames = DataHelpers.GetPropertyTuple(entity);

            IUpdate command = new CommandBuilder();

            string strCommand = command.Update(tableName, propNames.Keys, idColumnName);

            await connection.OpenAsync();

            using (MySqlCommand cmd = new MySqlCommand(strCommand, connection))
            {
                foreach (var item in propNames)
                {
                    if (item.Key == idColumnName)
                    {
                        continue;
                    }

                    cmd.Parameters.Add($"@{item.Key}", item.Value.valueType).Value = item.Value.value;
                }

                cmd.Parameters.AddWithValue($"@{idColumnName}", DataHelpers.GetValueByPropertyName(entity, idColumnName));

                await cmd.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }

        public async Task DeleteData(string idColumnName, object value)
        {
            IDelete command = new CommandBuilder();

            string delcommand = command.Delete(tableName, idColumnName);

            await connection.OpenAsync();

            using (MySqlCommand cmd = new MySqlCommand(delcommand, connection))
            {
                cmd.Parameters.AddWithValue($"@{idColumnName}", value);

                await cmd.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
