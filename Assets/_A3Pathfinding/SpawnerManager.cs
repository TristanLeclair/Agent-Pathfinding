using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _A3Pathfinding
{
    public class SpawnerManager : MonoBehaviour
    {
        private static SpawnerManager _instance;

        public int maxHumans = 5;

        public int maxChairs = 10;

        public GameObject humanPrefab;
        public GameObject chairPrefab;

        public Human[] humans;
        public Chair[] chairs;

        public bool readMaxFromFile;

        public static SpawnerManager Instance => _instance
            ? _instance
            : _instance = FindObjectOfType<SpawnerManager>();

        private void Awake()
        {
            if (readMaxFromFile) ReadMaxFromFile();
            humans = new Human[maxHumans];
            chairs = new Chair[maxChairs];
        }

        private void ReadMaxFromFile()
        {
            // open file
            var path = Application.dataPath + "/max.csv";
            var lines = System.IO.File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                maxHumans = 4;
                maxChairs = 4;
                Debug.Log("max.csv is empty");
                return;
            }

            var lastLine = lines[^1];
            var split = lastLine.Split(',');
            maxHumans = int.Parse(split[0]);
            maxChairs = int.Parse(split[1]);
            Debug.Log($"maxHumans: {maxHumans}, maxChairs: {maxChairs}");
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (maxHumans + maxChairs > Grid.Instance.gridSizeX *
                Grid.Instance.gridSizeY)
            {
                var msg =
                    $"Not enough space for all humans and chairs {maxHumans} + {maxChairs} > {Grid.Instance.gridSizeX} * {Grid.Instance.gridSizeY}";
                Debug.LogError(msg);
            }

            var positions = SelectRandomGridPositions(maxHumans + maxChairs);
            for (var i = 0; i < maxHumans; i++)
            {
                var humanGameObject = Instantiate(humanPrefab, transform);
                var human = humanGameObject.GetComponent<Human>();
                var (_, nextPos) = positions.Pop();
                humanGameObject.transform.position =
                    new Vector3(nextPos.x, 1, nextPos.y);
                humans[i] = human;
            }

            for (var i = 0; i < maxChairs; i++)
            {
                var chairGameObject = Instantiate(chairPrefab, transform);
                var chair = chairGameObject.GetComponent<Chair>();
                var (_, nextPos) = positions.Pop();
                chairGameObject.transform.position =
                    new Vector3(nextPos.x, 1, nextPos.y);
                chairs[i] = chair;
            }

            StartCoroutine(WaitBeforeCallingGoalNode());
        }

        private static IEnumerator WaitBeforeCallingGoalNode()
        {
            yield return new WaitForSeconds(2);
            Grid.Instance.ChooseNewGoalNode();
        }


        private static Stack<Tuple<Vector2Int, Vector2>>
            SelectRandomGridPositions(
                int numberOfCells)
        {
            var positions = new Stack<Tuple<Vector2Int, Vector2>>();
            var occupiedGrid = new bool[Grid.Instance.gridSizeX,
                Grid.Instance.gridSizeY];
            for (var i = 0; i < numberOfCells; i++)
            {
                var x = Random.Range(0, Grid.Instance.gridSizeX);
                var y = Random.Range(0, Grid.Instance.gridSizeY);
                var node = Grid.Instance.GetNodeFromGridPosition(x, y);
                if (!node.Clear || node.IsGoalNode || occupiedGrid[x, y])
                {
                    i--;
                    continue;
                }

                var position = node.WorldPoint;
                var gridPosition = new Vector2Int(x, y);
                var vector2 = new Vector2(position.x,
                    position.z);
                var tuple = new Tuple<Vector2Int, Vector2>(gridPosition,
                    vector2);
                positions.Push(tuple);
                occupiedGrid[x, y] = true;
            }

            return positions;
        }

        public void StopHumans()
        {
            foreach (var human in humans)
                human.StopMove();
        }

        public void ResetHumanPathing()
        {
            foreach (var human in humans)
                human.ResetPathing();
        }

        public static int GetMaxHumans()
        {
            return Instance.maxHumans;
        }

        public static int GetMaxChairs()
        {
            return Instance.maxChairs;
        }
    }
}