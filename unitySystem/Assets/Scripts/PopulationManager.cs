using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<CellController> cellControllers;

    public PeopleCounterUI peopleCounterUI;     
    public PeopleDensityUI peopleDensityUI;     

    void Start()
    {
        // Find all CellController components in the current scene and store them in a list.
        cellControllers = new List<CellController>(FindObjectsOfType<CellController>());
        StartCoroutine(LogPopulationEvery2Seconds());
    }

    IEnumerator LogPopulationEvery2Seconds()
    {
        while (true)
        {
            int totalPopulation = 0;    
            int activeCellCount = 0;    

            foreach (CellController cell in cellControllers)
            {
                int count = cell.GetPopulation();
                totalPopulation += count;

                if (count > 0)
                {
                    activeCellCount++; 
                }
            }

            Debug.Log("Total number of avatars: " + totalPopulation);

            if (peopleCounterUI != null)
            {
                peopleCounterUI.SetPeopleCount(totalPopulation);
            }

            if (peopleDensityUI != null && activeCellCount > 0)
            {
                float density = (float)totalPopulation / activeCellCount;
                peopleDensityUI.SetDensity(density);
            }

            yield return new WaitForSeconds(2f); 
        }
    }
}
