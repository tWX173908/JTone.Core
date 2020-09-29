

namespace JTone.Core
{
    /// <summary>
    /// 仓储层异常
    /// </summary>
    public class RepositoryException : System.Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public RepositoryException(string message)
            : base(message)
        {

        }
    }
}
