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

        public static Dictionary<WallOrientation, HashSet<int>> WallPossibilities(List<int> cheapestPath)
        {
            HashSet<int> horizontal = new HashSet<int>();
            HashSet<int> vertical = new HashSet<int>();

            foreach (int v in cheapestPath.ForEachReverse(CustomAgent.LIMIT_WALL_BEGIN_COUNT, CustomAgent.LIMIT_WALL_END_COUNT))
            {
                #region PlaceHorizontalWall
                if (PlayerExtension.PlaceHorizontalWall(v))
                {
                    horizontal.Add(v);
                    PlayerExtension.RemoveHorizontalWall(v);
                }

                if (PlayerExtension.PlaceHorizontalWall(v - 1))
                {
                    horizontal.Add(v - 1);
                    PlayerExtension.RemoveHorizontalWall(v - 1);
                }

                if (PlayerExtension.PlaceHorizontalWall(v - BOARD_SIZE))
                {
                    horizontal.Add(v - BOARD_SIZE);
                    PlayerExtension.RemoveHorizontalWall(v - BOARD_SIZE);
                }

                if (PlayerExtension.PlaceHorizontalWall((v - 1) - BOARD_SIZE))
                {
                    horizontal.Add((v - 1) - BOARD_SIZE);
                    PlayerExtension.RemoveHorizontalWall((v - 1) - BOARD_SIZE);
                }
                #endregion

                #region PlaceVerticalWall
                if (PlayerExtension.PlaceVerticalWall(v))
                {
                    vertical.Add(v);
                    PlayerExtension.RemoveVerticalWall(v);
                }

                if (PlayerExtension.PlaceVerticalWall(v - BOARD_SIZE))
                {
                    vertical.Add(v - BOARD_SIZE);
                    PlayerExtension.RemoveVerticalWall(v - BOARD_SIZE);
                }

                if (PlayerExtension.PlaceVerticalWall(v - 1))
                {
                    vertical.Add(v - 1);
                    PlayerExtension.RemoveVerticalWall(v - 1);
                }

                if (PlayerExtension.PlaceVerticalWall((v - 1) - BOARD_SIZE))
                {
                    vertical.Add((v - 1) - BOARD_SIZE);
                    PlayerExtension.RemoveVerticalWall((v - 1) - BOARD_SIZE);
                }
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
