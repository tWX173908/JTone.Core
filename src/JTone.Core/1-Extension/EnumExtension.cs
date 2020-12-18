using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace JTone.Core
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static int Value(this Enum instance)
        {
            return (int) (object) instance;
        }


        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Desc(this Enum instance)
        {
            var field = instance.GetType().GetTypeFields().FirstOrDefault(f => f.Name == instance.ToString());
            return field == null ? default : field.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }


        /// <summary>
        /// 获取枚举文本
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Text(this Enum instance)
        {
            return instance.ToString();
        }


        /// <summary>
        /// 数值类型转枚举
        /// </summary>
        /// <typeparam name="T1">数据类型</typeparam>
        /// <typeparam name="T2">枚举类型</typeparam>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static T2 ToEnum<T1, T2>(this T1 t1)
            where T1 : struct
            where T2 : struct
        {
            var type2 = typeof(T2);
            if (!type2.IsEnum)
                throw new NotSupportedException($"{type2} is not enum type");

            var type1 = typeof(T1);
            if (!type1.IsSimpleType())
                throw new NotSupportedException($"{type1} is not simple type");

            var t2 = Enum.Parse(type2, t1.ToString());

            if (!Enum.IsDefined(type2, t2))
                throw new NotSupportedException($"{t2} can not convert");

            return (T2) t2;
        }


        /// <summary>
        /// 字符串数值类型转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str) 
        where T : struct
        {
            var type2 = typeof(T);
            if (!type2.IsEnum)
                throw new NotSupportedException($"{type2} is not enum type");

            if (long.TryParse(str, out _))
            {
                return (T)Enum.Parse(type2, str);
            }

            throw new NotSupportedException($"{str} can not convert");
        }


        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str, Func<string, T> func)
            where T : struct
        {
            return func(str);
        }
    }
}
