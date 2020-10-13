using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    class BreadthFirstSearch
    {
        private BreadthFirstSearch(Player player)
        {
            Player = player;
            Marked = new bool[QuoridorGraph.SQUARE_SPACES];
            EdgeTo = new int[QuoridorGraph.SQUARE_SPACES];
            DistTo = new int[QuoridorGraph.SQUARE_SPACES];
            Q = new Queue<int>();

            for (int v = 0; v < QuoridorGraph.SQUARE_SPACES; v++)
                DistTo[v] = QuoridorGraph.SQUARE_SPACES;

            int s = Player.Position;
            DistTo[s] = 0;
            Marked[s] = true;
            Q.Enqueue(s);
        }

        public Player Player { get; private set; }

        public Stack<int> Path { get; private set; }

        public Queue<int> Q { get; private set; }

        public bool[] Marked { get; private set; }

        public int[] EdgeTo { get; private set; }

        public int[] DistTo { get; private set; }

        #region Static
        private static bool CheckCollision;
        private static int Checkpoint;

        public static Dictionary<Player, Stack<int>> CreatePaths()
        {
            BreadthFirstSearch selfBFS = new BreadthFirstSearch(QuoridorPlayer.Self);
            BreadthFirstSearch opponentBFS = new BreadthFirstSearch(QuoridorPlayer.Opponent);

            CheckCollision = true;

            while (selfBFS.Q.Count > 0 || opponentBFS.Q.Count > 0)
            {
                if (selfBFS.Q.Count > 0)
                    Expand(selfBFS, opponentBFS);

                if (opponentBFS.Q.Count > 0)
                    Expand(opponentBFS, selfBFS);
            }

            if (selfBFS.Player.MyTurn)
            {
                return new Dictionary<Player, Stack<int>>()
                {
                    { opponentBFS.Player, ConstructPath(opponentBFS) },
                    { selfBFS.Player, ConstructPath(selfBFS, opponentBFS) }
                };
            }
            else
            {
                return new Dictionary<Player, Stack<int>>()
                {
                    { selfBFS.Player, ConstructPath(selfBFS) },
                    { opponentBFS.Player, ConstructPath(opponentBFS, selfBFS) }
                };
            }
        }

        private static void Expand(BreadthFirstSearch b, BreadthFirstSearch c)
        {
            int v = b.Q.Dequeue();

            foreach (int w in QuoridorGraph.BoardGraph.Adj(v))
            {
                if (!b.Marked[w])
                {
                    if (CheckCollision)
                    {
                        /* Check collision when facing opponent */
                        if (b.DistTo[v] == c.DistTo[w])
                        {
                            CheckCollision = false;

                            if (b.Player.MyTurn)
                            {
                                Checkpoint = v;
                                continue;
                            }
                        }

                        /* Check first come, first served collision */
                        if (b.DistTo[v] + 1 == c.DistTo[w])
                        {
                            CheckCollision = false;

                            if (b.Player.MyTurn)
                            {
                                Checkpoint = v;
                                continue;
                            }
                        }
                    }

                    b.EdgeTo[w] = v;
                    b.DistTo[w] = b.DistTo[v] + 1;
                    b.Marked[w] = true;
                    b.Q.Enqueue(w);
                }
            }
        }

        private static Stack<int> ConstructPath(BreadthFirstSearch b, BreadthFirstSearch c = null)
        {
            int cheapestDist = QuoridorGraph.SQUARE_SPACES;
            int w = -1;

            foreach (int g in b.Player.Goals)
            {
                if (b.DistTo[g] < cheapestDist)
                {
                    cheapestDist = b.DistTo[g];
                    w = g;
                }
            }

            /* Opponent blocking the path */
            if (w == -1)
            {
                if (Checkpoint != b.Player.Position)
                {
                    w = Checkpoint;
                }
                else
                {
                    /* Random walk */
                    foreach (int v in QuoridorGraph.BoardGraph.Adj(b.Player.Position))
                    {
                        if (b.DistTo[v] < QuoridorGraph.SQUARE_SPACES)
                        {
                            w = v;

                            /* Avoid the opponent's path */
                            if (!c.Path.Contains(v))
                                break;
                        }
                    }

                    /* Isolated */
                    if (w == -1)
                        return new Stack<int>();
                }
            }

            Stack<int> path = new Stack<int>();

            while (b.DistTo[w] != 0)
            {
                path.Push(w);
                w = b.EdgeTo[w];
            }

            return path;
        }
        #endregion
    }
}
