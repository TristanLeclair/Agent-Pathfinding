using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static _A3Pathfinding.CollisionUtils;

namespace _A3Pathfinding
{
    public class Human : MonoBehaviour
    {
        // how often we recalculate path on sustained collisions
        private const float CollisionInterval = 0.5f;
        public float speed = 2f;
        public Vector3[] path;

        private int _pathIndexReached;

        private float _timer;

        private Vector2Int GridPosition =>
            Grid.Instance.GetNodeFromPositionOnGrid(transform.position)
                .GridPosition;

        // Update is called once per frame
        private void Update()
        {
            if (Grid.Instance.GoalNode == null ||
                (path != null && path.Length != 0)) return;

            SearchForPath();
        }

        private void OnCollisionEnter(Collision other)
        {
            HandleCollision(other);
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            _timer += Time.deltaTime;

            if (_timer < CollisionInterval) return;

            _timer = 0;
            HandleCollision(collisionInfo);
        }

        private void HandleCollision(Collision collisionInfo)
        {
            var otherLayer = collisionInfo.gameObject.layer;

            if (!IsObstacleLayer(otherLayer) &&
                !IsPlayerLayer(otherLayer))
                return;

            ResetPathing();
        }


        public void StopMove()
        {
            StopCoroutine(nameof(MoveAlongPath));
        }

        private void SearchForPath()
        {
            var pathNodes = AStarPathfinding.Instance.FindPath(
                Grid.Instance.GetNodeFromGridPosition(GridPosition),
                Grid.Instance.GoalNode);

            var enumerable = pathNodes.ToList();
            foreach (var node in enumerable)
                Grid.Instance.PathSet.Add(node);

            path = enumerable.Select(node => node.WorldPoint).ToArray();

            StopCoroutine(nameof(MoveAlongPath));
            _pathIndexReached = 0;
            StartCoroutine(nameof(MoveAlongPath));
        }

        private IEnumerator MoveAlongPath()
        {
            if (path == null || path.Length < 1) yield break;

            var currentPosition = path[0];
            while (true)
            {
                // reduce tolerance as we get closer to the goal
                // further away from the goal, the humans won't reach the middle of the node
                // 
                /*
                var len = path.Length / 10.0;
                var smoothing = 0.5 * len /
                                _pathIndexReached;
                                */

                double smoothing;
                if (path.Length > 1)
                {
                    var frac = (double)(path.Length - _pathIndexReached) /
                               path.Length;
                    smoothing = DoubleLerp(0.2, 0.9, frac);
                }
                else
                {
                    smoothing = 0.02;
                }

                if (VectorsEqualIgnoringY(transform.position, currentPosition,
                        smoothing))
                {
                    _pathIndexReached++;
                    // Debug.Log(_pathIndexReached);
                    if (_pathIndexReached >= path.Length) yield break;

                    currentPosition = path[_pathIndexReached];
                }

                var currentWorldPosition = transform.position;
                var movement = Vector3.MoveTowards(currentWorldPosition,
                    currentPosition, speed * Time.deltaTime);
                // We don't want to move in the y direction
                movement.Set(movement.x, currentWorldPosition.y, movement.z);
                currentWorldPosition = movement;
                transform.position = currentWorldPosition;
                yield return null;
            }
        }

        private static double DoubleLerp(double a, double b, double frac)
        {
            return a + (b - a) * Mathf.Clamp01((float)frac);
        }

        private static bool VectorsEqualIgnoringY(Vector3 v1, Vector3 v2,
            double tolerance = 0.5)
        {
            return Math.Abs(v1.x - v2.x) < tolerance &&
                   Math.Abs(v1.z - v2.z) < tolerance;
        }

        public void ResetPathing()
        {
            // Debug.Log("Resetting pathing");
            StopCoroutine(nameof(MoveAlongPath));
            path = null;
            _pathIndexReached = 0;
        }
    }
}