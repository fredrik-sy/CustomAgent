﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class ListExtension
    {
        public static IEnumerable<TSource> ForEachReverse<TSource>(this List<TSource> source, int beginCount = QuoridorGraph.SQUARE_SPACES, int endCount = 0)
        {
            if (source.Count <= beginCount + endCount)
            {
                for (int i = source.Count - 1; i > -1; i--)
                    yield return source[i];
            }
            else
            {
                beginCount = (source.Count - 1) - beginCount;

                for (int i = source.Count - 1; i > beginCount; i--)
                    yield return source[i];

                for (int i = endCount - 1; i > -1; i--)
                    yield return source[i];
            }
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
    }
}
