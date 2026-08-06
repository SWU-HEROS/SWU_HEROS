using UnityEngine;
using UnityEngine.UI; 

public class PeopleDensityUI : MonoBehaviour
{
    public Text densityText; // "1 /��"
    public Image densityImage;

    public void SetDensity(float density)
    {
        // Display density as a rounded integer value.
        densityText.text = $"{Mathf.RoundToInt(density)} people/m²";

        //밀도에 따른 이미지 색상 변경
        ChangeImageColor(density);

        // Pass density to DangerDetector.
        if (DangerDetector.Instance != null)
        {
            DangerDetector.Instance.UpdateDensity(density);
        }
    }

    //이미지 변경 
    void ChangeImageColor(float density)
    {
        if (density <= 3)
        {
            densityImage.color = Color.green;
        }

        else if (density <=7) {

            densityImage.color = Color.yellow;
        }

        else
        {
            densityImage.color = Color.red;
        }
    }
  }


