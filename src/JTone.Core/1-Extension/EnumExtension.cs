using System;
using System.ComponentModel;
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
            return 0;
        }


        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Des(this Enum instance)
        {
            return instance.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ?? nameof(instance);
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
    }
}
