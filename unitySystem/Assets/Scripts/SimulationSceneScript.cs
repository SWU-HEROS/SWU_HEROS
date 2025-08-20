using UnityEngine;
using UnityEngine.UI;

public class SimulationSceneScript : MonoBehaviour
{
    public Text dangerLevelText;

    void Start()
    {
        // Retrieve the DangerLevel value from PlayerPrefs (default is 0 = Caution).
        int dangerLevel = PlayerPrefs.GetInt("DangerLevel", 0);

        string dangerText = "";

        switch (dangerLevel)
        {
            case 0:
                dangerText = "Caution";
                break;
            case 1:
                dangerText = "Danger";
                break;
            case 2:
                dangerText = "Very Danger";
                break;
            default:
                dangerText = "Non";
                break;
        }

        // Display risk level on the text UI.
        dangerLevelText.text = dangerText;
    }
}
