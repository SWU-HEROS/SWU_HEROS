using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PredictionManager : MonoBehaviour
{
    public static PredictionManager Instance;

    private int predictedOff = 0;
    private int predictedOn = 0;

    private DateTime currentPredictionTime;

    private const int INTERVAL_MINUTES = 6;  

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(UpdatePredictionLoop());
    }

    IEnumerator UpdatePredictionLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SendPredictionRequest("http://127.0.0.1:8000/get_off_predict", isOff: true));
            yield return StartCoroutine(SendPredictionRequest("http://127.0.0.1:8000/get_on_predict", isOff: false));

            currentPredictionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            yield return new WaitForSeconds(3600f); 
        }
    }

    IEnumerator SendPredictionRequest(string url, bool isOff)
    {
        DateTime now = DateTime.Now;

        PredictRequestData data = new PredictRequestData
        {
            ds = now.ToString("yyyy-MM-dd HH:mm:ss"),
            is_weekend = (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday) ? 1 : 0,
            is_halloween = (now.Month == 10 && now.Day == 31) ? 1 : 0
        };

        int dayOfWeek = ((int)now.DayOfWeek + 1);
        if (dayOfWeek == 1) dayOfWeek = 7;
        typeof(PredictRequestData).GetField($"dayofweek_{dayOfWeek}").SetValue(data, 1);
        typeof(PredictRequestData).GetField($"month_{now.Month}").SetValue(data, 1);

        int nextHour = now.Hour + 1;
        if (nextHour > 24) nextHour = 1; 
        typeof(PredictRequestData).GetField($"hour_{nextHour}").SetValue(data, 1);

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log($"[Prediction Response] {jsonResponse}");

            PredictResponseItem[] responses = JsonHelper.FromJson<PredictResponseItem>(jsonResponse);

            if (responses != null && responses.Length > 0)
            {
                int yhatValue = Mathf.RoundToInt(responses[0].yhat);

                if (isOff)
                {
                    predictedOff = yhatValue;
                    Debug.Log($"[Alighting Prediction] {yhatValue}people");
                }
                else
                {
                    predictedOn = yhatValue;
                    Debug.Log($"[Boarding Prediction] {yhatValue}people");
                }
            }
            else
            {
                Debug.LogWarning("Prediction result is empty.");
            }
        }
        else
        {
            Debug.LogError($"Prediction failed. ({(isOff ? "Alighting" : "Boarding")}): " + www.error);
        }
    }

    public int GetRemainingPassengerCount()  
    {
        return GetRemainingCount(predictedOff);
    }

    public int GetRemainingBoardingCount() 
    {
        return GetRemainingCount(predictedOn);
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

    [Serializable]
    public class PredictResponseItem
    {
        public string ds;
        public float yhat;
    }

    [Serializable]
    public class PredictResponseWrapper
    {
        public PredictResponseItem[] predictions;
    }

    // JsonHelper: Since JsonUtility cannot directly parse arrays, a wrapper class is used.
    public static class JsonHelper
    {
        // Change the wrapper class to be generic.
        [Serializable]
        private class Wrapper<T>
        {
            public T[] predictions;
        }

        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"predictions\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.predictions;
        }
    }

}