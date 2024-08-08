using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessLayer
{
    public interface IQuery
    {
        string QueryString { get; }
        IEnumerable<string> SelectedColumns { get; set; }
        KeyValuePair<string, object?> WhereConditionColVal { get; set; }
    }
    public enum SortOrders
    {
        ASCENDING,
        DESCENDING
    }

    public class QueryBuilder : IQuery
    {
        readonly StringBuilder queryString = new StringBuilder();
        public string QueryString { get { return queryString.ToString(); } }
        public IEnumerable<string> SelectedColumns { get; set; }
        public KeyValuePair<string, object?> WhereConditionColVal { get; set; }

        public QueryBuilder()
        {

        }

        public QueryBuilder(string query)
        {
            queryString.Clear();
            queryString.Append(query);
        }

        public QueryBuilder Select(IEnumerable<string> columnOutput = null)
        {
            SelectedColumns = columnOutput;

            queryString.Append($"SELECT {(columnOutput == null ? "*" : string.Join(',', columnOutput))} ");
            return this;
        }

        public QueryBuilder Select(string selectString)
        {
         
            queryString.Append($"SELECT {selectString} ");
            return this;
        }

        public QueryBuilder From(string tableName)
        {
            queryString.Append($"FROM {tableName} ");
            return this;
        }

        public QueryBuilder Limit(int offset, int count)
        {
            queryString.Append($"LIMIT {offset},{count} ");
            return this;
        }

        public QueryBuilder OrderBy(Dictionary<string, SortOrders> sorting)
        {

            queryString.Append($"ORDER BY ");

            StringBuilder orderBuilder = new StringBuilder();

            foreach (var k in sorting)
            {
                orderBuilder.Append($"{k.Key} {(k.Value == SortOrders.ASCENDING ? "ASC" : "DESC")}, ");
            }

            string trimmed = orderBuilder.ToString().Trim(' ').TrimEnd(',');
            queryString.Append(trimmed).Append(' ');


            return this;
        }

        public QueryBuilder Where(string columnName, object columnValue)
        {
            queryString.Append($"WHERE {columnName} = @{columnName} ");
            WhereConditionColVal = new KeyValuePair<string, object?>(columnName, columnValue);
            return this;
        }

        public QueryBuilder Where(string columnName, string compOperator, object columnValue)
        {
            queryString.Append($"WHERE {columnName} {compOperator} @{columnName} ");
            WhereConditionColVal = new KeyValuePair<string, object?>(columnName, $"{compOperator} {columnValue}");
            return this;
        }

        public static QueryBuilder FullTextSearch(string tableName, string columnName, string searchText, int offset = 0, int count = 20)
        {
            //StringBuilder orderBuilder = new StringBuilder();
            string query = $"SELECT * FROM {tableName} WHERE MATCH ({columnName}) AGAINST ('{searchText}' IN NATURAL LANGUAGE MODE) LIMIT {offset}, {count}";


            return new QueryBuilder(query);
            
        }

        public override string ToString()
        {
            return queryString.ToString();
        }
    }
}
