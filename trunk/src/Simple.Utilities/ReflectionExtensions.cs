using System;
using System.Reflection;

namespace Simple.Utilities
    
{
    public static class ReflectionExtensions
    {
                    /// <summary>
            /// Get the version of an assembly
            /// </summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static string Version(this Assembly assembly)
            {
                return assembly.Version(3);
            }

            /// <summary>
            /// Get the version of an assembly
            /// </summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static string Version(this Assembly assembly, int fieldsCount)
            {
                Requires.ArgumentNotNull(assembly, "assembly");
                Requires.IntArgumentPositive(fieldsCount, "fieldsCount");

                return assembly.GetName().Version.ToString(fieldsCount);
            }

            /// <summary>
            /// Run an action against all public properties of a type
            /// </summary>
            /// <param name="type">contextual Type</param>
            /// <param name="action">action to perform</param>
            public static void ForEachPublicProperty(Type type, Action<PropertyInfo> action)
            {
                type.ForEachPublicProperty(action, null);
            }

            /// <summary>
            /// Run an action against all public properties of a type
            /// </summary>
            /// <param name="type">contextual Type</param>
            /// <param name="action">action to perform</param>
            /// <param name="filterForAttribute">type of attribute to filter, null to take all public properties without filter</param>
            public static void ForEachPublicProperty(Type type, Action<PropertyInfo> action, Type filterForAttribute)
            {
                var properties = ReflectionHelper.GetPublicProperties(type);

                var index = 0;
                while (index < properties.Length)
                {
                    if (filterForAttribute == null ||
                        properties[index].IsDefined(filterForAttribute, true))
                    {
                        // run the action on each property matching the attribute
                        action(properties[index]);

                    }
                    index++;
                }
            }

            public static T GetDefinedAttribute<T>(this MemberInfo member)
                where T : System.Attribute
            {
                return member.GetDefinedAttribute<T>(true);
            }

            public static T GetDefinedAttribute<T>(MemberInfo member, bool inherit)
                where T : System.Attribute
            {
                return member.GetCustomAttributes(typeof(T), inherit)[0] as T;
            }

          
    }
}
