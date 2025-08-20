using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUIManager : MonoBehaviour
{
    public GameObject resultPanel;
    public Text resultText;

    private void Start()
    {
        resultPanel.SetActive(false); 
    }

    public void ShowResult(float elapsedSeconds, int nonStopCount, string reason)
    {
        string timeStr = $"{Mathf.FloorToInt(elapsedSeconds)} seconds";
        resultText.text = $"Simulation ended! ({reason})\n\n" +
                          $"Total elapsed time: {timeStr}\n" +
                          $"Number of non-stop occurrences: {nonStopCount}";

        resultPanel.SetActive(true);
    }


    public void OnCloseButtonClicked()
    {
        resultPanel.SetActive(false);
    }
}
