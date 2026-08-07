using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using System;

public class PositionUpdater : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;

    // Define three collections
    private IMongoCollection<BsonDocument> collectionA;
    private IMongoCollection<BsonDocument> collectionB;


    public string mongoConnectionString = "mongodb://localhost:27017";
    public string dbName = "HEROS";
    public string collectionNameA = "people_data";
    public string collectionNameB = "test_collection";

    // Variables for storing predicted values
    private int predictedOff = 0;
    private int predictedOn = 0;
    private DateTime currentPredictionTime;

    private const int INTERVAL_MINUTES = 6;  // Train interval

    void Start()
    {
        client = new MongoClient(mongoConnectionString);
        database = client.GetDatabase(dbName);

        collectionA = database.GetCollection<BsonDocument>(collectionNameA);
        collectionB = database.GetCollection<BsonDocument>(collectionNameB);

        StartCoroutine(UpdatePositions());

        // Example: Set predicted values at startup
        // In the actual project, these values will be set by PredictionManager
        SetPredictedValues(100, 120);  // Remove if necessary
    }

    IEnumerator UpdatePositions()
    {
        while (true)
        {
            // Periodically retrieve new data from the database
            yield return new WaitForSeconds(10f);
        }
    }

    // Retrieve data from a specific collection
    public List<PersonData> GetCurrentPeopleData(string collectionId)
    {
        IMongoCollection<BsonDocument> targetCollection = null;

        switch (collectionId)
        {
            case "A": targetCollection = collectionA; break;
            case "B": targetCollection = collectionB; break;
            default:
                Debug.LogError($"Invalid collectionId: {collectionId}");
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

    // Manually set predicted values (can be called by PredictionManager, etc.)
    public void SetPredictedValues(int onCount, int offCount)
    {
        predictedOn = onCount;
        predictedOff = offCount;
        currentPredictionTime = DateTime.Now;
    }

    // Methods for external access (used by AvatarSpawnerOn/Off)
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

    // Test
    public List GetCurrentPeopleData()
    {
        // Dummy data list for testing
        return new List
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
    }

    // Method called externally by AvatarSpawnerOn/Off to retrieve current person data
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
