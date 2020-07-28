

namespace JTone.Core.Exception
{
    /// <summary>
    /// 第三方平台异常
    /// </summary>
    public class ThirdPlatformException : System.Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public ThirdPlatformException(string message)
            : base(message)
        {

        }
    }
}
