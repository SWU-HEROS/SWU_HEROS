using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCTV : MonoBehaviour
{
    public RawImage webcamDisplay;

    void Start()
    {
        // Get a list of all webcam devices.
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No webcam detected.");
            return;
        }

        // Create a WebCamTexture using the first webcam device.
        WebCamTexture webcamTexture = new WebCamTexture(devices[0].name);

        // Assign the webcam feed to the RawImage component.
        webcamDisplay.texture = webcamTexture;

        // Start the webcam.
        webcamTexture.Play();
    }
}
