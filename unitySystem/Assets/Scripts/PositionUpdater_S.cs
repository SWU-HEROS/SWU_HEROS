using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using System;

public class PositionUpdater_S : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;

    //컬렉션 2개 정의
    private IMongoCollection<BsonDocument> collectionC;
    private IMongoCollection<BsonDocument> collectionD;
    

    public string mongoConnectionString = "mongodb://localhost:27017";
    public string dbName = "HEROS";
    public string collectionNameC = "test_collection_A";
    public string collectionNameD = "test_collection_B";

    // ✅ 예측값 저장용 변수
    private int predictedOff = 0;
    private int predictedOn = 0;
    private DateTime currentPredictionTime;

    private const int INTERVAL_MINUTES = 6;  // 배차 간격

    void Start()
    {
        client = new MongoClient(mongoConnectionString);
        database = client.GetDatabase(dbName);

        collectionC = database.GetCollection<BsonDocument>(collectionNameC);
        collectionD = database.GetCollection<BsonDocument>(collectionNameD);

        StartCoroutine(UpdatePositions());

        // 예시: 시작 시점에서 예측값 설정 (실제 프로젝트에서는 PredictionManager에서 설정하게 됨)
        SetPredictedValues(100, 120);  // 필요 시 제거
    }

    IEnumerator UpdatePositions()
    {
        while (true)
        {
            // DB에서 주기적으로 새 데이터를 가져오는 루틴
            yield return new WaitForSeconds(10f);
        }
    }

    // 특정 컬렉션에서 데이터 가져오기
    public List<PersonData> GetCurrentPeopleData(string collectionId)
    {
        IMongoCollection<BsonDocument> targetCollection = null;

        switch (collectionId)
        {
            case "C": targetCollection = collectionC; break;
            case "D": targetCollection = collectionD; break;
            default:
                Debug.LogError($"잘못된 collectionId: {collectionId}");
                return new List<PersonData>();
        }

        var docs = targetCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        var peopleList = new List<PersonData>();

        foreach (var doc in docs)
        {
            if (doc.Contains("cells"))
            {
                var cells = doc["cells"].AsBsonArray;
                foreach (var cell in cells)
                {
                    var people = cell["people"].AsBsonArray;
                    foreach (var personDoc in people)
                    {
                        peopleList.Add(BsonToPersonData(personDoc.AsBsonDocument));
                    }
                }
            }
        }
        return peopleList;
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

    // ✅ 예측값 수동 설정 (PredictionManager 등에서 호출 가능)
    public void SetPredictedValues(int onCount, int offCount)
    {
        predictedOn = onCount;
        predictedOff = offCount;
        currentPredictionTime = DateTime.Now;
    }

    // ✅ 외부 참조용 메서드 (AvatarSpawnerOn/Off에서 사용)
    public int GetRemainingBoardingCount()
    {
        return GetRemainingCount(predictedOn);
    }

    public int GetRemainingPassengerCount()
    {
        return GetRemainingCount(predictedOff);
    }

    private int GetRemainingCount(int prediction)
    {
        DateTime now = DateTime.Now;
        int minutesPast = (int)(now - currentPredictionTime).TotalMinutes;

        int totalBatches = 60 / INTERVAL_MINUTES;
        int passedBatches = minutesPast / INTERVAL_MINUTES;
        int remainingBatches = Mathf.Max(0, totalBatches - passedBatches);

        return remainingBatches > 0 ? prediction / totalBatches : 0;
    }


    //테스트
    /*public List<PersonData> GetCurrentPeopleData()
    {
        // 테스트용 더미 데이터 리스트
        return new List<PersonData>
        {
            new PersonData
            {
                peopleID = "Test001",
                movement_speed = 1.0f,
                movement_direction = new Vector3(-11.3f, 48.9f, 234.5f)
            },
            new PersonData
            {
                peopleID = "Test002",
                movement_speed = 1.5f,
                movement_direction = new Vector3(-11.28f, 44.07f, 258.71f)
            },
            new PersonData
            {
                peopleID = "Test003",
                movement_speed = 1.2f,
                movement_direction = new Vector3(-6f, 1.74f, 498f)
            },
            new PersonData
            {
                peopleID = "Test004",
                movement_speed = 1.0f,
                movement_direction = new Vector3(-1.8f, 66.9f, 86.7f)
            },
            new PersonData
            {
                peopleID = "Test005",
                movement_speed = 1.5f,
                movement_direction = new Vector3(172.5f, 66.9f, 102.3f)
            },
            new PersonData
            {
                peopleID = "Test006",
                movement_speed = 1.2f,
                movement_direction = new Vector3(-152.5f, 66.9f, 70.4f)
            }
        };
    }*/


    // ✅ AvatarSpawnerOn/Off 등 외부에서 호출해 현재 사람 정보 가져오는 메서드 
    // 컬렉션이 하나일 때만 필요함
    /*public List<PersonData> GetCurrentPeopleData()
    {
        var docs = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        var peopleList = new List<PersonData>();

        foreach (var doc in docs)
        {
            if (doc.Contains("cells"))
            {
                var cells = doc["cells"].AsBsonArray;
                foreach (var cell in cells)
                {
                    var people = cell["people"].AsBsonArray;
                    foreach (var personDoc in people)
                    {
                       
                        var person = BsonToPersonData(personDoc.AsBsonDocument);
                        peopleList.Add(person);

                        //Debug.Log($"[MongoDB] ID: {person.peopleID}, Speed: {person.movement_speed}, Dir: {person.movement_direction}");
                    }
                }
            }
        }

        return peopleList;
    }*/
}
