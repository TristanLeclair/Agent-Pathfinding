using System;
using UnityEngine;
using static _A3Pathfinding.CollisionUtils;

namespace _A3Pathfinding
{
    public class Chair : MonoBehaviour
    {
        public float speed = 1f;
        public float maxRange = 100f;

        private int _maxColliders;

        private GameObject _human;
        private bool _humanFound;

        public Vector2Int GridPosition =>
            Grid.Instance.GetNodeFromPositionOnGrid(transform.position)
                .GridPosition;

        private void Start()
        {
            _maxColliders = SpawnerManager.Instance.maxChairs +
                           SpawnerManager.Instance.maxHumans + 1;
            InvokeRepeating(nameof(StartFindClosestHuman), 2f, 2f);
        }

        private void Update()
        {
            if (!_humanFound) return;
            // move towards human
            transform.position = Vector3.MoveTowards(transform.position,
                _human.transform.position,
                speed * Time.deltaTime);
        }

        private void StartFindClosestHuman()
        {
            FindClosestHuman();
        }

        private void FindClosestHuman(float searchRadius = 5f)
        {
            while (true)
            {
                var colliders = new Collider[_maxColliders];

                // Find colliders in range
                Physics.OverlapSphereNonAlloc(transform.position,
                    searchRadius, colliders);

                var closest = Mathf.Infinity;
                GameObject closestHuman = null;

                foreach (var colliderInRange in colliders)
                {
                    if (colliderInRange == null) continue;

                    var colliderLayer = colliderInRange.gameObject.layer;

                    if (!IsPlayerLayer(colliderLayer))
                        continue;

                    var distance = Vector3.Distance(transform.position,
                        colliderInRange.transform.position);

                    if (distance >= closest) continue;

                    closest = distance;
                    closestHuman = colliderInRange.gameObject;
                }

                if (closestHuman != null)
                {
                    _human = closestHuman;
                    _humanFound = true;
                    return;
                }

                searchRadius *= 2f;

                if (!(searchRadius > maxRange)) continue;

                // Debug.LogError("No human found at range " + maxRange);
                _humanFound = false;
                return;
            }
        }
    }
}