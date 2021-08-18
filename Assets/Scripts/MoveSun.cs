using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This makes the skybox and directional light blend/rotate at the same speed

public class MoveSun : MonoBehaviour
{
    private Material skyboxShader;
    public float sunOrbitRadius = 100f;
    public static float dayNightLength = 64f;

    private float theta_ = 0;

    private static float timeOffset = 0;

    // Audio depends on day night cycle
    public GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        skyboxShader = RenderSettings.skybox;
        skyboxShader.SetFloat("_Blend", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        float newTheta = Mathf.Deg2Rad * Inventory.Mod((int)((Time.time - timeOffset) * 360f / dayNightLength), 360);

        // Check if it is morning
        if(theta_ < 5*Mathf.PI/4 && newTheta >= 5*Mathf.PI/4)
        {
            audioManager.GetComponent<AudioManager>().PlayMajor();
        }
        // Check if it is evening
        else if (theta_ < Mathf.PI/4 && newTheta >= Mathf.PI/4)
        {
            audioManager.GetComponent<AudioManager>().PlayMinor();
        }

        // Move the skybox and sun
        theta_ = newTheta;
        float z = sunOrbitRadius * Mathf.Sin(theta_); // I'm not sure if offsetting the angle is good or not
        float y = sunOrbitRadius * Mathf.Cos(theta_);
        transform.position = new Vector3(0, y, z);
        transform.LookAt(Vector3.zero);
        skyboxShader.SetFloat("_Blend", (-Mathf.Cos(theta_) + 1) / 0.5f);
    }

    public static void UpdateTimeOffset()
    {
        timeOffset = Time.time;
    }
}
