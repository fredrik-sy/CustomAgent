using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        /* Depth >= 0 */
        public const int ALPHA_BETA_PRUNING_DEPTH = 4;

        /* Ignore Middle Wall Element If List.Count > LIMIT_WALL_BEGIN_COUNT + LIMIT_WALL_END_COUNT */
        public const int LIMIT_WALL_BEGIN_COUNT = 4;
        public const int LIMIT_WALL_END_COUNT = 2;

        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            QuoridorGraph.CreateGraph(status.Tiles, status.HorizontalWall, status.VerticalWall);
            PlayerExtension.Self = status.Self;
            PlayerExtension.Opponent = status.Players.Find(o => !o.Equals(status.Self));
            PlayerExtension.Initialize(status.Tiles);
            return NextAction();
        }

        public static Action NextAction()
        {
            Action action = default(Action);
            int actionScore = int.MinValue;
            int score;

            Dijkstra dijkstraSelf = new Dijkstra(QuoridorGraph.Graph, PlayerExtension.Self);
            Dijkstra dijkstraOpponent = new Dijkstra(QuoridorGraph.Graph, PlayerExtension.Opponent);

            dijkstraSelf.CreatePath();

            var selfUntouchedWallPositions = QuoridorGraph.WallPossibilities(dijkstraSelf.Path, LIMIT_WALL_BEGIN_COUNT, LIMIT_WALL_END_COUNT);

            dijkstraOpponent.CreatePath(dijkstraSelf);

            if (PlayerExtension.Self.HasWall())
            {
                if (dijkstraSelf.Path.Count > dijkstraOpponent.Path.Count || dijkstraSelf.OpponentCloseBy)
                {
                    var wallPositions = dijkstraSelf.OpponentCloseBy ? QuoridorGraph.WallPossibilities(dijkstraOpponent.Path, LIMIT_WALL_BEGIN_COUNT, LIMIT_WALL_END_COUNT)
                        .Join(selfUntouchedWallPositions)
                        : QuoridorGraph.WallPossibilities(dijkstraOpponent.Path, LIMIT_WALL_BEGIN_COUNT, LIMIT_WALL_END_COUNT);

                    #region EvaluateHorizontal
                    foreach (int horizontalWallPosition in wallPositions[WallOrientation.Horizontal])
                    {
                        PlayerExtension.Self.PlaceHorizontalWall(horizontalWallPosition);

                        score = AlphaBetaPruning.Evaluate(ALPHA_BETA_PRUNING_DEPTH, int.MinValue, int.MaxValue, false);

                        if (actionScore < score)
                        {
                            action = new PlaceWallAction(QuoridorGraph.ToX(horizontalWallPosition), QuoridorGraph.ToY(horizontalWallPosition), WallOrientation.Horizontal);
                            actionScore = score;
                        }

                        PlayerExtension.Self.RemoveHorizontalWall(horizontalWallPosition);
                    }
                    #endregion

                    #region EvaluateVertical
                    foreach (int verticalWallPosition in wallPositions[WallOrientation.Vertical])
                    {
                        PlayerExtension.Self.PlaceVerticalWall(verticalWallPosition);

                        score = AlphaBetaPruning.Evaluate(ALPHA_BETA_PRUNING_DEPTH, int.MinValue, int.MaxValue, false);

                        if (actionScore < score)
                        {
                            action = new PlaceWallAction(QuoridorGraph.ToX(verticalWallPosition), QuoridorGraph.ToY(verticalWallPosition), WallOrientation.Vertical);
                            actionScore = score;
                        }

                        PlayerExtension.Self.RemoveVerticalWall(verticalWallPosition);
                    }
                    #endregion
                }
            }

            #region EvaluateMove
            int v = dijkstraSelf.Path.Peek();

            PlayerExtension.Self.Move(v);

            score = AlphaBetaPruning.Evaluate(ALPHA_BETA_PRUNING_DEPTH, int.MinValue, int.MaxValue, false);

            if (actionScore < score)
            {
                action = new MoveAction(QuoridorGraph.ToX(v), QuoridorGraph.ToY(v));
                actionScore = score;
            }

            PlayerExtension.Self.RevertMove();
            #endregion

            //Debug.Print(action.GetType().IsAssignableFrom(typeof(MoveAction)) ?
            //    ((Func<MoveAction, string>)((MoveAction moveAction) => "(" + moveAction.Column + ", " + moveAction.Row + ")"))((MoveAction)action) :
            //    ((Func<PlaceWallAction, string>)((PlaceWallAction placeWallAction) => "(" + placeWallAction.Column + ", " + placeWallAction.Row + ") " + placeWallAction.WallAlignment))((PlaceWallAction)action));

            return action;
        }
    }
}