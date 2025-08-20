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
            Debug.LogError("[Spawner-승차] PositionUpdater_S를 찾을 수 없습니다. PositionUpdaterObject가 활성화되어 있는지 확인하세요!");
            return;
        }

        StartCoroutine(WaitUntilNextIntervalThenStart());
    }

    IEnumerator WaitUntilNextIntervalThenStart()
    {
        int secondsPastHour = DateTime.Now.Minute * 60 + DateTime.Now.Second;
        int secondsToWait = intervalSeconds - (secondsPastHour % intervalSeconds);

        Debug.Log($"[Spawner-승차] {DateTime.Now:HH:mm:ss} - {secondsToWait}초 후 첫 생성 시작");
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
                Debug.Log($"[Spawner-승차] 무정차 중이므로 스폰 생략 (무정차 횟수: {SimulationController.Instance.nonStopCount})");
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

            Debug.Log($"[Spawner-On] {DateTime.Now:HH:mm:ss} - 생성할 승차 인원 수: {spawnCount}");
        }
    }

    IEnumerator SpawnAvatarsFromPositionUpdater()
    {
        List<PersonData> peopleData = positionUpdater.GetCurrentPeopleData(collectionId);

        foreach (PersonData person in peopleData)
        {
            Vector3 newPos = person.movement_direction;

            // NavMesh 위 y값으로 보정
            if (NavMesh.SamplePosition(newPos, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                newPos = navHit.position;
            }
            else
            {
                Debug.LogWarning("NavMesh 위에 위치 없음, 스폰 생략");
                continue;
            }

            // 아바타가 없는 경우 → 생성
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

                // NavMeshAgent가 없다면 붙여주기
                NavMeshAgent agent = newAvatar.GetComponent<NavMeshAgent>();
                if (agent == null) agent = newAvatar.AddComponent<NavMeshAgent>();

                agent.speed = person.movement_speed;       // 데이터 기반 속도
                agent.acceleration = agent.speed * 2f;     // 부드럽게 달릴 수 있도록
                agent.angularSpeed = 120f;
                agent.SetDestination(newPos);

                avatarDict[person.peopleID] = new AvatarInfo
                {
                    avatar = newAvatar,
                };
            }
            else
            {
                // 이미 존재하는 아바타는 NavMeshAgent를 이용해 걷게 만들기
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
