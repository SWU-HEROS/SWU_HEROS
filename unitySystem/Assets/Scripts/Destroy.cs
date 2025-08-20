using UnityEngine;

public class Destroy : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")      
        {        
            Debug.Log("Recognized");
            Destroy(other.gameObject);      
        }  
        else
        {
            Debug.Log("Not recognized");
        } 
    }
}
