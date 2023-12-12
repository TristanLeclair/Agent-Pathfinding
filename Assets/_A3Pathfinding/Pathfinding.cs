using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _A3Pathfinding
{
    public abstract class Pathfinding : MonoBehaviour
    {
        /// <summary>
        /// Find a path between two positions.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="goalNode"></param>
        /// <returns>An enumerable of nodes that describes the shortest path between the two</returns>
        public abstract IEnumerable<Node> FindPath(
            Node startNode,
            Node goalNode);
    }
}