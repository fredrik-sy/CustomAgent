using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    class ConnectionGraph
    {
        private HashSet<int>[] adjV;
        private HashSet<int>[] adjW;

        public ConnectionGraph(int v, int w)
        {
            V = v;
            W = w;
            adjV = new HashSet<int>[V];
            adjW = new HashSet<int>[W];
            Initialize();
        }

        public int V { get; private set; }

        public int W { get; private set; }

        public int E { get; private set; }

        private void Initialize()
        {
            for (int v = 0; v < V; v++)
                adjV[v] = new HashSet<int>();

            for (int w = 0; w < W; w++)
                adjW[w] = new HashSet<int>();
        }

        public void AddUndirectedEdge(int v, int w)
        {
            E++;

            if (adjV[v].Add(w) && adjW[w].Add(v) == false)
                throw new ArgumentException();
        }

        public void RemoveUndirectedEdge(int v, int w)
        {
            E--;
            adjV[v].Remove(w);
            adjW[w].Remove(v);
        }

        public HashSet<int> AdjV(int v)
        {
            return adjV[v];
        }

        public HashSet<int> AdjW(int w)
        {
            return adjW[w];
        }
    }
}
