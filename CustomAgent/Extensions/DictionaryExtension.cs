using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class DictionaryExtension
    {
        public static Dictionary<TKey, TSource> Join<TKey, TSource>(this Dictionary<TKey, TSource> source, Dictionary<TKey, TSource> other)
        {
            foreach (var o in other)
                source[o.Key] = o.Value;

            return source;
        }
    }
}
