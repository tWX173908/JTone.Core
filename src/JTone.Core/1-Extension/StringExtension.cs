

namespace JTone.Core
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 是否空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }


        /// <summary>
        /// 是否非空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !IsNullOrEmpty(str);
        }


        /// <summary>
        /// 字符串转数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Int(this string str, int defaultValue = 0)
        {
            return int.TryParse(str, out var value) ? value : defaultValue;
        }


        /// <summary>
        /// 字符串转数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long Long(this string str, long defaultValue = 0)
        {
            return long.TryParse(str, out var value) ? value : defaultValue;
        }
    }
}
