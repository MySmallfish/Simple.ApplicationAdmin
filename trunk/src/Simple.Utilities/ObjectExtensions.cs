using System;
using System.IO;
using System.Runtime.Serialization;
using Simple.Utilities;

namespace Simple
{
    public static class ObjectExtensions
    {
        public static TObject As<TObject>(this object instance)
            where TObject :class 
        {
            if (!instance.Is<TObject>())
            {
                throw new ArgumentException(
                    "Specified instance, of type '{0}', must be derivative '{1}'.".ParseTemplate(
                        (instance.IsNull() ? "null" : instance.GetType().Name), typeof (TObject)));
            }

            var result = instance as TObject;
            
            return result;
        }

        public static bool IsNot<TObject>(this object instance)
        {
            return !instance.Is<TObject>();
        }

        public static bool Is<TObject>(this object instance)
        {
            return instance is TObject;
        }

        public static bool IsNull<TObject>(this TObject value)
            where TObject: class
        {
            return value == null;
        }

        public static bool IsDbNull(this object value)
        {
            return Convert.IsDBNull(value);
        }

        public static bool IsNotDbNull(this object value)
        {
            return !value.IsDbNull();
        }

        public static bool IsNotNull<TObject>(this TObject value)
            where TObject : class
        {
            return !value.IsNull();
        }

        public static TObject Once<TObject>(this TObject target, Func<TObject> initialize)
            where TObject : class
        {
            if (target.IsNull())
            {
                return initialize();
            }

            return target;
        }

        public static void WhenNotNull<TObject>(this TObject target, Action<TObject> whenNotNull)
            where TObject : class
        {
            if (target.IsNotNull())
            {
                whenNotNull(target);
            }
        }

        public static TOut ValueOrDefault<TObject, TOut>(this TObject target, Func<TObject, TOut> getValue, Func<TOut> getDefaultValue)
                where TObject : class
        {
            Requires.ArgumentNotNull(getDefaultValue, "getDefaultValue");
            if (target.IsNull())
            {
                return getDefaultValue();    
            }

            return getValue(target);
            
        }

        public static TOut ValueOrDefault<TObject,TOut>(this TObject target, Func<TObject,TOut> getValue)
            where TObject: class
            
        {
            return target.ValueOrDefault(getValue, () => default(TOut));
        }

        public static object ValueOrDBNull(this object value)
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

        public static void RaiseEvent(this object source, EventHandler handler)
        {
            Requires.ArgumentNotNull(source, "source");

            if (handler != null)
            {
                handler(source, EventArgs.Empty);
            }
        }

        public static T Clone<T>(this T source)
            where T : class
        {
            var result = default(T);
            if (source.IsNotNull())
            {
                var serializer = new NetDataContractSerializer();
                using (var memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, source);
                    memoryStream.Flush();
                    memoryStream.MoveToStart();
                    result = serializer.ReadObject(memoryStream) as T;
                }
            }
            return result;
        }

        public static TDataContract CloneDataContract<TDataContract>(this TDataContract dataContract)
            where TDataContract : class
        {
            if (!typeof(TDataContract).IsDefined(typeof(DataContractAttribute), true))
            {
                throw new ArgumentException("This extension method works only on DataContract types");
            }

            var result = default(TDataContract);

            if (dataContract != null)
            {
                var xmlClone = DataHelper.MapToXml(dataContract);
                result = DataHelper.MapFromXml<TDataContract>(xmlClone);
            }

            return result;
        }
    }
}
