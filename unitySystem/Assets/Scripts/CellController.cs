using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CellController : MonoBehaviour
{
    public string cellID; // Automatically set as "c1", "c2", ..., "c200".
    private HashSet<GameObject> avatarsInCell = new HashSet<GameObject>(); // For storing avatars currently on the cell.

    private Renderer rend;      
    private Color baseColor;      

    public int AvatarCount => avatarsInCell.Count;

    public int GetPopulation()
    {
        return avatarsInCell.Count;
    }

    void Awake()
    {
      
        string objectName = gameObject.name;
        int index = 1;

        if (objectName == "¼¿")
        {
            index = 1;
        }
        else if (objectName.StartsWith("¼¿ ("))
        {
            string numberOnly = new string(objectName.Where(char.IsDigit).ToArray());
            if (int.TryParse(numberOnly, out int parsedIndex))
            {
                index = parsedIndex + 1; 
            }
        }

        cellID = "c" + index.ToString();


        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            baseColor = rend.material.color;
        }
    }

    void Update()
    {

        UpdateColorBasedOnPopulation();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))  
        {
            avatarsInCell.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove avatar from set when it leaves the cell
        if (other.CompareTag("Player"))
        {
            avatarsInCell.Remove(other.gameObject);
        }
    }

    void UpdateColorBasedOnPopulation()
    {
        if (rend == null) return;

        int count = avatarsInCell.Count;

        // Change cell color based on population count.
        if (count <= 3)
        {
            rend.material.color = baseColor; 
        }
        else if (count <= 5)
        {
            rend.material.color = Color.yellow; 
        }
        else if (count <= 7)
        {
            rend.material.color = new Color(1f, 0.5f, 0f);
        }
        else
        {
            rend.material.color = Color.red; 
        }
    }
}
