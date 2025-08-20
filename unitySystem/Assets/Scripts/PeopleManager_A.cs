using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine.AI;

public class PeopleManager_A : MonoBehaviour
{
    public GameObject avatarPrefab;
    public string collectionName = "people_data"; // 하차

    public string mongoConnectionString = "mongodb://localhost:27017";
    public string dbName = "HEROS";
    private IMongoCollection<BsonDocument> collection;

    private class AvatarInfo
    {
        public GameObject avatar;
        public NavMeshAgent agent;  // ✅ 추가
        public Vector3 targetPosition;
        public float moveDuration = 10f;
        public float elapsedTime = 0f;
        public Vector3 startPosition;
        public bool isMoving = false;
    }

    private Dictionary<string, AvatarInfo> peopleDict = new Dictionary<string, AvatarInfo>();

    void Start()
    {
        var client = new MongoClient(mongoConnectionString);
        var database = client.GetDatabase(dbName);
        collection = database.GetCollection<BsonDocument>(collectionName);

        LoadInitialPeople();
        StartCoroutine(UpdatePositions());
    }

    void LoadInitialPeople()
    {
        var latestDoc = collection.Find(Builders<BsonDocument>.Filter.Empty)
                                  .Sort(Builders<BsonDocument>.Sort.Descending("datetime"))
                                  .Limit(1)
                                  .FirstOrDefault();

        var peopleList = new List<PersonData>();

        if (latestDoc != null && latestDoc.Contains("cells"))
        {
            var cells = latestDoc["cells"].AsBsonArray;
            foreach (var cell in cells)
            {
                var people = cell["people"].AsBsonArray;
                foreach (var personDoc in people)
                {
                    peopleList.Add(BsonToPersonData(personDoc.AsBsonDocument));
                }
            }
        }

        InitializePeople(peopleList);
    }

    IEnumerator UpdatePositions()
    {
        while (true)
        {
            var latestDoc = collection.Find(Builders<BsonDocument>.Filter.Empty)
                                      .Sort(Builders<BsonDocument>.Sort.Descending("datetime"))
                                      .Limit(1)
                                      .FirstOrDefault();

            if (latestDoc != null && latestDoc.Contains("cells"))
            {
                var cells = latestDoc["cells"].AsBsonArray;

                foreach (var cell in cells)
                {
                    var people = cell["people"].AsBsonArray;
                    foreach (var personDoc in people)
                    {
                        string peopleID = personDoc["peopleID"].AsString;
                        var dir = personDoc["movement_direction"].AsBsonArray;
                        Vector3 newPos = new Vector3(
                            (float)dir[0].ToDouble(),
                            (float)dir[1].ToDouble(),
                            (float)dir[2].ToDouble()
                        );

                        UpdatePersonPosition(peopleID, newPos);
                    }
                }
            }

            yield return new WaitForSeconds(10f);
        }
    }

    PersonData BsonToPersonData(BsonDocument doc)
    {
        return new PersonData
        {
            peopleID = doc["peopleID"].AsString,
            movement_speed = (float)doc["movement_speed"].ToDouble(),
            movement_direction = new Vector3(
                (float)doc["movement_direction"][0].ToDouble(),
                (float)doc["movement_direction"][1].ToDouble(),
                (float)doc["movement_direction"][2].ToDouble()
            )
        };
    }

    public void InitializePeople(List<PersonData> people)
    {
        foreach (var person in people)
        {
            if (!peopleDict.ContainsKey(person.peopleID))
            {
                Vector3 pos = GetGroundPosition(person.movement_direction);

                GameObject avatar = Instantiate(avatarPrefab, pos, Quaternion.Euler(0, 180, 0));
                avatar.name = person.peopleID;

                NavMeshAgent agent = avatar.GetComponent<NavMeshAgent>();
                if (agent == null) agent = avatar.AddComponent<NavMeshAgent>();

                agent.Warp(pos);
                agent.speed = person.movement_speed;

                peopleDict.Add(person.peopleID, new AvatarInfo
                {
                    avatar = avatar
                });
            }
        }
    }

    Vector3 GetGroundPosition(Vector3 pos)
    {
        Vector3 rayOrigin = new Vector3(pos.x, 100f, pos.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 200f))
        {
            pos.y = hit.point.y;
        }
        else
        {
            pos.y = 0f;
        }
        return pos;
    }

    public void UpdatePersonPosition(string peopleID, Vector3 newPosition)
    {
        // 1️⃣ NavMesh 위 좌표 확인
        if (!NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"[PeopleManager_B] {peopleID} 위치가 NavMesh 위에 없음, 이동 생략");
            return;
        }

        newPosition = hit.position;

        if (!peopleDict.ContainsKey(peopleID))
        {
            // 2️⃣ 아바타 생성
            GameObject avatar = Instantiate(avatarPrefab, newPosition, Quaternion.Euler(0, 180, 0));
            avatar.name = peopleID;

            // 3️⃣ NavMeshAgent 확인 & Warp
            NavMeshAgent agent = avatar.GetComponent<NavMeshAgent>();
            if (agent == null) agent = avatar.AddComponent<NavMeshAgent>();

            agent.Warp(newPosition);   // 먼저 NavMesh 위로 이동
            agent.SetDestination(newPosition);

            peopleDict[peopleID] = new AvatarInfo { avatar = avatar, agent = agent };
            return;
        }

        // 이미 존재하는 아바타
        AvatarInfo info = peopleDict[peopleID];
        NavMeshAgent agentExisting = info.agent ?? info.avatar.AddComponent<NavMeshAgent>();

        agentExisting.Warp(agentExisting.transform.position); // 현재 위치를 NavMesh 위로
        agentExisting.SetDestination(newPosition);
    }

}
