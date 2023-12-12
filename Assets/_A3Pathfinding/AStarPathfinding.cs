using System.Collections.Generic;
using System.Linq;

namespace _A3Pathfinding
{
    public class AStarPathfinding : Pathfinding
    {
        private static AStarPathfinding _instance;

        public static AStarPathfinding Instance => _instance
            ? _instance
            : _instance = FindObjectOfType<AStarPathfinding>();

        /// <inheritdoc />
        public override IEnumerable<Node> FindPath(
            Node startNode,
            Node goalNode)
        {
            var openSet = new MinHeap<Node>(Grid.Instance.MaxSize);
            var closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            // While we still have explorable nodes
            while (openSet.Count > 0)
            {
                // Get the node with the lowest fCost
                var currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                // If we made it to the finish, retrace the path by accessing parents
                if (currentNode == goalNode)
                    return RetracePath(startNode, goalNode);

                // Loop through neighbours
                var neighbours = Grid.Instance.GetNeighbours(
                    currentNode);
                foreach (var neighbour in neighbours)
                {
                    // Neighbour is not explorable
                    if (!neighbour.Clear || closedSet.Contains(neighbour))
                        continue;

                    // Calculate new FCost
                    var newCost = currentNode.GCost +
                                  currentNode.GetDistanceToNode(neighbour);

                    // If the new cost is greater than the neighbour's gCost and the neighbour is in the open set, skip
                    if (newCost >= neighbour.GCost &&
                        openSet.Contains(neighbour)) continue;

                    // Update neighbour
                    neighbour.GCost = newCost;
                    neighbour.HCost =
                        currentNode.GetDistanceToNode(neighbour);
                    neighbour.Parent = currentNode;

                    // Add neighbour to open set if it's not already there
                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }

            return Enumerable.Empty<Node>();
        }

        /// <summary>
        ///     Retrace the path from the end node to the start node by accessing parents
        ///     assigned during pathfinding.
        /// </summary>
        /// <param name="startNode">Start of path</param>
        /// <param name="endNode">End of path</param>
        /// <returns>Final path from start to end</returns>
        private static IEnumerable<Node> RetracePath(Node startNode,
            Node endNode)
        {
            var pathFromEnd = new List<Node>();
            var currentNode = endNode;

            // Loop through parents until we reach the start node
            while (currentNode != startNode)
            {
                pathFromEnd.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // Reverse list
            pathFromEnd.Reverse();

            return pathFromEnd;
        }
    }
}