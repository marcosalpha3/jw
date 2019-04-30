using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SystemNet.Practices.Data
{
    public class SqlHelper
    {
        private static bool IsNull(IDataReader dr, string columnName)
        {
            bool exists = dr.GetSchemaTable().Rows.Cast<DataRow>().
                Any(row => (row[0].ToString().ToUpper() == columnName.ToUpper()));
            return !exists || dr.IsDBNull(dr.GetOrdinal(columnName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool GetBool(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return false;
            }
            return Convert.ToBoolean(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static byte GetByte(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return byte.MinValue;
            }
            return Convert.ToByte(dr.GetValue(dr.GetOrdinal(columnName)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static byte? GetByteNullable(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return null;
            }
            return Convert.ToByte(dr.GetValue(dr.GetOrdinal(columnName)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static short GetShort(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return short.MinValue;
            }
            return Convert.ToInt16(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetInt(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return int.MinValue;
            }
            return Convert.ToInt32(dr.GetValue(dr.GetOrdinal(columnName)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static long GetLong(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return long.MinValue;
            }
            return Convert.ToInt64(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static double GetDouble(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return double.MinValue;
            }
            return Convert.ToDouble(dr.GetValue(dr.GetOrdinal(columnName)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static decimal GetDecimal(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return decimal.MinValue;
            }
            return Convert.ToDecimal(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static ushort GetUshort(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return ushort.MinValue;
            }
            return Convert.ToUInt16(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static uint GetUint(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return uint.MinValue;
            }
            return Convert.ToUInt32(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static ulong GetUlong(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return ulong.MinValue;
            }
            return Convert.ToUInt64(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static float GetFloat(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return float.MinValue;
            }
            return Convert.ToSingle(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GetString(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return string.Empty;
            }
            return Convert.ToString(dr.GetValue(dr.GetOrdinal(columnName)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(dr.GetValue(dr.GetOrdinal(columnName)).ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// CultureInfo.InvariantCulture esta associado ao idioma ingles, logo
        /// ao recuperar a data a partir de um windows services com terminal estando por exemplo em cultura portuguesa a informação fica incorreta
        /// </summary> 
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeCurrentCulture(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(dr.GetValue(dr.GetOrdinal(columnName)).ToString(), CultureInfo.CurrentCulture);
        }

        public static DateTime? GetDateTimeNullableCurrentCulture(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return null;
            }
            return Convert.ToDateTime(dr.GetValue(dr.GetOrdinal(columnName)).ToString(), CultureInfo.CurrentCulture);
        }

        public static DateTime GetDateTime2(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return DateTime.MinValue;
            }
            DateTime retorno;
            if (DateTime.TryParse(dr.GetValue(dr.GetOrdinal(columnName)).ToString(), out retorno))
                return retorno;
            else
                return Convert.ToDateTime(dr.GetValue(dr.GetOrdinal(columnName)).ToString(), CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Guid GetGuid(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return Guid.Empty;
            }
            return (Guid)dr.GetValue(dr.GetOrdinal(columnName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static byte[] GetByteArray(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return null;
            }
            return (byte[])dr.GetValue(dr.GetOrdinal(columnName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(IDataReader dr, string columnName)
        {
            if (IsNull(dr, columnName))
            {
                return TimeSpan.MinValue;
            }
            return (TimeSpan)dr.GetValue(dr.GetOrdinal(columnName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(byte value)
        {
            return ((value == byte.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(short value)
        {
            return ((value == short.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(int value)
        {
            return ((value == int.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(long value)
        {
            return ((value == long.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(double value)
        {
            return ((value == double.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(decimal value)
        {
            return ((value == decimal.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(ushort value)
        {
            return ((value == ushort.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(uint value)
        {
            return ((value == uint.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(ulong value)
        {
            return ((value == ulong.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(float value)
        {
            return ((value == float.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(string value)
        {
            return ((value == string.Empty | value == null) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(DateTime value)
        {
            return ((value == DateTime.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(Guid value)
        {
            return ((value == Guid.Empty) ? ((object)DBNull.Value) : ((object)value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(TimeSpan value)
        {
            return ((value == TimeSpan.MinValue) ? ((object)DBNull.Value) : ((object)value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(char value)
        {
            return ((value == '\0') ? ((object)DBNull.Value) : ((object)value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetEntry(bool? value)
        {
            return (!value.HasValue ? ((object)DBNull.Value) : ((object)value.Value));
        }
    }
}
