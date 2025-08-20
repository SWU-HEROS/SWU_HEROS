using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneScript : MonoBehaviour
{
    public Toggle cautionToggle;    
    public Toggle dangerToggle;     
    public Toggle extremeToggle;    

    public void StartSimulation()
    {
        int dangerLevel = 0; // Default value: 'Caution'

        if (cautionToggle.isOn)
        {
            dangerLevel = 0;
        }
        else if (dangerToggle.isOn)
        {
            dangerLevel = 1;
        }
        else if (extremeToggle.isOn)
        {
            dangerLevel = 2;
        }

        // Save dangerLevel in PlayerPrefs.
        PlayerPrefs.SetInt("DangerLevel", dangerLevel);

        // Load scene "SimulationScene".
        SceneManager.LoadScene("SimulationScene");
    }
}
