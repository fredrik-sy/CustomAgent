﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class AlphaBetaPruning
    {
        public static int Evaluate(int depth, int alpha, int beta, bool maximizingPlayer)
        {
            Dijkstra dijkstraSelf = new Dijkstra(QuoridorGraph.BoardGraph, PlayerExtension.Self);
            Dijkstra dijkstraOpponent = new Dijkstra(QuoridorGraph.BoardGraph, PlayerExtension.Opponent);

            dijkstraSelf.CreatePath();
            dijkstraOpponent.CreatePath(dijkstraSelf);

            /* Wall Isolation */
            if (dijkstraSelf.Path == null || dijkstraOpponent.Path == null)
                return maximizingPlayer ? int.MaxValue : int.MinValue;

            /* Corner Isolation */
            if (dijkstraSelf.Path.Count == 0 || dijkstraOpponent.Path.Count == 0)
                return 0;

            if (ReachedTerminal(depth, maximizingPlayer, out int heuristicValue))
                return heuristicValue;

            if (maximizingPlayer)
            {
                int value = int.MinValue;

                if (dijkstraSelf.Path.Count > 0)
                {
                    int v = dijkstraSelf.Path.Peek();

                    PlayerExtension.Self.Move(v);
                    value = Max(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                    alpha = Max(alpha, value);
                    PlayerExtension.Self.RevertMove();

                    if (alpha >= beta)
                        return value;
                }

                if (PlayerExtension.Self.NumberOfWalls() > 0)
                {
                    if (dijkstraSelf.Path.Count > dijkstraOpponent.Path.Count || dijkstraSelf.OpponentCloseBy)
                    {
                        var wallPositions = QuoridorGraph.WallPossibilities(dijkstraOpponent.Path, CustomAgent.LIMIT_WALL_BEGIN_COUNT, CustomAgent.LIMIT_WALL_END_COUNT);

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

                return value == int.MinValue ? int.MaxValue : value;
            }
            else
            {
                int value = int.MaxValue;

                if (dijkstraOpponent.Path.Count > 0)
                {
                    int v = dijkstraOpponent.Path.Peek();

                    PlayerExtension.Opponent.Move(v);
                    value = Min(value, Evaluate(depth - 1, alpha, beta, !maximizingPlayer));
                    beta = Min(beta, value);
                    PlayerExtension.Opponent.RevertMove();

                    if (alpha >= beta)
                        return value;
                }

                if (PlayerExtension.Opponent.NumberOfWalls() > 0)
                {
                    if (dijkstraOpponent.Path.Count > dijkstraSelf.Path.Count || dijkstraOpponent.OpponentCloseBy)
                    {
                        var wallPositions = QuoridorGraph.WallPossibilities(dijkstraSelf.Path, CustomAgent.LIMIT_WALL_BEGIN_COUNT, CustomAgent.LIMIT_WALL_END_COUNT);

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

                return value == int.MaxValue ? int.MinValue : value;
            }
        }

        private static bool ReachedTerminal(int depth, bool maximizingPlayer, out int heuristicValue)
        {

            if (depth == 0 ||
                PlayerExtension.Self.Goals().Contains(PlayerExtension.Self.Position()) ||
                PlayerExtension.Opponent.Goals().Contains(PlayerExtension.Opponent.Position()))
            {
                Dijkstra dijkstraSelf = new Dijkstra(QuoridorGraph.BoardGraph, PlayerExtension.Self);
                Dijkstra dijkstraOpponent = new Dijkstra(QuoridorGraph.BoardGraph, PlayerExtension.Opponent);

                dijkstraSelf.CreatePath();
                dijkstraOpponent.CreatePath();

                /* Wall Isolation */
                if (dijkstraSelf.Path == null || dijkstraOpponent.Path == null)
                    heuristicValue = maximizingPlayer ? int.MaxValue : int.MinValue;
                else
                    heuristicValue = dijkstraOpponent.Path.Count - dijkstraSelf.Path.Count;

                return true;
            }

            heuristicValue = 0;
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
