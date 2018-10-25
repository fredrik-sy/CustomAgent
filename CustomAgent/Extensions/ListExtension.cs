using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class ListExtension
    {
        public static IEnumerable<TSource> ForEachRangeReverse<TSource>(this List<TSource> source, int limitBeginElement = -1, int limitEndElement = -1)
        {
            if (limitBeginElement < -1 || limitEndElement < -1)
                throw new ArgumentOutOfRangeException();

            if (limitBeginElement != -1)
                limitBeginElement = Clamp(source.Count - (limitBeginElement + 1), 0, source.Count);

            if (limitEndElement != -1)
                limitEndElement = Clamp(0, source.Count - (limitBeginElement + 1), source.Count);

            for (int i = source.Count - 1; i > limitBeginElement; i--)
                yield return source[i];
            
            for (int i = 0; i < limitEndElement; i++)
                yield return source[i];
        }

        public static TSource Peek<TSource>(this List<TSource> source)
        {
            TSource item = source[source.Count - 1];
            return item;
        }

        public static TSource Pop<TSource>(this List<TSource> source)
        {
            TSource item = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);
            return item;
        }

        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
