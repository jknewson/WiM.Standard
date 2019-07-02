﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WIM.Utilities.Extensions
{
    public static class DBOpsExtensions
    {
        public static IEnumerable<T> Select<T>(this IDataReader reader,
                               Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }
        public static Boolean HasColumn(this IDataReader reader,
                                       string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == columnName)
                {
                    return true;
                }
            }

            return false;
        }
        public static T? GetValueOrNull<T>(this string valueAsString)
        where T : struct
        {
            if (string.IsNullOrEmpty(valueAsString))
                return null;
            return (T)Convert.ChangeType(valueAsString, typeof(T));
        }

        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static T GetDataType<T>(this System.Data.IDataReader r, string name, object defaultIfNull = null)
        {
            try
            {
                var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                var col = r.GetOrdinal(name);
                return r.IsDBNull(col) || String.IsNullOrEmpty(r[name].ToString()) ? (T)defaultIfNull : (T)Convert.ChangeType(r[name], t);
            }
            catch (Exception ex)
            {
                return (T)defaultIfNull;
            }
        }
    }
}
