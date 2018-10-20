using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class PriorityQueue<TKey> where TKey : IComparable
    {
        private int[] pq;
        private int[] qp;
        private TKey[] keys;

        public PriorityQueue(int capacity)
        {
            pq = new int[capacity + 1];
            qp = new int[capacity + 1];
            keys = new TKey[capacity + 1];

            for (int i = 0; i <= capacity; i++)
                qp[i] = -1;
        }

        public int Count { get; private set; }

        public bool Empty
        {
            get
            {
                return Count == 0;
            }
        }

        public bool Contains(int i)
        {
            return qp[i] != -1;
        }

        public void DecreaseKey(int i, TKey key)
        {
            if (!Contains(i))
                throw new ArgumentException();

            if (keys[i].CompareTo(key) <= 0)
                return;

            keys[i] = key;
            Swim(qp[i]);
        }

        public void Delete(int i)
        {
            if (!Contains(i))
                throw new ArgumentException();

            int index = qp[i];
            Swap(index, Count--);
            Swim(index);
            Sink(index);
            keys[i] = default(TKey);
            qp[i] = -1;
        }

        public int DeleteMin()
        {
            if (Count == 0)
                throw new InvalidOperationException();

            int min = pq[1];
            Swap(1, Count--);
            Sink(1);
            qp[min] = -1;
            keys[min] = default(TKey);
            pq[Count + 1] = -1;
            return min;
        }
        
        public void Insert(int i, TKey key)
        {
            if (Contains(i))
                throw new ArgumentException();

            Count++;
            qp[i] = Count;
            pq[Count] = i;
            keys[i] = key;
            Swim(Count);
        }

        private bool Greater(int i, int j)
        {
            return keys[pq[i]].CompareTo(keys[pq[j]]) > 0;
        }

        private void Sink(int k)
        {
            while (2 * k <= Count)
            {
                int j = 2 * k;

                if (j < Count && Greater(j, j + 1))
                    j++;

                if (!Greater(k, j))
                    break;

                Swap(k, j);
                k = j;
            }
        }

        private void Swap(int i, int j)
        {
            int tmp = pq[i];
            pq[i] = pq[j];
            pq[j] = tmp;
            qp[pq[i]] = i;
            qp[pq[j]] = j;
        }

        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Swap(k, k / 2);
                k = k / 2;
            }
        }
    }
}
