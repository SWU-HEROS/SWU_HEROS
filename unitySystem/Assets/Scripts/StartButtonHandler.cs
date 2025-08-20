using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButtonHandler : MonoBehaviour
{
    public ToggleGroup dangerToggleGroup;

    public void OnStartButtonClicked()
    {
        Toggle selectedToggle = null;

        foreach (var toggle in dangerToggleGroup.ActiveToggles())
        {
            selectedToggle = toggle;
            break;
        }

        if (selectedToggle == null)
        {
            Debug.LogWarning("Risk level not selected!");
            return;
        }

        DangerLevel selectedLevel = DangerLevel.Danger;

        switch (selectedToggle.name)
        {
            case "ToggleCaution":
                selectedLevel = DangerLevel.Caution;
                break;
            case "ToggleDanger":
                selectedLevel = DangerLevel.Danger;
                break;
            case "ToggleVeryDanger":
                selectedLevel = DangerLevel.VeryDanger;
                break;
        }

        PlayerPrefs.SetInt("SelectedDangerLevel", (int)selectedLevel);
        PlayerPrefs.Save();

        Debug.Log($"Selected risk level saved: {selectedLevel}");

        SceneManager.LoadScene("SimulationScene"); 
    }
}
