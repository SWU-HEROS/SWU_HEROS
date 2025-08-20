using UnityEngine;

public class DangerDetector : MonoBehaviour
{
    public static DangerDetector Instance;

    private void Awake()
    {
        Instance = this;
    }

    public float currentDensity = 0f;

    // Add a method to update the density value
    public void UpdateDensity(float newDensity)
    {
        currentDensity = newDensity;
    }

    public DangerLevel GetCurrentDangerLevel()
    {
        if (currentDensity <= 3f) return DangerLevel.Safe;
        else if (currentDensity <= 5f) return DangerLevel.Caution;
        else if (currentDensity <= 7f) return DangerLevel.Danger;
        else return DangerLevel.VeryDanger;
    }

    public bool IsDangerAtOrAboveLevel(DangerLevel level)
    {
        return GetCurrentDangerLevel() >= level;
    }

    public bool IsSafe()
    {
        return GetCurrentDangerLevel() == DangerLevel.Safe;
    }
}

