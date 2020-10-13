using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Quoridor.AI.Color;
using Tile = Quoridor.AI.Tile;

namespace CustomAgent
{
    static class QuoridorPlayer
    {
        public static Player Self;
        public static Player Opponent;

        public static void CreatePlayer(Tile[,] tiles, Quoridor.AI.Player self, List<Quoridor.AI.Player> players)
        {
            Self = new Player(self);
            Opponent = new Player(players.Find(o => !o.Equals(self)));
            Self.MyTurn = true;

            for (int y = 0; y < QuoridorGraph.BOARD_SIZE; y++)
            {
                for (int x = 0; x < QuoridorGraph.BOARD_SIZE; x++)
                {
                    Color color = tiles[x, y].Color;

                    if (color == Self.Color)
                        Self.Goals.Add(QuoridorGraph.BoardSizeToOneDimension(x, y));

                    if (color == Opponent.Color)
                        Opponent.Goals.Add(QuoridorGraph.BoardSizeToOneDimension(x, y));
                }
            }
        }

        public static void RotateTurn()
        {
            Self.MyTurn = !Self.MyTurn;
            Opponent.MyTurn = !Opponent.MyTurn;
        }
    }
}
