using UnityEngine;
using UnityEngine.UI;

public class PeopleCounterUI : MonoBehaviour
{
    public Text peopleText; 

    public void SetPeopleCount(int count)
    {
        peopleText.text = $"{count}";
    }
}
