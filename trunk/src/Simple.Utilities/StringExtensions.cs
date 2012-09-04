using System;
using System.Linq;
using System.Text;

namespace Simple.Utilities
{
    public static class StringExtensions
    {
        
        public static bool IsImageMimeType(this string mimeType)
        {
            return mimeType.ToLower().StartsWith("image/");
        }

        public static string ComputeHash(this string value)
        {
            Requires.ArgumentNotNull(value, "value");

            var buffer = Encoding.UTF8.GetBytes(value);
            buffer = System.Security.Cryptography.SHA512Managed.Create().ComputeHash(buffer);
            return Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
        }

        public static string IfNullOrEmptyTake(this string source, string other)
        {
            var result = source;
            if (source.IsNullOrEmpty())
            {
                result = other;
            }
            return result;
        }

        public static string WithoutSpaces(this string source)
        {
            if (source.NotNullOrEmpty())
            {
                return source.Replace(" ", "");
            }

            return source;
        }

        public static bool NotIn(this string valueToFind, params string[] listOfValues)
        {
            return !valueToFind.In(listOfValues);
        }

        public static bool NotIn(this string valueToFind, bool ignoreCase, params string[] listOfValues)
        {
            return !valueToFind.In(ignoreCase, listOfValues);
        }

        public static bool In(this string valueToFind,params string[] listOfValues)
        {
            return In(valueToFind, false, listOfValues);
        }

        public static bool In(this string valueToFind, bool ignoreCase, params string[] listOfValues)
        {
            Requires.ArgumentNotNullOrEmptyString(valueToFind, "valueToFind");
            Requires.ArgumentNotNull(listOfValues, "listOfValues");

            Predicate<string> match =
                value => ignoreCase ? value.CaseInsensitiveEquals(valueToFind) : value == valueToFind;
            
            return
                (from value in listOfValues 
                 where match(value)
                 select true).FirstOrDefault();
        }

        public static int? ParseIntOrNull(this string value)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                return intValue;
            }

            return default(int?);
        }
        
        public static int ParseIntOrDefault(this string value)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                return intValue;
            }

            return default(int);
        }

        public static bool CaseInsensitiveEquals(this string my, string other)
        {
            return string.Compare(my, other, true) == 0;
        }

        public static string ParseTemplate(this string template, params object[] replacements)
        {
            return string.Format(template, replacements);
        }

        public static StringBuilder AppendLine(this StringBuilder builder, string format, params object[] replacements)
        {
            builder.AppendFormat(format, replacements);
            builder.AppendLine();
            return builder;
        }

        public static void RemoveTrailingChar(this StringBuilder stringBuilder)
        {
            RemoveTrailingChars(stringBuilder, 1);
        }

        public static void RemoveTrailingChars(this StringBuilder stringBuilder, int count)
        {
            if (stringBuilder.Length > count)
            {
                stringBuilder.Remove(stringBuilder.Length - count, count);
            }
        }

        public static void WhenNotNullOrEmpty(this string source, Action @do)
        {
            if (source.NotNullOrEmpty())
            {
                @do();
            }
        }

        public static bool NotNullOrEmptyTrimmed(this string source)
        {
            var result = false;
            if (source.NotNullOrEmpty() && source.Trim().NotNullOrEmpty())
            {
                result = true;
            }
            return result;
        }

        public static bool NotNullOrEmpty(this string source)
        {
            return !source.IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static string After(this string source, string start)
        {
            if (!string.IsNullOrEmpty(source) && source.Length > start.Length)
            {
                return source.Substring(start.Length);
            }

            return source;
        }

        public static string Until(this string source, char character)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var indexOfChar = source.IndexOf(character);
                if (indexOfChar >= 0)
                {
                    return source.Substring(0, indexOfChar);
                }
            }

            return source;
        }

        private const string CommaDelimiter = ",";

        public static bool FindInCommaDelimitedList(this string source, string part)
        {
            return source.FindInDelimitedList(part, CommaDelimiter);
        }

        public static bool FindInDelimitedList(this string source, string part, string delimiter)
        {
            Requires.ArgumentNotNull(source, "source");
            
            return source.ToLower().Append(delimiter).Contains(part.Append(delimiter));
        }

        public static string PrependWhenNotNullOrEmpty(this string source, string prefix)
        {
            if (source.NotNullOrEmpty())
            {
                return prefix + source;
            }

            return string.Empty;
        }

        public static string AppendWhenNotNullOrEmpty(this string source, string addition)
        {
            if (source.NotNullOrEmpty())
            {
                return source + addition;
            }

            return string.Empty;
        }

        public static string Append(this string source, string addition)
        {
            Requires.ArgumentNotNull(source, "source");

            return source + addition;
        }
    }
}
