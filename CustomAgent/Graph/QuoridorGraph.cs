using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class QuoridorGraph
    {
        public const int BOARD_SIZE = 9;
        public const int SQUARE_SPACES = 81;

        public static Graph Graph;

        public static void CreateGraph(Tile[,] tiles, bool[,] horizontalWall, bool[,] verticalWall)
        {
            Graph = new Graph(SQUARE_SPACES);

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (x > 0 && !verticalWall[x - 1, y])
                        Graph.AddUndirectedEdge((x - 1) + BOARD_SIZE * y, x + BOARD_SIZE * y);

                    if (y > 0 && !horizontalWall[x, y - 1])
                        Graph.AddUndirectedEdge(x + BOARD_SIZE * (y - 1), x + BOARD_SIZE * y);
                }
            }
        }

        #region PlaceWallAction
        public static bool HorizontalWallVacant(int v)
        {
            if (v < 0 || v >= SQUARE_SPACES ||
                ToX(v) > BOARD_SIZE - 2 ||
                ToY(v) > BOARD_SIZE - 2)
                return false;

            return Graph.IsAdj(v, v + BOARD_SIZE) && Graph.IsAdj(v + 1, (v + 1) + BOARD_SIZE);
        }

        public static bool VerticalWallVacant(int v)
        {
            if (v < 0 || v >= SQUARE_SPACES ||
                ToX(v) > BOARD_SIZE - 2 ||
                ToY(v) > BOARD_SIZE - 2)
                return false;

            return Graph.IsAdj(v, v + 1) && Graph.IsAdj(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
        }

        public static bool PlaceHorizontalWall(int v)
        {
            if (HorizontalWallVacant(v))
            {
                Graph.RemoveUndirectedEdge(v, v + BOARD_SIZE);
                Graph.RemoveUndirectedEdge(v + 1, (v + 1) + BOARD_SIZE);
                return true;
            }

            return false;
        }

        public static bool PlaceVerticalWall(int v)
        {
            if (VerticalWallVacant(v))
            {
                Graph.RemoveUndirectedEdge(v, v + 1);
                Graph.RemoveUndirectedEdge(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
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

            Graph.AddUndirectedEdge(v, v + BOARD_SIZE);
            Graph.AddUndirectedEdge(v + 1, (v + 1) + BOARD_SIZE);
        }

        public static void RemoveVerticalWall(int v)
        {
            if (v < 0 || v >= SQUARE_SPACES ||
                ToX(v) > BOARD_SIZE - 2 ||
                ToY(v) > BOARD_SIZE - 2)
                throw new ArgumentException();

            Graph.AddUndirectedEdge(v, v + 1);
            Graph.AddUndirectedEdge(v + BOARD_SIZE, (v + 1) + BOARD_SIZE);
        }
        #endregion

        public static Dictionary<WallOrientation, HashSet<int>> WallPossibilities(List<int> cheapestPath, int beginCount = SQUARE_SPACES, int endCount = 0)
        {
            HashSet<int> horizontal = new HashSet<int>();
            HashSet<int> vertical = new HashSet<int>();

            foreach (int v in cheapestPath.ForEachReverse(beginCount, endCount))
            {
                #region PlaceHorizontalWall
                if (HorizontalWallVacant(v))
                    horizontal.Add(v);

                if (HorizontalWallVacant(v - 1))
                    horizontal.Add(v - 1);

                if (HorizontalWallVacant(v - BOARD_SIZE))
                    horizontal.Add(v - BOARD_SIZE);

                if (HorizontalWallVacant((v - 1) - BOARD_SIZE))
                    horizontal.Add((v - 1) - BOARD_SIZE);
                #endregion

                #region PlaceVerticalWall
                if (VerticalWallVacant(v))
                    vertical.Add(v);

                if (VerticalWallVacant(v - BOARD_SIZE))
                    vertical.Add(v - BOARD_SIZE);

                if (VerticalWallVacant(v - 1))
                    vertical.Add(v - 1);

                if (VerticalWallVacant((v - 1) - BOARD_SIZE))
                    vertical.Add((v - 1) - BOARD_SIZE);
                #endregion
            }

            return new Dictionary<WallOrientation, HashSet<int>>()
            {
                { WallOrientation.Horizontal, horizontal },
                { WallOrientation.Vertical, vertical }
            };
        }

        public static int ToOneDimension(int x, int y)
        {
            return BOARD_SIZE * y + x;
        }

        public static Point ToTwoDimension(int v)
        {
            return new Point(v % BOARD_SIZE, v / BOARD_SIZE);
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
