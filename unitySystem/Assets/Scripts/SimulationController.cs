using System.Collections;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public static SimulationController Instance;

    public ResultUIManager resultUIManager;
    public int nonStopCount { get; private set; } = 0;
    public bool isNonStopMode { get; private set; } = false;

    private float elapsedTime = 0f;
    private float simulationMaxSeconds = 5f;//2 * 3600f;
    private float nonStopInterval = 360f; 

    public DangerLevel selectedDangerLevel = DangerLevel.Danger;

    private void Awake()
    {
        Instance = this;

        int savedLevel = PlayerPrefs.GetInt("SelectedDangerLevel", (int)DangerLevel.Danger);
        selectedDangerLevel = (DangerLevel)savedLevel;

        Debug.Log($"SimulationController Awake: Loaded saved risk level: {selectedDangerLevel}");
    }

    private void Start()
    {
        StartCoroutine(SimulationLoop());
    }

    IEnumerator SimulationLoop()
    {
        float lastLogTime = 0f;
        float nonStopTimer = 0f;
        bool enteredNonStop = false; 

        while (true)
        {
            elapsedTime += Time.deltaTime;


            if (Time.time - lastLogTime >= 10f)
            {
                if (DangerDetector.Instance != null)
                {
                    Debug.Log($"[10-second interval log] Current density level: {DangerDetector.Instance.GetCurrentDangerLevel()}, Selected level: {selectedDangerLevel}");

                    lastLogTime = Time.time;
                }
            }

            if (DangerDetector.Instance != null && DangerDetector.Instance.IsDangerAtOrAboveLevel(selectedDangerLevel))
            {
                isNonStopMode = true;

                if (!enteredNonStop)
                {
                    nonStopCount++;
                    Debug.Log($"Entering non-stop! Cumulative count:: {nonStopCount}");
                    enteredNonStop = true;
                    nonStopTimer = 0f;
                }

                nonStopTimer += Time.deltaTime;

                if (nonStopTimer >= nonStopInterval)
                {
                    nonStopCount++;
                    Debug.Log($"Non-stop ongoing! Cumulative count: {nonStopCount}");
                    nonStopTimer = 0f;
                }
            }
            else
            {
                isNonStopMode = false;
                enteredNonStop = false;
                nonStopTimer = 0f;
            }

 
            if (isNonStopMode && DangerDetector.Instance != null && DangerDetector.Instance.IsSafe())
            {
                Debug.Log("Safe return, simulation ended.");
                EndSimulation("Safe return");
                yield break;
            }

            if (elapsedTime > simulationMaxSeconds)
            {
                Debug.Log("Simulation time ended");
                EndSimulation("time ended");
                yield break;
            }

            yield return null;
        }
    }

    void EndSimulation(string reason)
    {
        Debug.Log($"[Simulation End] Reason: {reason} / Time: {elapsedTime:F1} seconds / Non-stop occurrences: {nonStopCount} times");

        if (resultUIManager != null)
        {
            resultUIManager.ShowResult(elapsedTime, nonStopCount, reason);
        }
        else
        {
            Debug.LogWarning("Result UI Manager is not connected.");
        }
    }

}
