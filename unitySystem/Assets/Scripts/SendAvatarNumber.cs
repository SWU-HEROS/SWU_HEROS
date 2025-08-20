/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendAvatarNumber : MonoBehaviour
{
    private string serverURL = "http://localhost/PBLUnityDB/personal.php";
    private Rigidbody rb;
    private float rotationSpeed = 5.0f; // 회전 속도
    private float fetchInterval = 3.0f; // 좌표 갱신 주기

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        int number = ExtractNValueFromObjectName();
        StartCoroutine(UpdateCoordinatesPeriodically(number));
    }

    int ExtractNValueFromObjectName()
    {
        string objectName = gameObject.name;
        int startIndex = objectName.IndexOf('(') + 1;
        int endIndex = objectName.IndexOf(')');
        string nString = objectName.Substring(startIndex, endIndex - startIndex);
        return int.Parse(nString);
    }

    IEnumerator UpdateCoordinatesPeriodically(int number)
    {
        while (true)
        {
            yield return new WaitForSeconds(fetchInterval);
            StartCoroutine(FetchCoordinateFromServer(number));
        }
    }

    IEnumerator FetchCoordinateFromServer(int number)
    {
        WWWForm form = new WWWForm();
        form.AddField("number", number.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string serverResponse = www.downloadHandler.text;
                Debug.Log("Server Response: " + serverResponse);
                ParseAndMoveAvatar(serverResponse);
            }
            else
            {
                Debug.LogError("Request failed. Error: " + www.error);
            }
        }
    }

    void ParseAndMoveAvatar(string serverResponse)
    {
        // 문자열에서 "Coordinate X: "과 같은 부분을 제거하고 숫자 값만 추출하여 좌표를 구함
        string[] coordinatesInfo = serverResponse.Split(new string[] { "<br>" }, System.StringSplitOptions.None);
        float x = 0, y = 0, z = 0;
        foreach (string info in coordinatesInfo)
        {
            if (info.StartsWith("Coordinate X:"))
            {
                string[] parts = info.Split(':');
                float.TryParse(parts[1], out x);
            }
            else if (info.StartsWith("Coordinate Y:"))
            {
                string[] parts = info.Split(':');
                float.TryParse(parts[1], out y);
            }
            else if (info.StartsWith("Coordinate Z:"))
            {
                string[] parts = info.Split(':');
                float.TryParse(parts[1], out z);
            }
        }

        // 좌표값을 이용하여 이동
        Vector3 targetPosition = new Vector3(x, y, z);
        StartCoroutine(MoveToTargetPosition(targetPosition));
    }

    IEnumerator MoveToTargetPosition(Vector3 targetPosition)
    {
        float moveSpeed = 5.0f; // 이동 속도
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 목표 위치까지 부드럽게 이동합니다.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 목표 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
*/