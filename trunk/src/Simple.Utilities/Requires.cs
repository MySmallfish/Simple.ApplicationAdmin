using System;
using System.Linq;
using System.IO;
using System.Globalization;

namespace Simple.Utilities
{
    public static class Requires
    {
        
        public static void NotNull(object instance, string message)
        {
            NotNull<NullReferenceException>(instance, message);
        }

        

        public static void NotNull<TException>(object instance, string message)
            where TException : Exception
        {
            if (instance.IsNull())
            {
                var exception = Activator.CreateInstance(typeof(TException), message) as TException;
                
                throw exception;
            }
        }

        /// <summary>
        /// Check if a folder exists, if not, throw <see cref="DirectoryNotFoundException"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="argName"></param>
        public static void EnsureFolderExists(string path, string argName)
        {

            Requires.ArgumentNotNullOrEmptyString(path, argName);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(ErrorMessages.ArgumentFolderDoesNotExist, path, argName));
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.StringCannotBeEmpty, argumentName));
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }


        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignee">The argument type.</param>
        /// <param name="providedType">The type it must be assignable from.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void TypeIsAssignableFromType(Type assignee, Type providedType, string argumentName)
        {
            if (!providedType.IsAssignableFrom(assignee))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    ErrorMessages.TypeNotCompatible, assignee, providedType), argumentName);
        }

        /// <summary>
        /// Ensures that <c>int</c> argument is positive
        /// </summary>
        /// <param name="argValue"></param>
        /// <param name="argName"></param>
        public static void IntArgumentPositive(int argValue, string argName)
        {
            if (argValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argName, ErrorMessages.IntArgumentMustBePositive);
            }
        }

        /// <summary>
        /// Ensures that <c>long</c> argument is positive
        /// </summary>
        /// <param name="argValue"></param>
        /// <param name="argName"></param>
        public static void LongArgumentPositive(long argValue, string argName)
        {
            if (argValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argName, ErrorMessages.IntArgumentMustBePositive);
            }
        }


        /// <summary>
        /// Checks a guid argument to ensure it isn't Guid.Empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentGuidIsNotEmpty(Guid argumentValue, string argumentName)
        {
            if (argumentValue.Equals(Guid.Empty))
            {
                throw new ArgumentException(ErrorMessages.GuidArgumentMustNotBeEmpty, argumentName);
            }
        }

        /// <summary>
        /// Ensure that <paramref name="path"/> is exists
        /// </summary>
        /// <param name="path">
        ///     path to ensure
        /// </param>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        public static void DirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(ErrorMessages.DirectoryNotFound, path));
            }
        }

        /// <summary>
        /// Ensure a file exists
        /// </summary>
        /// <param name="path">file path</param>
        /// <exception cref="System.IO.FileNotFoundException"/>
        public static void FileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(ErrorMessages.FileNotFound, path));
            }
        }

        /// <summary>
        /// Ensure a file exists
        /// </summary>
        /// <param name="path">file path</param>
        /// <exception cref="System.IO.FileNotFoundException"/>
        public static void FileArgumentExists(string path, string argumentName)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(ErrorMessages.ArgumentFileNotFound, path, argumentName));
            }
        }

        public static void FileArgumentValid(string path, string argumentName)
        {
            Requires.ArgumentNotNullOrEmptyString(path, argumentName);
            var fileName = Path.GetFileName(path);
            Requires.ArgumentNotNullOrEmptyString(fileName, argumentName);

            var index = 0;
            var invalidChars = Path.GetInvalidFileNameChars();
            while (index < fileName.Length && !invalidChars.Contains(fileName[index++])) ;
            if (index != fileName.Length)
            { 
                throw new ArgumentException(string.Format("FileName '{0}' for argument '{1}' is invalid. File names cannot conatin these chars: {2}", path, argumentName, new string(invalidChars)));
            }
        }

  /// <summary>
        /// Checks if an array contains at least 1 items, if not throws <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="connectionName"></param>
        public static void AtLeastOneItemInArray(Array array, string argumentName)
        {
            if (array.IsNull() || array.Length == 0)
            {
                throw new ArgumentException(ErrorMessages.ArrayMustContainAtLeastOneItem , argumentName);
            }
        }
    }
}
