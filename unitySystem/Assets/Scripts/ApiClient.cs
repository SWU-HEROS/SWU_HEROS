using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class ApiClient : MonoBehaviour
{
    private string apiUrl1 = "http://localhost:8000/first/first";
    private string apiUrl2 = "http://localhost:8000/second/second";
    private string apiUrl3 = "http://localhost:8000/third/third";

    public Button button1;
    public Button button2;
    public Button button3;

    void Start()
    {
        button1.onClick.AddListener(() => StartCoroutine(GetDataFromApi(apiUrl1, "Button 1")));
        button2.onClick.AddListener(() => StartCoroutine(GetDataFromApi(apiUrl2, "Button 2")));
        button3.onClick.AddListener(() => StartCoroutine(GetDataFromApi(apiUrl3, "Button 3")));
    }

    IEnumerator GetDataFromApi(string url, string buttonName)
    {
        // Time when the button was pressed
        DateTime buttonClickTime = DateTime.Now;
        double buttonClickTimestamp = (buttonClickTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        Debug.Log($"{buttonName} clicked at Unix timestamp: {buttonClickTimestamp:F7}");

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response from " + url + ": " + request.downloadHandler.text);

            // Time when the response was received
            DateTime responseTime = DateTime.Now;
            double responseTimestamp = (responseTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            Debug.Log($"{buttonName} response received at Unix timestamp: {responseTimestamp:F7}");

            // Calculate the elapsed time between button click time and response time
            double elapsedTime = responseTimestamp - buttonClickTimestamp;
            Debug.Log($"{buttonName} elapsed time: {elapsedTime:F7} seconds");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
