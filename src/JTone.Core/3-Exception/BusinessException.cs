

namespace JTone.Core
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class BusinessException : System.Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public BusinessException(string message)
            : base(message)
        {

        }
    }
}
