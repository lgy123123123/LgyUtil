using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LgyUtil
{
    /// <summary>
    /// Convert转换帮助类
    /// </summary>
    public static class ObjectConvertUtil
    {
        /// <summary>
        /// 转成int类型(支持字符串科学计数法转换，支持百分比转换)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static int ToInt(this object obj, bool ignoreError = false)
        {
            try
            {
                if (obj is string s && s.IsNotNullOrEmpty())
                {
                    if (s.Contains("E") || s.Contains("e"))
                        return int.Parse(s, System.Globalization.NumberStyles.Any);
                    else if (s.EndsWith("%"))
                        return Convert.ToInt32(s.TrimEnd('%')) / 100;
                }
                return Convert.ToInt32(obj);
            }
            catch
            {
                if (ignoreError)
                    return 0;
                throw;
            }
        }

        /// <summary>
        /// Nullable int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? ToIntNullable(this object obj)
        {
            int? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToInt32(obj);
            }
            catch
            {
                // ignored
            }

            return ret;
        }

        /// <summary>
        /// 转成double类型(支持字符串科学计数法转换，支持百分比转换)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, bool ignoreError = false)
        {
            try
            {
                if (obj is string s && s.IsNotNullOrEmpty())
                {
                    if (s.Contains("E") || s.Contains("e"))
                        return double.Parse(s, System.Globalization.NumberStyles.Any);
                    else if (s.EndsWith("%"))
                        return Convert.ToDouble(s.TrimEnd('%')) / 100;
                }
                return Convert.ToDouble(obj);
            }
            catch
            {
                if (ignoreError)
                    return 0;
                throw;
            }
        }
        /// <summary>
        /// Nullable double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double? ToDoubleNullable(this object obj)
        {
            double? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToDouble(obj);
            }
            catch
            {
                // ignored
            }

            return ret;
        }
        /// <summary>
        /// 转成long类型(支持字符串科学计数法转换，支持百分比转换)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static long ToLong(this object obj, bool ignoreError = false)
        {
            try
            {
                if (obj is string s && s.IsNotNullOrEmpty())
                {
                    if (s.Contains("E") || s.Contains("e"))
                        return long.Parse(s, System.Globalization.NumberStyles.Any);
                    else if (s.EndsWith("%"))
                        return Convert.ToInt64(s.TrimEnd('%')) / 100;
                }
                return Convert.ToInt64(obj);
            }
            catch
            {
                if (ignoreError)
                    return 0;
                throw;
            }
        }
        /// <summary>
        /// Nullable long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long? ToLongNullable(this object obj)
        {
            long? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToInt64(obj);
            }
            catch
            {
                // ignored
            }

            return ret;
        }
        /// <summary>
        /// 转成decimal类型(支持字符串科学计数法转换，支持百分比转换)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, bool ignoreError = false)
        {
            try
            {
                if (obj is string s && s.IsNotNullOrEmpty())
                {
                    if (s.Contains("E") || s.Contains("e"))
                        return decimal.Parse(s, System.Globalization.NumberStyles.Any);
                    else if (s.EndsWith("%"))
                        return Convert.ToDecimal(s.TrimEnd('%')) / 100;
                }
                return Convert.ToDecimal(obj);
            }
            catch
            {
                if (ignoreError)
                    return 0;
                throw;
            }
        }
        /// <summary>
        /// Nullable decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? ToDecimalNullable(this object obj)
        {
            decimal? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToDecimal(obj);
            }
            catch
            {
                // ignored
            }

            return ret;
        }
        /// <summary>
        /// 转成float类型(支持字符串科学计数法转换)
        /// 谨慎使用！！！由于浮点数精度问题，转换结果可能有问题，不建议使用float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static float ToFloat(this object obj, bool ignoreError = false)
        {
            try
            {
                if (obj is string s && s.IsNotNullOrEmpty() && (s.Contains("E") || s.Contains("e")))
                {
                    return float.Parse(s, System.Globalization.NumberStyles.Any);
                }
                return Convert.ToSingle(obj);
            }
            catch
            {
                if (ignoreError)
                    return 0;
                throw;
            }
        }
        /// <summary>
        /// Nullable float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float? ToFloatNullable(this object obj)
        {
            float? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToSingle(obj);
            }
            catch
            {
                // ignored
            }

            return ret;
        }
        /// <summary>
        /// 转换成bool类型，1或"true"，返回true。"false"或其他数字,返回false
        /// 支持string,int,double,float,decimal,bool,byte,sbyte类型
        /// 转换失败会报错，非以上类型，都会返回false
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">如果忽略报错，在出错的情况下返回false</param>
        /// <returns></returns>
        public static bool ToBool(this object obj, bool ignoreError = false)
        {
            if (obj == null)
                return false;
            try
            {
                switch (obj)
                {
                    case string s: return s.ToLower() == "true" || s == "1";
                    case int i: return i == 1;
                    case double d: return d == 1;
                    case float f: return f == 1;
                    case decimal de: return de == 1;
                    case bool b: return b;
                    case byte bt: return bt == 1;
                    case sbyte sb: return sb == 1;
                    default: return false;
                }
            }
            catch
            {
                if (ignoreError)
                    return false;
                throw;
            }
        }
        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]
        /// 若是一个类，则需要给类打上[Serializable]标签
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原
        /// 若是一个类，则需要给类打上[Serializable]标签
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static object ToObject(this byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// 将一个序列化后的byte[]数组还原
        /// 必须给T打上[Serializable]标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] Bytes) where T : class
        {
            return ToObject(Bytes) as T;
        }
        /// <summary>
        /// 数字转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int i) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), i))
            {
                return (T)Enum.ToObject(typeof(T), i);
            }
            return default;
        }
    }
}
