using UnityEngine;

public enum DangerLevel
{
    Safe = 0,
    Caution = 1,
    Danger = 2,
    VeryDanger = 3
}


public class DangerLevelSelector : MonoBehaviour
{
    public void SelectCaution() => SaveDangerLevel(1);
    public void SelectDanger() => SaveDangerLevel(2);
    public void SelectVeryDanger() => SaveDangerLevel(3);

    private void SaveDangerLevel(int level)
    {
        PlayerPrefs.SetInt("SelectedDangerLevel", level);
        PlayerPrefs.Save(); 
        Debug.Log($"Risk level saved.: {(DangerLevel)level}");
    }
}
