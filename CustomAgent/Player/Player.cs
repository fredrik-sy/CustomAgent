using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Quoridor.AI.Color;

namespace CustomAgent
{
    class Player
    {
        private Stack<int> timeline;

        public Player(Quoridor.AI.Player player)
        {
            Color = player.Color;
            NumberOfWalls = player.NumberOfWalls;
            Goals = new List<int>();
            timeline = new Stack<int>();
            timeline.Push(QuoridorGraph.BoardGraph.ToOneDimension(player.Position));
        }

        public Color Color { get; private set; }

        public List<int> Goals { get; private set; }

        public bool MyTurn { get; set; }

        public int NumberOfWalls { get; private set; }

        public int Position { get => timeline.Peek(); }

        #region MoveAction
        public void Move(int v)
        {
            timeline.Push(v);
            QuoridorPlayer.RotateTurn();
        }

        public void RevertMove()
        {
            timeline.Pop();
            QuoridorPlayer.RotateTurn();
        }
        #endregion

        #region WallAction
        public bool PlaceHorizontalWall(int v)
        {
            if (QuoridorGraph.PlaceHorizontalWall(v))
            {
                NumberOfWalls--;
                QuoridorPlayer.RotateTurn();
                return true;
            }

            return false;
        }

        public bool PlaceVerticalWall(int v)
        {
            if (QuoridorGraph.PlaceVerticalWall(v))
            {
                NumberOfWalls--;
                QuoridorPlayer.RotateTurn();
                return true;
            }

            return false;
        }

        public void RemoveHorizontalWall(int v)
        {
            QuoridorGraph.RemoveHorizontalWall(v);
            NumberOfWalls++;
            QuoridorPlayer.RotateTurn();
        }

        public void RemoveVerticalWall(int v)
        {
            QuoridorGraph.RemoveVerticalWall(v);
            NumberOfWalls++;
            QuoridorPlayer.RotateTurn();
        }
        #endregion
    }
}
