using System.Collections.Generic;
using System.Linq;


namespace JTone.Core
{
    public class ListExtension
    {
        /// <summary>
        /// Count扩展 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Count<T>(IEnumerable<T> data)
        {
            return data?.Count() ?? 0;
        }
    }
}
