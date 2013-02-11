using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DL.Framework.Common
{
    public static class ListExtensions
    {
        public static string Flatten(this IList<string> list)
        {
            return Flatten(list, Constants.Space, false);
        }

        public static string Flatten(this IList<string> list, bool toLower)
        {
            return Flatten(list, Constants.Space, toLower);
        }

        public static string Flatten(this IList<string> list, string separator, bool toLower)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            string[] items = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
                items[i] = (toLower) ? list[i].ToLower() : list[i];

            return string.Join(separator, items);
        }

        public static List<string> SplitToList(this string str)
        {
            return SplitToList(str, false);
        }

        public static List<string> SplitToList(this string str, bool toLower)
        {
            if (toLower && str != null)
                str = str.ToLower();

            return (str ?? string.Empty).Split(new string[] { Constants.Space }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (T item in items)
                list.Add(item);
        }

        public static string ConvertListToDbString<T>(this IEnumerable<T> list)
        {
            return list.ConvertListToString<T>(",", true);
        }

        public static string ConvertListToString<T>(this IEnumerable<T> list, string separator)
        {
            return list.ConvertListToString(separator, true);
        }

        public static string ConvertListToString<T>(this IEnumerable<T> list, string separator, bool ignoreDuplicates)
        {
            if (list == null)
                return null;

            StringBuilder sb = new StringBuilder();
            IList<T> addedList = new List<T>();

            string s = "";
            foreach (T item in list)
            {
                if (item != null && (!ignoreDuplicates || !addedList.Contains(item)))
                {
                    sb.AppendFormat("{0}{1}", s, item.ToString());
                    addedList.Add(item);
                }

                s = separator;
            }

            return sb.ToString();
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in collection) c.Add(e);
            return c;
        }

        public static AsyncObservableCollection<T> ToAsnycObservableCollection<T>(this IEnumerable<T> collection)
        {
            var c = new AsyncObservableCollection<T>();
            foreach (var e in collection) c.Add(e);
            return c;
        }
    }
}
