using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AvatarSpawner_D : MonoBehaviour
{
    public GameObject avatarPrefab;
    public Color avatarColor = Color.red;
    public string collectionId = "D";

    private const int intervalSeconds = 10;
    private const int simulationDurationSeconds = 2 * 3600;
    private bool isSpawning = false;

    private PositionUpdater_S positionUpdater;

    private class AvatarInfo
    {
        public GameObject avatar;
        public NavMeshAgent agent;
    }

    private Dictionary<string, AvatarInfo> avatarDict = new Dictionary<string, AvatarInfo>();

    void Start()
    {
        positionUpdater = FindObjectOfType<PositionUpdater_S>();

        if (positionUpdater == null)
        {
            Debug.LogError("[Spawner-Alighting] PositionUpdater_S was not found. Make sure that PositionUpdaterObject is active.");
            return;
        }

        StartCoroutine(WaitUntilNextIntervalThenStart());
    }

    IEnumerator WaitUntilNextIntervalThenStart()
    {
        int secondsPastHour = DateTime.Now.Minute * 60 + DateTime.Now.Second;
        int secondsToWait = intervalSeconds - (secondsPastHour % intervalSeconds);

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
        }
    }

    IEnumerator SpawnAvatarsFromPositionUpdater()
    {
        List<PersonData> peopleData = positionUpdater.GetCurrentPeopleData(collectionId);

        foreach (PersonData person in peopleData)
        {
            Vector3 targetPos = person.movement_direction;

            // Adjust the position to the NavMesh
            if (!NavMesh.SamplePosition(targetPos, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"No valid position found on the NavMesh. Skipping {person.peopleID}.");
                continue;
            }

            targetPos = navHit.position;

            if (!avatarDict.TryGetValue(person.peopleID, out AvatarInfo info))
            {
                // Initial spawn
                GameObject newAvatar = Instantiate(avatarPrefab, targetPos, Quaternion.Euler(0, -90, 0));
                newAvatar.name = person.peopleID;

                Renderer[] renderers = newAvatar.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    foreach (Material m in r.materials)
                    {
                        m.color = avatarColor;
                    }
                }

                NavMeshAgent agent = newAvatar.GetComponent<NavMeshAgent>();
                if (agent == null) agent = newAvatar.AddComponent<NavMeshAgent>();

                agent.Warp(targetPos);
                agent.speed = person.movement_speed;
                agent.SetDestination(targetPos);

                avatarDict[person.peopleID] = new AvatarInfo
                {
                    avatar = newAvatar,
                    agent = agent
                };
            }
            else
            {
                // Update the destination only
                if (info.agent == null)
                {
                    info.agent = info.avatar.GetComponent<NavMeshAgent>();
                    if (info.agent == null) info.agent = info.avatar.AddComponent<NavMeshAgent>();
                }

                info.agent.speed = person.movement_speed;
                info.agent.SetDestination(targetPos);
            }

            yield return null;
        }
    }
}
