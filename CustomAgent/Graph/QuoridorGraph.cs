using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using Tile = Quoridor.AI.Tile;
using WallOrientation = Quoridor.AI.WallOrientation;

namespace CustomAgent
{
    static class QuoridorGraph
    {
        public const int BOARD_SIZE = 9;
        public const int BOARD_EDGE_SIZE = 10;
        public const int SQUARE_SPACES = 81;
        public const int SQUARE_EDGE_SPACES = 100;

        public static Point UnitX = new Point(1, 0);
        public static Point UnitY = new Point(0, 1);

        public static Graph BoardGraph;
        public static Graph WallGraph;
        public static ConnectionGraph ConnectionGraph;

        public static void CreateGraph(Tile[,] tiles, bool[,] horizontalWall, bool[,] verticalWall)
        {
            BoardGraph = new Graph(BOARD_SIZE, BOARD_SIZE);
            WallGraph = new Graph(BOARD_EDGE_SIZE, BOARD_EDGE_SIZE);
            ConnectionGraph = new ConnectionGraph(SQUARE_SPACES, SQUARE_EDGE_SPACES);

            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    ConnectionGraph.AddUndirectedEdge(BoardGraph.ToOneDimension(x, y), WallGraph.ToOneDimension(x, y));
                    ConnectionGraph.AddUndirectedEdge(BoardGraph.ToOneDimension(x, y), WallGraph.ToOneDimension(x + 1, y));
                    ConnectionGraph.AddUndirectedEdge(BoardGraph.ToOneDimension(x, y), WallGraph.ToOneDimension(x, y + 1));
                    ConnectionGraph.AddUndirectedEdge(BoardGraph.ToOneDimension(x, y), WallGraph.ToOneDimension(x + 1, y + 1));

                    if (x > 0)
                        if (verticalWall[x - 1, y])
                            WallGraph.AddUndirectedEdge(x, y, x, y + 1);
                        else
                            BoardGraph.AddUndirectedEdge(x - 1, y, x, y);

                    if (y > 0)
                        if (horizontalWall[x, y - 1])
                            WallGraph.AddUndirectedEdge(x, y, x + 1, y);
                        else
                            BoardGraph.AddUndirectedEdge(x, y - 1, x, y);
                }
            }
        }

        #region WallAction
        public static bool HorizontalWallVacant(int l, int m, int r)
        {
            if (l < 10 || r >= SQUARE_EDGE_SPACES - 10 || ToWallX(l) > BOARD_EDGE_SIZE - 3)
                return false;

            return (BoardGraph.IsAdj(l, m) || BoardGraph.IsAdj(m, r)) == false;
        }

        public static bool VerticalWallVacant(int t, int m, int b)
        {
            if (ToWallX(t) == 0 || ToWallX(t) == 9 || ToWallY(t) > BOARD_EDGE_SIZE - 3)
                return false;

            return (BoardGraph.IsAdj(t, m) || BoardGraph.IsAdj(m, b)) == false;
        }

        public static bool PlaceHorizontalWall(int v)
        {
            if (HorizontalWallVacant(v))
            {
                BoardGraph.RemoveUndirectedEdge(v, v + BOARD_SIZE);
                BoardGraph.RemoveUndirectedEdge(v + 1, (v + 1) + BOARD_SIZE);
                WallGraph.AddUndirectedEdge(BoardGraph.ToTwoDimension(v) + UnitY, BoardGraph.ToTwoDimension(v + 1) + UnitY);
                WallGraph.AddUndirectedEdge(BoardGraph.ToTwoDimension(v + 1) + UnitY, BoardGraph.ToTwoDimension(v + 2) + UnitY);
                return true;
            }

            return false;
        }

        public static bool PlaceVerticalWall(int v)
        {
            if (VerticalWallVacant(v))
            {
                BoardGraph.RemoveUndirectedEdge(v, v + 1);
                BoardGraph.RemoveUndirectedEdge(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
                WallGraph.AddUndirectedEdge(BoardGraph.ToTwoDimension(v + 1), BoardGraph.ToTwoDimension(v + 1) + UnitY);
                WallGraph.AddUndirectedEdge(BoardGraph.ToTwoDimension(v + 1) + UnitY, BoardGraph.ToTwoDimension(v + 1) + UnitY + UnitY);
                return true;
            }

            return false;
        }

        public static void RemoveHorizontalWall(int v)
        {
            if (v < 0 || v >= SQUARE_SPACES ||
                ToX(v) > BOARD_SIZE - 2 ||
                ToY(v) > BOARD_SIZE - 2)
                throw new ArgumentException();

            BoardGraph.AddUndirectedEdge(v, v + BOARD_SIZE);
            BoardGraph.AddUndirectedEdge(v + 1, (v + 1) + BOARD_SIZE);
            WallGraph.RemoveUndirectedEdge(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
            WallGraph.RemoveUndirectedEdge((v + 1) + BOARD_SIZE, (v + 2) + BOARD_SIZE);
        }

        public static void RemoveVerticalWall(int v)
        {
            if (v < 0 || v >= SQUARE_SPACES ||
                ToX(v) > BOARD_SIZE - 2 ||
                ToY(v) > BOARD_SIZE - 2)
                throw new ArgumentException();

            BoardGraph.AddUndirectedEdge(v, v + 1);
            BoardGraph.AddUndirectedEdge(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
            WallGraph.RemoveUndirectedEdge(v + 1, (v + 1) + BOARD_SIZE);
            WallGraph.RemoveUndirectedEdge((v + 1) + BOARD_SIZE, (v + 1) + (BOARD_SIZE * 2));
        }
        #endregion

        public static Dictionary<WallOrientation, HashSet<int>> WallPossibilities()
        {
            HashSet<int> horizontal = new HashSet<int>();
            HashSet<int> vertical = new HashSet<int>();

            var selfWallDistance = new SortedList<int, HashSet<int>>();
            var opponentWallDistance = new SortedList<int, HashSet<int>>();

            for (int v = 0; v < SQUARE_EDGE_SPACES; v++)
            {
                /* Find wall nib */
                if (WallGraph.Adj(v).Count == 1)
                {
                    selfWallDistance.Insert(ManhattanDistance(v + QuoridorPlayer.Self.Position % 2,
                                                              ToBoardEdgeDistance(QuoridorPlayer.Self.Position + QuoridorPlayer.Self.Position % 2)), v);
                    opponentWallDistance.Insert(ManhattanDistance(v, QuoridorPlayer.Opponent.Position), v);
                }
            }

            foreach (int w in selfWallDistance.MinByKey())
            {
                AddPossibleHorizontalWalls(ref horizontal, w);
                AddPossibleVerticalWalls(ref vertical, w);
            }

            foreach (int w in opponentWallDistance.MinByKey())
            {
                AddPossibleHorizontalWalls(ref horizontal, w);
                AddPossibleVerticalWalls(ref vertical, w);
            }

            return new Dictionary<WallOrientation, HashSet<int>>()
            {
                { WallOrientation.Horizontal, horizontal },
                { WallOrientation.Vertical, vertical }
            };
        }

        private static void AddPossibleHorizontalWalls(ref HashSet<int> horizontal, int v)
        {
            /* Left */
            if (HorizontalWallVacant(v - 2, v - 1, v))
                horizontal.Add(v - 1);

            /* Middle */
            if (HorizontalWallVacant(v - 1, v, v + 1))
                horizontal.Add(v);

            /* Right */
            if (HorizontalWallVacant(v, v + 1, v + 2))
                horizontal.Add(v + 1);
        }

        private static void AddPossibleVerticalWalls(ref HashSet<int> vertical, int v)
        {
            /* Top */
            if (VerticalWallVacant(v - BOARD_EDGE_SIZE * 2, v - BOARD_EDGE_SIZE, v))
                vertical.Add(v - BOARD_EDGE_SIZE);

            /* Middle */
            if (VerticalWallVacant(v - BOARD_EDGE_SIZE, v, v + BOARD_EDGE_SIZE))
                vertical.Add(v);

            /* Bottom */
            if (VerticalWallVacant(v, v + BOARD_EDGE_SIZE, v + BOARD_EDGE_SIZE * 2))
                vertical.Add(v + BOARD_EDGE_SIZE);
        }

        private static int ManhattanDistance(int v, int w)
        {
            return Math.Abs(ToX(v) - ToX(w)) + Math.Abs(ToY(v) - ToY(w));
        }

        public static int ToBoardVertex(int v)
        {
            return v - (v / BOARD_EDGE_SIZE) - BOARD_EDGE_SIZE;
        }

        public static int ToOddVertex(int v)
        {
            return v + v % 2;
        }

        public static int ToBoardEdgeDistance(int v)
        {
            return v + BOARD_EDGE_SIZE;
        }

        public static int BoardSizeToOneDimension(int x, int y)
        {
            return BOARD_SIZE * y + x;
        }

        public static int ToWallX(int v)
        {
            return v % BOARD_EDGE_SIZE;
        }

        public static int ToWallY(int v)
        {
            return v / BOARD_SIZE;
        }

        public static int ToX(int v)
        {
            return v % BOARD_SIZE;
        }

        public static int ToY(int v)
        {
            return v / BOARD_SIZE;
        }
    }
}
