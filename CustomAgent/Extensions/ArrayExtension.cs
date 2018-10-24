using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class ArrayExtension
    {
        public static void Fill<TSource>(this TSource[] source, TSource value)
        {
            for (int i = 0; i < source.Length; i++)
                source[i] = value;
        }
    }
}
