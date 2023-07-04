using Manager.Infrastructure.Database;
using Manager.Infrastructure.IRepositoies;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace Manager.Infrastructure.Repositoies
{
    public class ProcedureRepository : IProcedure
    {
        private readonly H5Context db;

        public ProcedureRepository(H5Context db)
        {
            this.db = db;
        }

        public async Task<IList<T>> ExecSpAsync<T>(string sql, MySqlParameter[]? mySqlParameters = null) where T : new()
        {
            try
            {
                var connection = db.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    db.Database.OpenConnection();
                }
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (mySqlParameters != null) cmd.Parameters.AddRange(mySqlParameters);
                var dr = await cmd.ExecuteReaderAsync();
                var columnSchema = dr.GetColumnSchema();
                var data = new List<T>();
                T model;
                while (await dr.ReadAsync())
                {
                    model = new T();
                    foreach (var kv in columnSchema)
                    {
                        //if (kv.ColumnOrdinal.HasValue)
                        //{
                        foreach (var item in typeof(T).GetProperties())
                        {
                            if (item.Name == kv.ColumnName)
                            {
                                model.GetType().GetProperty(item.Name).SetValue(model, ConvertTo(dr.GetValue(kv.ColumnOrdinal.Value), item.PropertyType));
                                break;
                            }
                        }
                        // }
                    }
                    data.Add(model);
                }
                dr.Dispose();
                return data;
            }
            catch (Exception ex)
            {
                Log.Error("ExecSpAsync : {0}", ex);
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>> ExecSqlAsync(string sql, MySqlParameter[]? mySqlParameters = null)
        {
            try
            {
                var connection = db.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    db.Database.OpenConnection();
                }
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                if (mySqlParameters != null) cmd.Parameters.AddRange(mySqlParameters);

                DbDataReader dr = await cmd.ExecuteReaderAsync();
                var list = new List<DynamicEntity>();
                while (await dr.ReadAsync())
                {
                    DynamicEntity entity = new();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        entity.SetMember(dr.GetName(i), dr.GetValue(i));
                    }
                    list.Add(entity);
                }
                dr.Dispose();
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("ExecSpAsync dynamic : {0}", ex);
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>> ExecSpAsync(string sql, MySqlParameter[]? mySqlParameters = null)
        {
            try
            {
                var connection = db.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    db.Database.OpenConnection();
                }
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (mySqlParameters != null) cmd.Parameters.AddRange(mySqlParameters);

                DbDataReader dr = await cmd.ExecuteReaderAsync();
                var list = new List<DynamicEntity>();
                while (await dr.ReadAsync())
                {
                    DynamicEntity entity = new();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        entity.SetMember(dr.GetName(i), dr.GetValue(i));
                    }
                    list.Add(entity);
                }
                dr.Dispose();
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("ExecSpAsync dynamic :{0}", ex);
                return null;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="convertibleValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        internal static object ConvertTo(object convertibleValue, Type type)
        {
            if (!type.IsGenericType)
            {
                return Convert.ChangeType(convertibleValue, type);
            }
            else
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(type));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, type.FullName));
        }

        public IEnumerable<dynamic> ExecSpSync(string sql, MySqlParameter[]? mySqlParameters = null)
        {
            try
            {
                var connection = db.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    db.Database.OpenConnection();
                }
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (mySqlParameters != null) cmd.Parameters.AddRange(mySqlParameters);

                DbDataReader dr = cmd.ExecuteReader();
                var list = new List<DynamicEntity>();
                while (dr.Read())
                {
                    DynamicEntity entity = new();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        entity.SetMember(dr.GetName(i), dr.GetValue(i));
                    }
                    list.Add(entity);
                }
                dr.Dispose();
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("ExecSpSync dynamic :{0}", ex);
                return null;
            }
        }

        public IEnumerable<dynamic> ExecSqlSync(string sql, MySqlParameter[]? mySqlParameters = null)
        {
            try
            {
                var connection = db.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    db.Database.OpenConnection();
                }
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                if (mySqlParameters != null) cmd.Parameters.AddRange(mySqlParameters);

                DbDataReader dr = cmd.ExecuteReader();
                var list = new List<DynamicEntity>();
                while (dr.Read())
                {
                    DynamicEntity entity = new();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        entity.SetMember(dr.GetName(i), dr.GetValue(i));
                    }
                    list.Add(entity);
                }
                dr.Dispose();
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("ExecSqlSync dynamic : {0}", ex);
                return null;
            }
        }

        /// <summary>
        /// 动态实体
        /// </summary>
        internal class DynamicEntity : DynamicObject
        {
            /// <summary>
            /// 属性和值的字典表
            /// </summary>
            private readonly Dictionary<string, object> values = new(StringComparer.OrdinalIgnoreCase);

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (values.ContainsKey(binder.Name))
                {
                    result = values[binder.Name];
                }
                else
                {
                    throw new System.MissingMemberException($"The property {binder.Name} does not exis");
                }
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                SetMember(binder.Name, value);
                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return values.Keys;
            }

            internal void SetMember(string propertyName, object value)
            {
                if (object.ReferenceEquals(value, DBNull.Value))
                {
                    values[propertyName] = null;
                }
                else
                {
                    values[propertyName] = value;
                }
            }
        }
    }
}