using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    static class PlayerExtension
    {
        public static Player Self;
        public static Player Opponent;

        #region TempData
        private static Dictionary<Color, bool> TurnHistory = new Dictionary<Color, bool>()
        {
            { Color.Blue, false },
            { Color.Red, false }
        };

        private static Dictionary<Color, List<int>> GoalLocation = new Dictionary<Color, List<int>>()
        {
            { Color.Blue, new List<int>() },
            { Color.Red, new List<int>() }
        };

        private static Dictionary<Color, Stack<int>> MoveHistory = new Dictionary<Color, Stack<int>>()
        {
            { Color.Blue, new Stack<int>() },
            { Color.Red, new Stack<int>() }
        };

        private static Dictionary<Color, int> WallCount = new Dictionary<Color, int>()
        {
            { Color.Blue, 10 },
            { Color.Red, 10 }
        };
        #endregion

        public static void Initialize(Tile[,] tiles)
        {
            GoalLocation[Color.Blue].Clear();
            GoalLocation[Color.Red].Clear();
            MoveHistory[Color.Blue].Clear();
            MoveHistory[Color.Red].Clear();
            TurnHistory[Self.Color] = true;
            TurnHistory[Opponent.Color] = false;
            WallCount[Self.Color] = Self.NumberOfWalls;
            WallCount[Opponent.Color] = Opponent.NumberOfWalls;

            for (int y = 0; y < QuoridorGraph.BOARD_SIZE; y++)
            {
                for (int x = 0; x < QuoridorGraph.BOARD_SIZE; x++)
                {
                    Tile tile = tiles[x, y];

                    if (tile.Color != Color.None)
                        GoalLocation[tile.Color].Add(QuoridorGraph.ToOneDimension(x, y));
                }
            }
        }

        public static bool Active(this Player player)
        {
            return TurnHistory[player.Color];
        }

        public static void Move(this Player player, int v)
        {
            MoveHistory[player.Color].Push(v);
            RotateTurn();
        }

        public static void RevertMove(this Player player)
        {
            MoveHistory[player.Color].Pop();
            RotateTurn();
        }

        public static List<int> Goals(this Player player)
        {
            return GoalLocation[player.Color];
        }
        
        public static int Position(this Player player)
        {
            return MoveHistory[player.Color].Count == 0 ? QuoridorGraph.BOARD_SIZE * player.Position.Y + player.Position.X : MoveHistory[player.Color].Peek();
        }

        private static void RotateTurn()
        {
            bool b = TurnHistory[Color.Blue];
            TurnHistory[Color.Blue] = TurnHistory[Color.Red];
            TurnHistory[Color.Red] = b;
        }

        #region Wall
        public static int NumberOfWalls(this Player player)
        {
            return WallCount[player.Color];
        }

        public static bool HasWall(this Player player)
        {
            return WallCount[player.Color] > 0;
        }

        public static bool PlaceHorizontalWall(this Player player, int v)
        {
            if (PlaceHorizontalWall(v))
            {
                WallCount[player.Color] = WallCount[player.Color] - 1;
                RotateTurn();
                return true;
            }

            return false;
        }

        public static bool PlaceHorizontalWall(int v)
        {
            if (v < 0 || v >= QuoridorGraph.SQUARE_SPACES ||
                QuoridorGraph.ToX(v) > QuoridorGraph.BOARD_SIZE - 2 ||
                QuoridorGraph.ToY(v) > QuoridorGraph.BOARD_SIZE - 2)
                return false;

            if (QuoridorGraph.Graph.IsAdj(v, v + QuoridorGraph.BOARD_SIZE) && QuoridorGraph.Graph.IsAdj(v + 1, (v + 1) + QuoridorGraph.BOARD_SIZE))
            {
                QuoridorGraph.Graph.RemoveUndirectedEdge(v, v + QuoridorGraph.BOARD_SIZE);
                QuoridorGraph.Graph.RemoveUndirectedEdge(v + 1, (v + 1) + QuoridorGraph.BOARD_SIZE);
                return true;
            }

            return false;
        }

        public static bool PlaceVerticalWall(this Player player, int v)
        {
            if (PlaceVerticalWall(v))
            {
                WallCount[player.Color] = WallCount[player.Color] - 1;
                RotateTurn();
                return true;
            }

            return false;
        }

        public static bool PlaceVerticalWall(int v)
        {
            if (v < 0 || v >= QuoridorGraph.SQUARE_SPACES ||
                QuoridorGraph.ToX(v) > QuoridorGraph.BOARD_SIZE - 2 ||
                QuoridorGraph.ToY(v) > QuoridorGraph.BOARD_SIZE - 2)
                return false;

            if (QuoridorGraph.Graph.IsAdj(v, v + 1) && QuoridorGraph.Graph.IsAdj(v + QuoridorGraph.BOARD_SIZE, (v + 1) + QuoridorGraph.BOARD_SIZE))
            {
                QuoridorGraph.Graph.RemoveUndirectedEdge(v, v + 1);
                QuoridorGraph.Graph.RemoveUndirectedEdge(v + QuoridorGraph.BOARD_SIZE, (v + 1) + QuoridorGraph.BOARD_SIZE);
                return true;
            }

            return false;
        }

        public static void RemoveHorizontalWall(this Player player, int v)
        {
            RemoveHorizontalWall(v);
            WallCount[player.Color] = WallCount[player.Color] + 1;
            RotateTurn();
        }

        public static void RemoveHorizontalWall(int v)
        {
            if (v < 0 || v >= QuoridorGraph.SQUARE_SPACES ||
                QuoridorGraph.ToX(v) > QuoridorGraph.BOARD_SIZE - 2 ||
                QuoridorGraph.ToY(v) > QuoridorGraph.BOARD_SIZE - 2)
                throw new ArgumentException();
            
            QuoridorGraph.Graph.AddUndirectedEdge(v, v + QuoridorGraph.BOARD_SIZE);
            QuoridorGraph.Graph.AddUndirectedEdge(v + 1, (v + 1) + QuoridorGraph.BOARD_SIZE);
        }

        public static void RemoveVerticalWall(this Player player, int v)
        {
            RemoveVerticalWall(v);
            WallCount[player.Color] = WallCount[player.Color] + 1;
            RotateTurn();
        }

        public static void RemoveVerticalWall(int v)
        {
            if (v < 0 || v >= QuoridorGraph.SQUARE_SPACES ||
                QuoridorGraph.ToX(v) > QuoridorGraph.BOARD_SIZE - 2 ||
                QuoridorGraph.ToY(v) > QuoridorGraph.BOARD_SIZE - 2)
                throw new ArgumentException();

            QuoridorGraph.Graph.AddUndirectedEdge(v, v + 1);
            QuoridorGraph.Graph.AddUndirectedEdge(v + QuoridorGraph.BOARD_SIZE, (v + 1) + QuoridorGraph.BOARD_SIZE);
        }
        #endregion
    }
}
