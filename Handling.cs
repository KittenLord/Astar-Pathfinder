using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixLib;

namespace AstarLearning
{


    public static class Pathfinder
    {


        public static List<Block> FindPath(Matrix matrix, Coordinates start, Coordinates end)
        {
            Block startBlock = matrix.GetBlock(start);
            Block targetBlock = matrix.GetBlock(end);

            List<Block> openSet = new List<Block>();
            HashSet<Block> closedSet = new HashSet<Block>();
            openSet.Add(startBlock);

            while (openSet.Count > 0)
            {
                Block currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].GetCost(Costs.FCost) < currentNode.GetCost(Costs.FCost) || (openSet[i].GetCost(Costs.FCost) == currentNode.GetCost(Costs.FCost) && openSet[i].GetCost(Costs.HCost) < currentNode.GetCost(Costs.HCost)))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetBlock)
                {
                    var path = RetracePath(startBlock, targetBlock);
                    return path;
                }

                foreach (var n in matrix.GetNeighbours(currentNode))
                {
                    if (n.tags.Contains("Obstacle") || closedSet.Contains(n)) continue;

                    if(currentNode.extraData.Count <= 0)
                    {
                        currentNode.extraData.Add(0);
                        currentNode.extraData.Add(0);
                        currentNode.extraData.Add(null);
                    }

                    if (n.extraData.Count <= 0)
                    {
                        n.extraData.Add(0);
                        n.extraData.Add(0);
                        n.extraData.Add(null);
                    }

                    int newCostToNeighbour = currentNode.GetCost(Costs.GCost) + GetDistance(currentNode, n);

                    if(newCostToNeighbour < n.GetCost(Costs.GCost) || !openSet.Contains(n))
                    {
                        n.extraData[ExtraData.gCost] = newCostToNeighbour;
                        n.extraData[ExtraData.hCost] = GetDistance(n, targetBlock);
                        n.extraData[ExtraData.parent] = currentNode;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                    }
                }
            }

            return null;
        }

        public static List<Block> RetracePath(Block start, Block end)
        {
            List<Block> path = new List<Block>();
            Block currentNode = end;

            while(currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.extraData[ExtraData.parent] as Block;
            }

            path.Reverse();

            return path;
        }

        public static int GetDistance(Block _1, Block _2)
        {
            int dx = Math.Abs(_1.coordinates.x - _2.coordinates.x);
            int dy = Math.Abs(_1.coordinates.y - _2.coordinates.y);

            return dx > dy ? ((14 * dy) + (10 * (dx - dy))) : ((14 * dx) + (10 * (dy - dx)));
        }

        class ExtraData
        {
            public const int gCost = 0;
            public const int hCost = 1;
            public const int parent = 2;
        }

        public static int ToInt(this object o)
        {
            return Convert.ToInt32(o);
        }

        public static T GetFromData<T>(this List<object> list, int id)
        {
            return (T)list[id];
        }

        public static int GetCost(this Block block, Costs costs)
        {
            switch (costs)
            {
                case Costs.FCost:

                    return block.extraData.GetFromData<int>(ExtraData.gCost) + block.extraData.GetFromData<int>(ExtraData.hCost);

                    break;
                case Costs.HCost:

                    return block.extraData.GetFromData<int>(ExtraData.hCost);

                    break;
                case Costs.GCost:

                    return block.extraData.GetFromData<int>(ExtraData.gCost);

                    break;
            }
            return -1;
        }
            
        public enum Costs
        {
            FCost, HCost, GCost
        }
    }
}
