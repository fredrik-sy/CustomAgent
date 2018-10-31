using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class DictionaryExtension
    {
        public static Dictionary<TKey, TSource> Join<TKey, TSource>(this Dictionary<TKey, TSource> source, Dictionary<TKey, TSource> other) where TSource : HashSet<int>
        {
            foreach (var o in other)
                foreach (int v in o.Value)
                    source[o.Key].Add(v);

            return source;
        }
    }
}
