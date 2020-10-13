using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;

namespace CustomAgent
{
    class Graph
    {
        private HashSet<int>[] adj;

        public Graph(int x, int y)
        {
            V = ToOneDimension(x, y);
            E = 0;
            X = x;
            Y = y;
            adj = new HashSet<int>[V];
            Initialize();
        }

        public int V { get; private set; }

        public int E { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        private void Initialize()
        {
            for (int v = 0; v < V; v++)
                adj[v] = new HashSet<int>();
        }

        public void AddUndirectedEdge(int x1, int y1, int x2, int y2)
        {
            AddUndirectedEdge(ToOneDimension(x1, y1), ToOneDimension(x2, y2));
        }

        public void AddUndirectedEdge(Point v, Point w)
        {
            AddUndirectedEdge(ToOneDimension(v), ToOneDimension(w));
        }

        public void AddUndirectedEdge(int v, int w)
        {
            E++;

            if (adj[v].Add(w) && adj[w].Add(v) == false)
                throw new ArgumentException();
        }

        public void RemoveUndirectedEdge(int v, int w)
        {
            E--;
            adj[v].Remove(w);
            adj[w].Remove(v);
        }

        public bool IsAdj(int v, int w)
        {
            return adj[v].Contains(w);
        }

        public HashSet<int> Adj(int v)
        {
            return adj[v];
        }

        public int ToOneDimension(Point p)
        {
            return ToOneDimension(p.X, p.Y);
        }

        public int ToOneDimension(int x, int y)
        {
            return X * y + x;
        }

        public Point ToTwoDimension(int v)
        {
            return new Point(ToX(v), ToY(v));
        }

        public int ToX(int v)
        {
            return v % X;
        }

        public int ToY(int v)
        {
            return v / X;
        }
    }
}
