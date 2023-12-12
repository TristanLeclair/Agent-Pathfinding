using System;
using _A3Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSCounter : MonoBehaviour
{
    public bool triggerQuitAfterSeconds;
    public float secondsToQuitAfter;
    public bool restart;
    public bool logToFile;
    public int startHumansToTest;
    public int startChairsToTest;
    public int maxHumansToTest;
    public int maxChairsToTest;
    public int maxIterations;
    public bool resetFspFileOnEnd;

    private int _iteration;
    private bool _reachedMaxHumans;
    private bool _reachedMaxChairs;

    private static SpawnerManager _spawnerManager;

    private void Start()
    {
        _spawnerManager = SpawnerManager.Instance;
        _iteration = 0;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!triggerQuitAfterSeconds) return;
        if (Time.realtimeSinceStartup > secondsToQuitAfter)
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnApplicationQuit()
    {
        if (!enabled) return;

        var elapsedTime = Time.realtimeSinceStartup;
        var fps = Time.frameCount / elapsedTime;
        Debug.Log($"Average FPS: {fps}");
        // append to csv file
        if (logToFile)
        {
            var path = Application.dataPath + "/fps.csv";
            System.IO.File.AppendAllText(path,
                $"{SpawnerManager.Instance.maxHumans}, {SpawnerManager.Instance.maxChairs}, {fps},\n");
        }
#if UNITY_EDITOR
        if (!restart)
        {
            QuitSimulation();
            return;
        }

        // Run multiple times per variation of humans / chairs
        IncrementIteration();
        _iteration = ReadIteration();
        Debug.Log("Ending iteration " + _iteration);
        if (_iteration >= maxIterations)
        {
            ResetIteration();
            switch (_reachedMaxHumans || GetChairs() > startChairsToTest)
            {
                case false when GetHumans() < maxHumansToTest:
                    DoubleHumans();
                    break;
                case false when GetHumans() >= maxHumansToTest:
                    _reachedMaxHumans = true;
                    ResetHumans();
                    DoubleChairs();
                    break;
                case true when GetChairs() < maxChairsToTest:
                    DoubleChairs();
                    break;
                case true when GetChairs() >= maxChairsToTest:
                    QuitSimulation();
                    return;
            }
        }


        // write max to file
        var maxPath = Application.dataPath + "/max.csv";
        System.IO.File.AppendAllText(maxPath,
            $"{GetHumans()}, {GetChairs()},\n");
        UnityEditor.EditorApplication.isPlaying = true;
#endif
    }

    private static void ResetIteration()
    {
        var iterationPath = Application.dataPath + "/iteration.txt";
        System.IO.File.WriteAllText(iterationPath, "0");
    }

    private static int ReadIteration()
    {
        var iterationPath = Application.dataPath + "/iteration.txt";
        if (!System.IO.File.Exists(iterationPath))
        {
            System.IO.File.WriteAllText(iterationPath, "0");
        }

        var iteration = int.Parse(System.IO.File.ReadAllText(iterationPath));
        return iteration;
    }

    private static void IncrementIteration()
    {
        var iterationPath = Application.dataPath + "/iteration.txt";
        if (!System.IO.File.Exists(iterationPath))
        {
            System.IO.File.WriteAllText(iterationPath, "0");
        }

        var iteration = int.Parse(System.IO.File.ReadAllText(iterationPath));
        System.IO.File.WriteAllText(iterationPath, $"{++iteration}");
    }

    private void QuitSimulation()
    {
        if (resetFspFileOnEnd)
            ResetFpsFile();
        ResetIteration();
        ResetMaxFile();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private static void ResetFpsFile()
    {
        var path = Application.dataPath + "/fps.csv";
        System.IO.File.WriteAllText(path, "MaxHumans, MaxChairs, FPS,\n");
    }

    private static void ResetMaxFile()
    {
        // clear max file
        var maxPath = Application.dataPath + "/max.csv";
        System.IO.File.WriteAllText(maxPath, "");
    }

    private void ResetHumans()
    {
        _spawnerManager.maxHumans = startHumansToTest;
    }

    private static int GetHumans() =>
        _spawnerManager.maxHumans;

    private static int GetChairs() =>
        _spawnerManager.maxChairs;

    private static void DoubleHumans()
    {
        _spawnerManager.maxHumans *= 2;
    }

    private static void DoubleChairs()
    {
        _spawnerManager.maxChairs *= 2;
    }
}