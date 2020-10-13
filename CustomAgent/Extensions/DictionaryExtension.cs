using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    static class DictionaryExtension
    {
        public static void Insert<TKey, TSource, TValue>(this IDictionary<TKey, TSource> source, TKey key, TValue value) where TSource : ICollection<TValue>, new()
        {
            if (source[key] == null)
                source[key] = new TSource();

            source[key].Add(value);
        }

        public static TSource MinByKey<TSource>(this IDictionary<int, TSource> source) where TSource : new()
        {
            int value = 0;
            bool hasValue = false;

            foreach (int key in source.Keys)
            {
                if (hasValue)
                {
                    if (key < value)
                        value = key;
                }
                else
                {
                    value = key;
                    hasValue = true;
                }
            }

            if (hasValue)
                return source[value];

            return new TSource();
        }
    }
}
