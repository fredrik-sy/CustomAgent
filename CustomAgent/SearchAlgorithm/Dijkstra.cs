using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class Dijkstra
    {
        private Graph graph;
        private Player player;

        public Dijkstra(Graph graph, Player player)
        {
            this.graph = graph;
            this.player = player;
        }

        public bool OpponentCloseBy { get; set; }
        public List<int> Path { get; private set; }

        public void CreatePath(Dijkstra collision = null)
        {
            PriorityQueue<int> vertexSet = new PriorityQueue<int>(81);
            int[] distance = new int[graph.V];
            int[] previous = new int[graph.V];

            OpponentCloseBy = false;
            bool collisionOccurred = false;
            int collisionCheckpoint = -1;
            int source = player.Position();

            distance[source] = 0;

            for (int v = 0; v < graph.V; v++)
            {
                if (v != source)
                    distance[v] = QuoridorGraph.SQUARE_SPACES;

                previous[v] = -1;
                vertexSet.Insert(v, distance[v]);
            }

            while (!vertexSet.Empty)
            {
                int u = vertexSet.DeleteMin();

                foreach (int v in QuoridorGraph.Graph.Adj(u))
                {
                    int alt = distance[u] + 1;

                    if (alt < distance[v])
                    {
                        if (collision != null)
                        {
                            /* Collision Check When Near Opponent */
                            if (alt == 1)
                            {
                                if (v == collision.player.Position())
                                {
                                    OpponentCloseBy = true;

                                    if (player.Active())
                                        continue;
                                    else
                                        collisionOccurred = true;
                                }
                            }
                            else if (collision.Path != null && alt - 1 < collision.Path.Count)    /* Collision Check When Player Meets */
                            {
                                if (v == collision.Path[collision.Path.Count - (alt - 1)])  /* LIFO List */
                                {
                                    if (player.Active())
                                    {
                                        collisionCheckpoint = u;
                                        continue;
                                    }
                                    else
                                    {
                                        collisionOccurred = true;
                                    }
                                }
                            }

                            /* Collision Check On Same Position */
                            if (collision.Path != null && alt < collision.Path.Count)
                            {
                                if (v == collision.Path[collision.Path.Count - alt])
                                {
                                    if (player.Active())
                                    {
                                        collisionOccurred = true;
                                    }
                                    else
                                    {
                                        collisionCheckpoint = u;
                                        continue;
                                    }
                                }
                            }
                        }

                        distance[v] = alt;
                        previous[v] = u;
                        vertexSet.DecreaseKey(v, alt);
                    }
                }
            }

            Path = ReconstructPath(player.Goals(), distance, previous);

            if (Path == null && collision != null)
            {
                List<int> goals = new List<int>();

                /* Opponent Blocking Path */
                if (collisionCheckpoint != -1)
                {
                    if (distance[collisionCheckpoint] != QuoridorGraph.SQUARE_SPACES)
                        goals.Add(collisionCheckpoint);

                    Path = ReconstructPath(goals, distance, previous);
                }

                /* Opponent Body Blocking - Random Walk In Any Direction */
                if (goals.Count == 0 && OpponentCloseBy)
                {
                    int w = -1;

                    foreach (int v in graph.Adj(player.Position()))
                    {
                        if (distance[v] != QuoridorGraph.SQUARE_SPACES)
                        {
                            w = v;

                            /* Avoid Opponents Path If Possible */
                            if (collision.Path != null && !collision.Path.Contains(distance[v]))
                                break;
                        }
                    }

                    if (w != -1)
                        Path = ReconstructPath(w, distance, previous);
                }
            }

            if (collisionOccurred)
                collision.CreatePath(this);
        }

        private List<int> ReconstructPath(int goal, int[] distance, int[] previous)
        {
            List<int> goals = new List<int>(1);
            goals.Add(goal);
            return ReconstructPath(goals, distance, previous);
        }

        private List<int> ReconstructPath(List<int> goals, int[] distance, int[] previous)
        {
            int cheapestDistance = QuoridorGraph.SQUARE_SPACES;
            int v = -1;

            foreach (int g in goals)
            {
                if (distance[g] < cheapestDistance)
                {
                    cheapestDistance = distance[g];
                    v = g;
                }
            }

            /* Opponent/Wall-Isolation Block */
            if (cheapestDistance == QuoridorGraph.SQUARE_SPACES)
                return null;

            List<int> cheapestPath = new List<int>();

            while (previous[v] != -1)
            {
                cheapestPath.Add(v);
                v = previous[v];
            }

            return cheapestPath;
        }
    }
}
