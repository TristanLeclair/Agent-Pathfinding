using UnityEngine;

namespace _A3Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        private int _heapIndex;

        public bool IsGoalNode
        {
            get => _isGoalNode;
            set
            {
                _isGoalNode = value;
                SetHeight(_isGoalNode ? 1 : 0);
            }
        }

        public Node Parent;
        private bool _isGoalNode;

        public Node(Vector3 worldPoint, int x, int y, bool isGoalNode = false)
        {
            WorldPoint = worldPoint;
            X = x;
            Y = y;
            IsGoalNode = isGoalNode;
        }

        public bool Clear =>
            /*
            !Physics.CheckBox(WorldPoint,
                new Vector3(Grid.Instance.nodeRadius, 1,
                    Grid.Instance.nodeRadius));
                    */
            !Physics.CheckSphere(WorldPoint,
                Grid.Instance.nodeRadius);

        public bool ClearOfHumans() =>
            !Physics.CheckSphere(WorldPoint,
                Grid.Instance.nodeRadius, Grid.Instance.humanLayer);

        public Vector3 WorldPoint { get; private set; }
        public int X { get; }
        public int Y { get; }

        public int GCost { get; set; }
        public int HCost { get; set; }

        private void SetHeight(float height) => WorldPoint = new Vector3(
            WorldPoint.x,
            height,
            WorldPoint.z);

        public Vector2Int GridPosition => new(X, Y);

        private int FCost => GCost + HCost;

        /// <inheritdoc />
        public int CompareTo(Node other)
        {
            var compare = FCost.CompareTo(other.FCost);
            if (compare == 0) compare = HCost.CompareTo(other.HCost);

            return -compare;
        }

        /// <inheritdoc />
        public int HeapIndex { get; set; }

        public int GetDistanceToNode(Node otherNode)
        {
            var dstX = Mathf.Abs(X - otherNode.X);
            var dstY = Mathf.Abs(Y - otherNode.Y);
            if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}