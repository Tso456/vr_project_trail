using UnityEngine;
using System.Collections.Generic;

public class POISwitcher : MonoBehaviour
{
    public List<Transform> POI; // List of Points of Interest (POIs) to switch between
    public Transform Player; // Reference to the camera's transform
    public float heightOffset = 0.5f; // Height offset to prevent falling into the ground

    public void switchPOI(int index)
    {
        if (index - 1 >= 0 && index - 1 < POI.Count)
        {
            // Move the camera to the selected POI
            Vector3 targetPosition = POI[index - 1].position;
            targetPosition.y += heightOffset;
            Player.position = targetPosition;
            Player.rotation = POI[index - 1].rotation;
            
        }   
        else
        {
            Debug.LogWarning("Invalid POI index: " + index);
        }
    }
}
