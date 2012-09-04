// © 2008 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Simple.Utilities
{
    public static class CollectionExtensions
    {
     
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.NotNullOrEmpty();
        }

        public static bool NotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        public static T Choose<T>(this IEnumerable<T> enumerable, Func<T> hasElements, Func<T> hasNoElements)
        {
            if (enumerable.Count() == 0)
            {
                return hasNoElements();
            }

            return hasElements();
        }

        public static void AsyncForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var waitHandles = new List<AutoResetEvent>();

            foreach (T item in collection)
            {
                var waitHandle = new AutoResetEvent(false);

                action(item);

                waitHandle.Set();
            }
            WaitHandle.WaitAll(waitHandles.ToArray());
        }
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (T item in collection)
            {
                action(item);
            }
        }
        //public static IEnumerable<U> ConvertAll<T, U>(this IEnumerable<T> collection, Converter<T, U> converter)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }
        //    if (converter == null)
        //    {
        //        throw new ArgumentNullException("converter");
        //    }
        //    foreach (T item in collection)
        //    {
        //        yield return converter(item);
        //    }
        //}
        //public static IEnumerable<T> Complement<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        //{
        //    foreach (T item in collection1)
        //    {
        //        if (collection2.Contains(item) == false)
        //        {
        //            yield return item;
        //        }
        //    }
        //}
        //public static IEnumerable<T> Except<T>(IEnumerable<T> collection1, IEnumerable<T> collection2) where T : IEquatable<T>
        //{
        //    IEnumerable<T> complement1 = Complement(collection1, collection2);
        //    IEnumerable<T> complement2 = Complement(collection2, collection1);
        //    return complement1.Union(complement2);
        //}
        //public static IEnumerable<T> Sort<T>(this IEnumerable<T> collection)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }
        //    List<T> list = new List<T>(collection);
        //    list.Sort();

        //    foreach (T item in list)
        //    {
        //        yield return item;
        //    }
        //}
        //public static int FindIndex<T>(this IEnumerable<T> collection, T value) where T : IEquatable<T>
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }
        //    using (IEnumerator<T> iterator = collection.GetEnumerator())
        //    {
        //        int index = 0;

        //        while (iterator.MoveNext())
        //        {
        //            if (iterator.Current.Equals(value) == false)
        //            {
        //                index++;
        //            }
        //            else
        //            {
        //                return index;
        //            }
        //        }
        //        return -1;
        //    }
        //}

    }
}