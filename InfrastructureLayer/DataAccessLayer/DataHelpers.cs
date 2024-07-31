using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessLayer
{
    public static class DataHelpers
    {
        private static Dictionary<Type, MySqlDbType> types = new()
        {
            {typeof(string),  MySqlDbType.String},
             {typeof(int),  MySqlDbType.Int32},
             {typeof(uint),  MySqlDbType.UInt32},
             {typeof(DateOnly),  MySqlDbType.Date},
             {typeof(DateTime),  MySqlDbType.DateTime},
             {typeof(ulong),  MySqlDbType.UInt64},
             {typeof(double),  MySqlDbType.Double},
             {typeof(decimal),  MySqlDbType.Decimal},
             {typeof(System.Single),  MySqlDbType.Float},
             {typeof(bool),  MySqlDbType.Bit},

        };

        internal static Dictionary<string, (object? value, MySqlDbType valueType)> GetPropertyTuple<T>(T entity) where T : class
        {
            Dictionary<string, (object? value, MySqlDbType valueType)> nameAndValue = new();

            var entityType = entity.GetType();
            var props = entityType.GetProperties();

            foreach (var prop in props)
            {
                var type = prop.PropertyType;

                if (types.TryGetValue(type, out var sqlType))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), sqlType));
                }                
                else
                {
                    throw new FieldAccessException(message: $"Datatype {type} not supported");
                }
            }

            return nameAndValue;
        }

        internal static Dictionary<string, (object? value, MySqlDbType valueType)> GetPropertyTupleOld<T>(T entity) where T : class
        {
            Dictionary<string, (object? value, MySqlDbType valueType)> nameAndValue = new();

            var entityType = entity.GetType();
            var props = entityType.GetProperties();

            foreach (var prop in props)
            {
                var type = prop.PropertyType;
                if (type == typeof(string))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.String));
                }
                else if (type == typeof(int))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Int32));
                }
                else if (type == typeof(uint))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.UInt32));
                }
                else if (type == typeof(DateOnly))
                {
                    //string formattedDate = DateOnly.Parse(prop.GetValue(entity).ToString()).ToString("yyyy-MM-dd");
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Date));
                }
                else if (type == typeof(DateTime))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.DateTime));
                }
                else if (type == typeof(ulong))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.UInt64));
                }
                else if (type == typeof(double))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Double));
                }
                else if (type == typeof(decimal))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Decimal));
                }
                else if (type == typeof(System.Single))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Float));
                }
                else if (type == typeof(bool))
                {
                    nameAndValue.Add(prop.Name, (prop.GetValue(entity), MySqlDbType.Bit));
                }
                else
                {
                    throw new FieldAccessException(message: "Datatype not supported");
                }
            }

            return nameAndValue;
        }

        internal static object? GetValueByPropertyName<T>(T entity, string propertyName)
        {
            var entityType = entity.GetType();
            var props = entityType.GetProperties();
            var p = props.SingleOrDefault(prop => prop.Name == propertyName);

            return p?.GetValue(entity);
        }

        internal static T CreateValueObject<T>(DbDataReader reader, IEnumerable<string> columnOutput) where T : class, new()
        {
            T obj = new T();
            Type type = obj.GetType();

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (columnOutput != null && !columnOutput.Contains(prop.Name))
                    continue;

                SetValueByType<T>(reader, prop, obj);
            }

            return obj;
        }

        private static void SetValueByType<T>(DbDataReader reader, PropertyInfo propInfo, T obj) where T : class, new()
        {
            if (propInfo.PropertyType == typeof(DateOnly))
            {
                var val = DateOnly.FromDateTime(Convert.ToDateTime(reader[propInfo.Name].ToString()));
                propInfo.SetValue(obj, val, null);
                return;
            }
            propInfo.SetValue(obj, reader[propInfo.Name], null);
        }
    }
}
