using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _A3Pathfinding
{
    public class Grid : MonoBehaviour
    {
        private static Grid _instance;

        public bool displayGizmos;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public int gridSizeX, gridSizeY;
        public HashSet<Node> PathSet = new();
        public LayerMask obstacleLayer;
        public LayerMask humanLayer;
        public GameObject goalIndicator;
        private Node[,] _grid;
        private float _timeSinceLastGoalChange;

        private float _nodeDiameter;

        public Node GoalNode;

        public static Grid Instance => _instance
            ? _instance
            : _instance = FindObjectOfType<Grid>();

        public int MaxSize => gridSizeX * gridSizeY;

        private void Awake()
        {
            _nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
            goalIndicator.SetActive(false);
            CreateGrid();
        }

        private void OnDrawGizmos()
        {
            // Sits horizontally on the ground, so y == z in 3d space
            Gizmos.DrawWireCube(transform.position,
                new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (_grid == null || !displayGizmos) return;

            foreach (var node in _grid)
            {
                Gizmos.color = node.Clear ? Color.white : Color.red;
                if (node == GoalNode) Gizmos.color = Color.green;
                else if (PathSet.Contains(node)) Gizmos.color = Color.black;

                // draw a cube with a small offset so that they don't overlap each other
                var cubeSize = Vector3.one * (_nodeDiameter - .1f);
                Gizmos.DrawCube(node.WorldPoint,
                    cubeSize);
            }
        }

        private void CreateGrid()
        {
            _grid = new Node[gridSizeX, gridSizeY];
            var gridBottomLeftPosition = transform.position -
                                         Vector3.right * gridWorldSize.x / 2 -
                                         Vector3.forward * gridWorldSize.y / 2;

            // loop through grid
            for (var x = 0; x < gridSizeX; ++x)
            for (var y = 0; y < gridSizeX; ++y)
            {
                var worldPoint = gridBottomLeftPosition +
                                 Vector3.right * (x * _nodeDiameter +
                                                  nodeRadius) +
                                 Vector3.forward * (y * _nodeDiameter +
                                                    nodeRadius);

                _grid[x, y] = new Node(worldPoint, x, y);
            }
        }

        public void ChooseNewGoalNode()
        {
            if (GoalNode != null)
            {
                GoalNode.IsGoalNode = false;
                WriteTimeToFindGoalToFile();
            }

            GoalNode = null;
            goalIndicator.SetActive(false);

            // wait half a second
            Invoke(nameof(ContinueChooseNewGoalNode), .5f);
        }

        private const string TimeToFindGoalFileHeader =
            "MaxHumans, MaxChairs, TimeToFindGoal,\n";

        private static void WriteTimeToFindGoalToFile()
        {
            var timeToFindGoalPath =
                Application.dataPath + "/timeToFindGoal.csv";
            var timeToFindGoal = Instance._timeSinceLastGoalChange;

            var stringToAddToFile =
                $"{SpawnerManager.GetMaxHumans()}, {SpawnerManager.GetMaxChairs()}, {timeToFindGoal},\n";
            if (!File.Exists(timeToFindGoalPath))
            {
                File.WriteAllText(timeToFindGoalPath,
                    TimeToFindGoalFileHeader + stringToAddToFile);
            }
            else
            {
                File.AppendAllText(timeToFindGoalPath, stringToAddToFile);
            }
        }

        public void ContinueChooseNewGoalNode()
        {
            var goalX = Random.Range(0, gridSizeX);
            var goalY = Random.Range(0, gridSizeY);
            GoalNode = _grid[goalX, goalY];
            GoalNode.IsGoalNode = true;

            // Move goal indicator
            var goalPosition = GoalNode.WorldPoint;
            goalPosition += Vector3.up * .2f;
            goalIndicator.transform.position = goalPosition;
            goalIndicator.SetActive(true);

            PathSet = new HashSet<Node>();
        }

        public IEnumerable<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();

            for (var x = -1; x < 2; ++x)
            for (var y = -1; y < 2; ++y)
            {
                // skip the current node
                if (x == 0 && y == 0) continue;

                var checkX = node.X + x;
                var checkY = node.Y + y;

                // check if the node is within the grid
                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(_grid[checkX, checkY]);
            }

            return neighbours;
        }

        /// <summary>
        ///     Return node that the world position sits on top of
        /// </summary>
        /// <param name="position">World position</param>
        /// <returns>Node in grid that the position is inside of</returns>
        public Node GetNodeFromPositionOnGrid(Vector3 position)
        {
            var percentX = Mathf.Clamp01((position.x + gridWorldSize.x / 2) /
                                         gridWorldSize.x);
            var percentY = Mathf.Clamp01((position.z + gridWorldSize.y / 2) /
                                         gridWorldSize.y);
            var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            var y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return _grid[x, y];
        }

        public Node GetNodeFromGridPosition(int x, int y)
        {
            return _grid[x, y];
        }

        public Node GetNodeFromGridPosition(Vector2Int gridPosition)
        {
            return _grid[gridPosition.x, gridPosition.y];
        }

        private void Update()
        {
            _timeSinceLastGoalChange += Time.deltaTime;
            if (GoalNode is { Clear: false } && !GoalNode.ClearOfHumans())
                WinManager.Win();
        }
    }
}