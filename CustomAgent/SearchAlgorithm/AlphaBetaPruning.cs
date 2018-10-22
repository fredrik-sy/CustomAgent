using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class AlphaBetaPruning
    {
        public static int Evaluate(int depth, int alpha, int beta, bool maximizingPlayer)
        {
            Dijkstra dijkstraSelf = new Dijkstra(QuoridorGraph.Graph, PlayerExtension.Self);
            Dijkstra dijkstraOpponent = new Dijkstra(QuoridorGraph.Graph, PlayerExtension.Opponent);

            dijkstraSelf.CreatePath();
            dijkstraOpponent.CreatePath(dijkstraSelf);

            /* Wall Isolation */
            if (dijkstraSelf.Path == null || dijkstraOpponent.Path == null)
                return maximizingPlayer ? int.MaxValue : int.MinValue;

            if (ReachedTerminal(dijkstraSelf, dijkstraOpponent, depth, maximizingPlayer, out int heuristicValue))
                return heuristicValue;

            if (maximizingPlayer)
            {
                int v = dijkstraSelf.Path.Peek();
                int value = int.MinValue;

                PlayerExtension.Self.Move(v);
                value = Max(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                alpha = Max(alpha, value);
                PlayerExtension.Self.RevertMove();

                if (alpha >= beta)
                    return value;

                if (PlayerExtension.Self.HasWall())
                {
                    if (dijkstraSelf.Path.Count > dijkstraOpponent.Path.Count)
                    {
                        var wallPositions = QuoridorGraph.WallPossibilities(dijkstraOpponent.Path);

                        #region EvaluateHorizontal
                        foreach (int horizontalWallPosition in wallPositions[WallOrientation.Horizontal])
                        {
                            PlayerExtension.Self.PlaceHorizontalWall(horizontalWallPosition);
                            value = Max(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                            alpha = Max(alpha, value);
                            PlayerExtension.Self.RemoveHorizontalWall(horizontalWallPosition);

                            if (alpha >= beta)
                                return value;
                        }
                        #endregion

                        #region EvaluateVertical
                        foreach (int verticalWallPosition in wallPositions[WallOrientation.Vertical])
                        {
                            PlayerExtension.Self.PlaceVerticalWall(verticalWallPosition);
                            value = Max(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                            alpha = Max(alpha, value);
                            PlayerExtension.Self.RemoveVerticalWall(verticalWallPosition);

                            if (alpha >= beta)
                                return value;
                        }
                        #endregion
                    }
                }

                return value;
            }
            else
            {
                int v = dijkstraOpponent.Path.Peek();
                int value = int.MaxValue;

                PlayerExtension.Opponent.Move(v);
                value = Min(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                beta = Min(beta, value);
                PlayerExtension.Opponent.RevertMove();

                if (alpha >= beta)
                    return value;

                if (PlayerExtension.Opponent.HasWall())
                {
                    if (dijkstraOpponent.Path.Count > dijkstraSelf.Path.Count)
                    {
                        var wallPositions = QuoridorGraph.WallPossibilities(dijkstraSelf.Path);

                        #region EvaluateHorizontal
                        foreach (int horizontalWallPosition in wallPositions[WallOrientation.Horizontal])
                        {
                            PlayerExtension.Opponent.PlaceHorizontalWall(horizontalWallPosition);
                            value = Min(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                            beta = Min(beta, value);
                            PlayerExtension.Opponent.RemoveHorizontalWall(horizontalWallPosition);

                            if (alpha >= beta)
                                return value;
                        }
                        #endregion

                        #region EvaluateVertical
                        foreach (int verticalWallPosition in wallPositions[WallOrientation.Vertical])
                        {
                            PlayerExtension.Opponent.PlaceVerticalWall(verticalWallPosition);
                            value = Min(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                            beta = Min(beta, value);
                            PlayerExtension.Opponent.RemoveVerticalWall(verticalWallPosition);

                            if (alpha >= beta)
                                return value;
                        }
                        #endregion
                    }
                }

                return value;
            }
        }

        private static bool ReachedTerminal(Dijkstra dijkstraSelf, Dijkstra dijkstraOpponent, int depth, bool maximizingPlayer, out int heuristicValue)
        {
            heuristicValue = 0;

            if (depth == 0 ||
                PlayerExtension.Self.Goals().Contains(PlayerExtension.Self.Position()) ||
                PlayerExtension.Opponent.Goals().Contains(PlayerExtension.Opponent.Position()))
            {
                heuristicValue += dijkstraOpponent.Path.Count - dijkstraSelf.Path.Count;
                return true;
            }

            return false;
        }

        private static int Max(int value1, int value2)
        {
            return value1 < value2 ? value2 : value1;
        }

        private static int Min(int value1, int value2)
        {
            return value1 < value2 ? value1 : value2;
        }
    }
}
