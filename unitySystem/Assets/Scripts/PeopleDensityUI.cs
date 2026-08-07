using UnityEngine;
using UnityEngine.UI;

public class PeopleDensityUI : MonoBehaviour
{
    public Text densityText;
    public Image densityImage;

    public void SetDensity(float density)
    {
        // Display density as a rounded integer value.
        densityText.text = $"{Mathf.RoundToInt(density)} people/m²";

        // Change the image color based on the density
        ChangeImageColor(density);

        // Pass density to DangerDetector.
        if (DangerDetector.Instance != null)
        {
            DangerDetector.Instance.UpdateDensity(density);
        }
    }

    // Change the image color
    void ChangeImageColor(float density)
    {
        if (density <= 3)
        {
            densityImage.color = Color.green;
        }

        else if (density <= 7)
        {
            densityImage.color = Color.yellow;
        }

        else
        {
            densityImage.color = Color.red;
        }
    }
}
