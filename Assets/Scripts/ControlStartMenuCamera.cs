using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just make the camera spin around and look at the scene

public class ControlStartMenuCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraTransform.Rotate(Vector3.up, rotationSpeed);
    }
}
