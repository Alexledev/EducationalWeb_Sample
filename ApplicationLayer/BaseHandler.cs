using Google.Protobuf.WellKnownTypes;
using Infrastructure.DataAccessLayer;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public abstract class BaseHandler<T> : IDisposable where T : class, new()
    {
        private readonly DBContext<T> dataHandler;

        public string TableName { get; private set; }

        public BaseHandler(string tableName)
        {
            this.TableName = tableName;
            this.dataHandler = new DBContext<T>(TableName);
        }

        public Task<List<T>> GetDataCollection(int offset = 0, int count = 20, IEnumerable<string> outputColumn = null)
        {
            QueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Select(outputColumn).From(this.TableName).Limit(offset, count);

            return dataHandler.GetCollectionData(queryBuilder);
        }

        public Task<(List<T> items, Int64 totalItemCount)> GetCollectionDataWithCount(int offset = 0, int count = 20, IEnumerable<string> outputColumn = null)
        {
            QueryBuilder queryDataBuilder = new QueryBuilder();
            queryDataBuilder.Select(outputColumn).From(this.TableName).Limit(offset, count);

            QueryBuilder queryScalarBuilder = new QueryBuilder();
            queryScalarBuilder.Select("COUNT(*)").From(this.TableName);

            return dataHandler.GetCollectionDataWithCount(queryDataBuilder, queryScalarBuilder);
        }

        public async Task<IDictionary<object, object>> GetCount(IEnumerable<KeyValuePair<string, object>> columnAndValue)
        {        
            List<IQuery> queries = new List<IQuery>();

            foreach (KeyValuePair<string, object> cav in columnAndValue)
            {
                QueryBuilder queryScalarBuilder = new();

                queryScalarBuilder.Select($"COUNT({cav.Key})").From(this.TableName).Where(cav.Key, cav.Value);

                queries.Add(queryScalarBuilder);
            }           

            return await dataHandler.GetCount(queries);
        }


        public Task<List<T>> GetDataCollectionOrdered(Dictionary<string, SortOrders> order, int offset = 0, int count = 20, IEnumerable<string> outputColumn = null)
        {
            QueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Select(outputColumn).From(this.TableName).OrderBy(order).Limit(offset, count);

            return dataHandler.GetCollectionData(queryBuilder);
        }

        public Task<List<T>> GetItemsByColumn(string columnName, object columnValue, int offset = 0, int count = 20, IEnumerable<string> outputColumn = null)
        {
            QueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Select(outputColumn).From(this.TableName).Where(columnName, columnValue).Limit(offset, count);

            return dataHandler.GetCollectionData(queryBuilder);
        }

        public Task<List<T>> GetItemsByColumnOrdered(string columnName, object columnValue, Dictionary<string, SortOrders> order, int offset = 0, int count = 20, IEnumerable<string> outputColumn = null)
        {
            QueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Select(outputColumn).From(this.TableName).Where(columnName, columnValue).OrderBy(order).Limit(offset, count);

            return dataHandler.GetCollectionData(queryBuilder);
        }

        protected Task<List<T>> FullTextSearchWithColumn(string searchColumn, string searchText)
        {
            IQuery searchQuery = QueryBuilder.FullTextSearch(this.TableName, searchColumn, searchText);
            return dataHandler.GetCollectionData(searchQuery);
        }

        public virtual Task InsertData(T entity)
        {
            return dataHandler.InsertData(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="idColumnName">The Column name and Value in the *entity* that is used in the WHERE statement</param>
        /// <returns></returns>
        public Task UpdateData(T entity, string idColumnName)
        {
            return dataHandler.UpdateData(entity, idColumnName);
        }

        public Task DeleteData(string columnName, object value)
        {
            return dataHandler.DeleteData(columnName, value);
        }

        public void Dispose()
        {
            dataHandler.Dispose();
        }
    }
}
