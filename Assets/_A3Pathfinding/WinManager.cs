using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _A3Pathfinding
{
    public class WinManager : MonoBehaviour
    {
        public static WinManager Instance => _instance
            ? _instance
            : _instance = FindObjectOfType<WinManager>();

        private static WinManager _instance;

        public static void Win()
        {
            GameManager.ResetGame();
        }
    }
}