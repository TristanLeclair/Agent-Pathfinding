using UnityEngine;

namespace _A3Pathfinding
{
    public abstract class CollisionUtils
    {
        public static readonly int ObstacleLayer =
            LayerMask.NameToLayer("Obstacle");

        public static readonly int PlayerLayer =
            LayerMask.NameToLayer("Player");

        public static bool IsObstacleLayer(int gameObjectLayer)
        {
            return IsLayer(gameObjectLayer, ObstacleLayer);
        }

        public static bool IsPlayerLayer(int gameObjectLayer)
        {
            return IsLayer(gameObjectLayer, PlayerLayer);
        }

        private static bool IsLayer(int gameObjectLayer, int layer)
        {
            return gameObjectLayer == layer;
        }
    }
}