using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AvatarSpawner_C : MonoBehaviour
{
    public GameObject avatarPrefab;
    public Color avatarColor = Color.blue;
    public float moveDuration = 10f;
    public string collectionId = "C";

    private const int intervalSeconds = 10;
    private const int simulationDurationSeconds = 2 * 3600;
    private bool isSpawning = false;

    private PositionUpdater_S positionUpdater;

    private class AvatarInfo
    {
        public GameObject avatar;
    }

    private Dictionary<string, AvatarInfo> avatarDict = new Dictionary<string, AvatarInfo>();

    void Start()
    {
        positionUpdater = FindObjectOfType<PositionUpdater_S>();

        if (positionUpdater == null)
        {
            Debug.LogError("[Spawner-Boarding] PositionUpdater_S was not found. Make sure that PositionUpdaterObject is active.");
            return;
        }

        StartCoroutine(WaitUntilNextIntervalThenStart());
    }

    IEnumerator WaitUntilNextIntervalThenStart()
    {
        int secondsPastHour = DateTime.Now.Minute * 60 + DateTime.Now.Second;
        int secondsToWait = intervalSeconds - (secondsPastHour % intervalSeconds);

        Debug.Log($"[Spawner-Boarding] {DateTime.Now:HH:mm:ss} - Initial spawn will begin in {secondsToWait} seconds");
        yield return new WaitForSeconds(secondsToWait);

        StartCoroutine(SpawnPassengersLoop());
    }

    IEnumerator SpawnPassengersLoop()
    {
        int totalRounds = simulationDurationSeconds / intervalSeconds;

        for (int i = 0; i < totalRounds; i++)
        {
            if (SimulationController.Instance != null && SimulationController.Instance.isNonStopMode)
            {
                Debug.Log($"[Spawner-Boarding] Spawn skipped due to non-stop mode (Non-stop count: {SimulationController.Instance.nonStopCount})");
                yield return new WaitForSeconds(intervalSeconds);
                continue;
            }

            if (isSpawning)
            {
                yield return new WaitForSeconds(intervalSeconds);
                continue;
            }

            isSpawning = true;
            yield return StartCoroutine(SpawnAvatarsFromPositionUpdater());
            isSpawning = false;

            yield return new WaitForSeconds(intervalSeconds);

            int totalSpawnCount = PredictionManager.Instance.GetRemainingBoardingCount();
            int spawnCount = totalSpawnCount / 2;

            Debug.Log($"[Spawner-On] {DateTime.Now:HH:mm:ss} - Number of boarding passengers to spawn: {spawnCount}");
        }
    }

    IEnumerator SpawnAvatarsFromPositionUpdater()
    {
        List<PersonData> peopleData = positionUpdater.GetCurrentPeopleData(collectionId);

        foreach (PersonData person in peopleData)
        {
            Vector3 newPos = person.movement_direction;

            // Adjust the y-coordinate to match the NavMesh
            if (NavMesh.SamplePosition(newPos, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                newPos = navHit.position;
            }
            else
            {
                Debug.LogWarning("No valid position found on the NavMesh. Skipping spawn.");
                continue;
            }

            // Create an avatar if it does not exist
            if (!avatarDict.ContainsKey(person.peopleID))
            {
                GameObject newAvatar = Instantiate(avatarPrefab, newPos, Quaternion.Euler(0, -90, 0));
                newAvatar.name = person.peopleID;

                Renderer[] renderers = newAvatar.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        mat.color = avatarColor;
                    }
                }

                // Add a NavMeshAgent if one does not exist
                NavMeshAgent agent = newAvatar.GetComponent<NavMeshAgent>();
                if (agent == null) agent = newAvatar.AddComponent<NavMeshAgent>();

                agent.speed = person.movement_speed;       // Set speed based on the data
                agent.acceleration = agent.speed * 2f;     // Enable smooth acceleration
                agent.angularSpeed = 120f;
                agent.SetDestination(newPos);

                avatarDict[person.peopleID] = new AvatarInfo
                {
                    avatar = newAvatar,
                };
            }
            else
            {
                // Move the existing avatar using the NavMeshAgent
                AvatarInfo info = avatarDict[person.peopleID];
                NavMeshAgent agent = info.avatar.GetComponent<NavMeshAgent>();

                if (agent == null)
                    agent = info.avatar.AddComponent<NavMeshAgent>();

                agent.speed = person.movement_speed;
                agent.SetDestination(newPos);
            }

            yield return null;
        }
    }
}
