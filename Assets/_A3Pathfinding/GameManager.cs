using UnityEngine;
using UnityEngine.SceneManagement;

namespace _A3Pathfinding
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance
            ? _instance
            : _instance = FindObjectOfType<GameManager>();

        private static GameManager _instance;

        public static void ResetGame()
        {
            SpawnerManager.Instance.StopHumans();

            var wait = new WaitForSecondsRealtime(0.5f);
            
            Grid.Instance.ChooseNewGoalNode();

            SpawnerManager.Instance.ResetHumanPathing();
        }
    }
}