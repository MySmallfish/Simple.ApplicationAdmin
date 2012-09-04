using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Data.Common;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using ServiceModelEx;

namespace Simple.Utilities
{
    public static class DataHelper
    {
        public static DateTime DateTimeParameterValue(DateTime dateTime)
        {
            return dateTime.ToUniversalTime();
        }

        public static object DateTimeParameterValue(DateTime? dateTime)
        {
            return DataHelper.ValueOrDBNull(dateTime.HasValue ? DateTimeParameterValue(dateTime.Value) : default(DateTime?));
        }

        public static DateTime DateAsUtc(DateTime source)
        {
            return DateTime.SpecifyKind(source, DateTimeKind.Utc);
        }
        public static DateTime? DateAsUtc(DateTime? source)
        {
            var result = default(DateTime?);
            if (source.HasValue)
            {
                result = DateAsUtc(source.Value);
            }
            return result;
        }
        public static TDataContract FromJson<TDataContract>(this string value)
        {
            var result = default(TDataContract);

            if (value.NotNullOrEmpty())
            {

                var serializer = new DataContractJsonSerializer(typeof(TDataContract));
                using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(value)))
                {
                    result = (TDataContract)serializer.ReadObject(memoryStream);
                }

            }
            return result;
        }

        public static string ToJson(this object value)
        {
            var result = default(string);
            if (value != null)
            {
                var serializer = new DataContractJsonSerializer(value.GetType());
                using (var memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, value);
                    memoryStream.Flush();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }
            return result;
        }


        public static XElement MapToXml<TDataContract>(TDataContract value)
            where TDataContract : class
        {
            var result = default(XElement);
            if (value != null)
            {
                var name = GetDataContractTypeName<TDataContract>();
                result = new XElement(name);
                var dataContractSerializer = new DataContractSerializer<TDataContract>();
                using (var writer = result.CreateWriter())
                {
                    dataContractSerializer.WriteObject(writer, value);
                    writer.Flush();
                }

            }
            return result;
        }

        private static string GetDataContractTypeName<TDataContract>()
        {
            var name = typeof (TDataContract).Name;
            if (typeof(TDataContract).IsArray)
            {
                name = "ArrayOf" + typeof (TDataContract).Name.Replace("[]", string.Empty);
            }
            return name;
        }

        public static TTargetDataContract MapFromXml<TTargetDataContract>(XElement element)
            where TTargetDataContract : class
        {
            var result = default(TTargetDataContract);
            if (element != null)
            {
                var dataContractSerializer = new DataContractSerializer<TTargetDataContract>();
                var valueNode = element.FirstNode;
                if (valueNode.IsNotNull())
                {
                    using (var reader = valueNode.CreateReader())
                    {
                        result = dataContractSerializer.ReadObject(reader);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get safe value, if value not exists (DBNull) return the default(EntityName)
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TType GetSafeValue<TType>(object value)
        {
            return GetSafeValue(value, default(TType));
        }

        /// <summary>
        /// Get safe value, if value not exists (DBNull) return the default value
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TType GetSafeValue<TType>(object value, TType defaultValue)
        {
            if (value.IsNull() || value.IsDbNull())
            {
                return defaultValue;
            }
            else
            {
                if (value is DateTime)
                {
                    value = ((DateTime)value).ToUniversalTime().ToLocalTime();
                }


                return (TType)value;
            }
        }

        /// <summary>
        /// Get string value or an empty string if value not exists
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValueOrEmptyString(object value)
        {
            return GetSafeValue<string>(value, string.Empty);
        }

        public static void TraceDataReader(DbDataReader reader)
        {
            int resultSet = 0;
            do
            {
                Trace.WriteLine("---------------------------------------------------------");
                Trace.WriteLine(resultSet);
                Trace.WriteLine("---------------------------------------------------------");

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Trace.Write(reader.GetName(i));
                        Trace.Write(" , ");
                    }
                    Trace.WriteLine("");
                }
                resultSet++;
            } while (reader.NextResult());
        }

        public static void AddParameter(System.Data.Common.DbCommand command, string paramName, System.Data.DbType dbType)
        {
            AddParameter(command, paramName, dbType, 0);
        }

        public static void AddParameter(System.Data.Common.DbCommand command, string paramName, System.Data.DbType dbType, int size)
        {
            AddParameter(command, paramName, dbType, size, ParameterDirection.Input);
        }

        public static void AddParameter(System.Data.Common.DbCommand command, string paramName, System.Data.DbType dbType, int size, ParameterDirection direction)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Direction = direction;
            command.Parameters.Add(parameter);
        }

        public static void AddParameterWithValue(System.Data.Common.DbCommand command, string paramName, System.Data.DbType dbType, int size, ParameterDirection direction, object value, bool handleDBNull)
        {
            if (handleDBNull)
            {
                value = ValueOrDBNull(value);
            }
            AddParameterWithValue(command, paramName, dbType, size, direction, value);
        }

        public static void AddParameterWithValue(System.Data.Common.DbCommand command, string paramName, DbType dbType, object value)
        {
            AddParameterWithValue(command, paramName, dbType, 0, ParameterDirection.Input, value);
        }

        public static void AddParameterWithValue(System.Data.Common.DbCommand command, string paramName, System.Data.DbType dbType, int size, ParameterDirection direction, object value)
        {
            AddParameter(command, paramName, dbType, size, direction);
            var parameter = command.Parameters[paramName];
            parameter.Value = value;
        }

        public static string GetLikeString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return string.Format("%{0}%", text);
        }

        public static object StringValueOrDBNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        public static object ValueOrDBNull(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
    }
}
