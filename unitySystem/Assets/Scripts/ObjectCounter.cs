using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCounter : MonoBehaviour
{
    public Plane countingPlane; 

    public int GetObjectsOnPlaneCount()
    {
     
        GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag("Player");


        int objectsOnPlane = 0;

        foreach (GameObject obj in objectsInScene)
        {
            if (IsObjectOnPlane(obj.transform.position))
            {
                objectsOnPlane++;
            }
        }

        return objectsOnPlane;
    }

    private bool IsObjectOnPlane(Vector3 position)
    {
        return countingPlane.GetDistanceToPoint(position) == 0f;
    }
}


