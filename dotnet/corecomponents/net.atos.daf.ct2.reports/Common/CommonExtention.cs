using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.atos.daf.ct2.reports.Common
{
    public static class CommonExtention
    {
        /// <summary>
        /// Create Batch/Chunk of givens Size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Array or List of given Type values</param>
        /// <param name="chunkSize">Provide required batch size</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int chunkSize)
        {
            return list.Select((item, index) => new { index, item })
                 .GroupBy(x => x.index / chunkSize)
                 .Select(x => x.Select(y => y.item));
        }
    }
}
