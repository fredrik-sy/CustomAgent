using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class Graph
    {
        private HashSet<int>[] adj;

        public Graph(int v)
        {
            V = v;
            E = 0;
            adj = new HashSet<int>[V];
            Initialize();
        }

        public int V { get; private set; }

        public int E { get; private set; }

        private void Initialize()
        {
            for (int v = 0; v < V; v++)
                adj[v] = new HashSet<int>();
        }

        public void AddUndirectedEdge(int v, int w)
        {
            E++;
            adj[v].Add(w);
            adj[w].Add(v);
        }

        public void RemoveUndirectedEdge(int v, int w)
        {
            E--;
            adj[v].Remove(w);
            adj[w].Remove(v);
        }

        public bool UndirectedEdgeExists(int v, int w)
        {
            return adj[v].Contains(w) && adj[w].Contains(v);
        }

        public bool IsAdj(int v, int w)
        {
            return adj[v].Contains(w);
        }

        public bool IsOccupied(int v)
        {
            return PlayerExtension.Self.Position() == v || PlayerExtension.Opponent.Position() == v;
        }

        public HashSet<int> Adj(int v)
        {
            return adj[v];
        }

        public int Degree(int v)
        {
            return adj[v].Count;
        }
    }
}
