using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    static class Dijkstra
    {
        public static Path[] CreatePath(params Player[] players)
        {
            Dictionary<Player, PriorityQueue<int>> vertexSet = new Dictionary<Player, PriorityQueue<int>>();
            Dictionary<Player, int[]> distance = new Dictionary<Player, int[]>();
            Dictionary<Player, int[]> previous = new Dictionary<Player, int[]>();

            foreach (Player player in players)
            {
                vertexSet[player] = new PriorityQueue<int>(QuoridorGraph.SQUARE_SPACES);
                distance[player] = new int[QuoridorGraph.SQUARE_SPACES];
                previous[player] = new int[QuoridorGraph.SQUARE_SPACES];

                distance[player][player.Position] = 0;
            }

            for (int v = 0; v < QuoridorGraph.SQUARE_SPACES; v++)
            {
                foreach (Player player in players)
                {
                    if (v != player.Position)
                        distance[player][v] = QuoridorGraph.SQUARE_SPACES;

                    previous[player][v] = -1;
                    vertexSet[player].Insert(v, distance[player][v]);
                }
            }

            while (!vertexSet.Values.All(o => o.Empty))
            {
                foreach (Player player in players)
                {
                    int u = vertexSet[player].DeleteMin();

                    foreach (int v in QuoridorGraph.BoardGraph.Adj(u))
                    {
                        int alt = distance[player][u] + 1;

                        if (alt < distance[player][v])
                        {
                            if (collisionPath != null)
                            {
                                /* Collision Check When Near Opponent */
                                if (alt == 1)
                                {
                                    if (v == collisionPath.Source)
                                    {
                                        path.OpponentCloseBy = true;

                                        if (player.MyTurn())
                                            continue;
                                        else
                                            collisionOccurred = true;
                                    }
                                }
                                else if (collisionPath.Path != null && alt - 1 < collisionPath.Path.Count)    /* Collision Check When Player Meets */
                                {
                                    if (v == collisionPath.Path[collisionPath.Path.Count - (alt - 1)])  /* LIFO List */
                                    {
                                        if (player.MyTurn())
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
                                if (collisionPath.Path != null && alt < collisionPath.Path.Count)
                                {
                                    if (v == collisionPath.Path[collisionPath.Path.Count - alt])
                                    {
                                        if (player.MyTurn())
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

                            distance[player][v] = alt;
                            previous[player][v] = u;
                            vertexSet[player].DecreaseKey(v, alt);
                        }
                    }
                }
            }

            return null;
        }

        //public static Path CreatePath(Player player, Path? collisionPath = null)
        //{
        //    PriorityQueue<int> vertexSet = new PriorityQueue<int>(QuoridorGraph.SQUARE_SPACES);

        //    int source = player.Position;

        //    bool collisionOccurred = false;
        //    int collisionCheckpoint = -1;

        //    distance[source] = 0;

        //    for (int v = 0; v < QuoridorGraph.SQUARE_SPACES; v++)
        //    {
        //        if (v != source)
        //            distance[v] = QuoridorGraph.SQUARE_SPACES;

        //        previous[v] = -1;
        //        vertexSet.Insert(v, distance[v]);
        //    }

        //    while (!vertexSet.Empty)
        //    {
        //        int u = vertexSet.DeleteMin();

        //        foreach (int v in QuoridorGraph.BoardGraph.Adj(u))
        //        {
        //            int alt = distance[u] + 1;

        //            if (alt < distance[v])
        //            {
        //                if (collisionPath != null)
        //                {
        //                    /* Collision Check When Near Opponent */
        //                    if (alt == 1)
        //                    {
        //                        if (v == collisionPath.Source)
        //                        {
        //                            path.OpponentCloseBy = true;

        //                            if (player.MyTurn())
        //                                continue;
        //                            else
        //                                collisionOccurred = true;
        //                        }
        //                    }
        //                    else if (collisionPath.Path != null && alt - 1 < collisionPath.Path.Count)    /* Collision Check When Player Meets */
        //                    {
        //                        if (v == collisionPath.Path[collisionPath.Path.Count - (alt - 1)])  /* LIFO List */
        //                        {
        //                            if (player.MyTurn())
        //                            {
        //                                collisionCheckpoint = u;
        //                                continue;
        //                            }
        //                            else
        //                            {
        //                                collisionOccurred = true;
        //                            }
        //                        }
        //                    }

        //                    /* Collision Check On Same Position */
        //                    if (collisionPath.Path != null && alt < collisionPath.Path.Count)
        //                    {
        //                        if (v == collisionPath.Path[collisionPath.Path.Count - alt])
        //                        {
        //                            if (player.MyTurn())
        //                            {
        //                                collisionOccurred = true;
        //                            }
        //                            else
        //                            {
        //                                collisionCheckpoint = u;
        //                                continue;
        //                            }
        //                        }
        //                    }
        //                }

        //                distance[v] = alt;
        //                previous[v] = u;
        //                vertexSet.DecreaseKey(v, alt);
        //            }
        //        }
        //    }

        //    Path = ReconstructPath(player.Goals(), distance, previous);

        //    if (Path == null && collisionPath != null)
        //    {
        //        List<int> goals = new List<int>();

        //        /* Opponent Blocking Path */
        //        if (collisionCheckpoint != -1)
        //        {
        //            if (distance[collisionCheckpoint] != QuoridorGraph.SQUARE_SPACES)
        //                goals.Add(collisionCheckpoint);

        //            Path = ReconstructPath(goals, distance, previous);
        //        }

        //        /* Opponent Body Blocking - Random Walk In Any Direction */
        //        if (goals.Count == 0 && OpponentCloseBy)
        //        {
        //            int w = -1;

        //            foreach (int v in graph.Adj(player.Position()))
        //            {
        //                if (distance[v] != QuoridorGraph.SQUARE_SPACES)
        //                {
        //                    w = v;

        //                    /* Avoid Opponents Path If Possible */
        //                    if (collisionPath.Path != null && !collisionPath.Path.Contains(v))
        //                        break;
        //                }
        //            }

        //            if (w != -1)
        //                Path = ReconstructPath(w, distance, previous);
        //            else if (graph.Adj(player.Position()).Count > 0)
        //                Path = new List<int>(); /* Isolated In A Corner */
        //        }
        //    }

        //    if (collisionOccurred)
        //        collisionPath.CreatePath(this);
        //}

        //private List<int> ReconstructPath(int goal, int[] distance, int[] previous)
        //{
        //    List<int> goals = new List<int>(1);
        //    goals.Add(goal);
        //    return ReconstructPath(goals, distance, previous);
        //}

        //private List<int> ReconstructPath(List<int> goals, int[] distance, int[] previous)
        //{
        //    int cheapestDistance = QuoridorGraph.SQUARE_SPACES;
        //    int v = -1;

        //    foreach (int g in goals)
        //    {
        //        if (distance[g] < cheapestDistance)
        //        {
        //            cheapestDistance = distance[g];
        //            v = g;
        //        }
        //    }

        //    /* Opponent/Wall-Isolation Block */
        //    if (cheapestDistance == QuoridorGraph.SQUARE_SPACES)
        //        return null;

        //    List<int> cheapestPath = new List<int>();

        //    while (previous[v] != -1)
        //    {
        //        cheapestPath.Add(v);
        //        v = previous[v];
        //    }

        //    return cheapestPath;
        //}
    }
}
