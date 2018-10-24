using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class JumpPointSearch
    {
        public static void FindPath(Player player)
        {
            int[] closedSet = new int[QuoridorGraph.SQUARE_SPACES];
            PriorityQueue<int> openSet = new PriorityQueue<int>(81);
            int[] cameFrom = new int[QuoridorGraph.SQUARE_SPACES];
            int[] gScore = new int[QuoridorGraph.SQUARE_SPACES];
            int[] fScore = new int[QuoridorGraph.SQUARE_SPACES];

            gScore.Fill(int.MaxValue);
            fScore.Fill(int.MaxValue);

            int start = player.Position();
            List<int> goals = player.Goals();

            gScore[start] = 0;
            fScore[start] = HeuristicCostEstimate(start, goals);

            while (!openSet.Empty)
            {
                int current = openSet.DeleteMin();

            }
            
        //current:= the node in openSet having the lowest fScore[] value
        //if current = goal
        //    return reconstruct_path(cameFrom, current)

        //openSet.Remove(current)
        //closedSet.Add(current)

        //for each neighbor of current
        //    if neighbor in closedSet
        //        continue		// Ignore the neighbor which is already evaluated.

        //    // The distance from start to a neighbor
        //tentative_gScore:= gScore[current] + dist_between(current, neighbor)

        //    if neighbor not in openSet	// Discover a new node
        //        openSet.Add(neighbor)
        //    else if tentative_gScore >= gScore[neighbor]
        //        continue		// This is not a better path.

        //    // This path is the best until now. Record it!
        //    cameFrom[neighbor] := current
        //    gScore[neighbor] := tentative_gScore
        //    fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)

        }

        private static int HeuristicCostEstimate(int p, List<int> qList)
        {
            int h = int.MaxValue;

            foreach (int q in qList)
            {
                int d = HeuristicCostEstimate(p, q);

                if (d < h)
                    h = d;
            }

            return h;
        }

        private static int HeuristicCostEstimate(int p, int q)
        {
            return Math.Abs(QuoridorGraph.ToX(p) - QuoridorGraph.ToX(q)) + Math.Abs(QuoridorGraph.ToY(p) - QuoridorGraph.ToY(q));
        }
    }
}
