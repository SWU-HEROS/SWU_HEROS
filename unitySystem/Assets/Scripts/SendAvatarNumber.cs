/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendAvatarNumber : MonoBehaviour
{
    private string serverURL = "http://localhost/PBLUnityDB/personal.php";
    private Rigidbody rb;
    private float rotationSpeed = 5.0f; // Rotation speed
    private float fetchInterval = 3.0f; // Coordinate update interval

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
        // Remove labels such as "Coordinate X:" from the response
        // and extract only the numeric coordinate values
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

        // Move using the coordinate values
        Vector3 targetPosition = new Vector3(x, y, z);
        StartCoroutine(MoveToTargetPosition(targetPosition));
    }

    IEnumerator MoveToTargetPosition(Vector3 targetPosition)
    {
        float moveSpeed = 5.0f; // Movement speed
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Move smoothly toward the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Rotate toward the target direction
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
*/
