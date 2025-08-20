using UnityEngine;
using UnityEngine.UI; 

public class PeopleDensityUI : MonoBehaviour
{
    public Text densityText; // "1 /��"

    public void SetDensity(float density)
    {
        // Display density as a rounded integer value.
        densityText.text = $"{Mathf.RoundToInt(density)} people/m²";

        // Pass density to DangerDetector.
        if (DangerDetector.Instance != null)
        {
            DangerDetector.Instance.UpdateDensity(density);
        }
    }
}
