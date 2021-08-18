using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This makes the skybox and directional light blend/rotate at the same speed

public class MoveSun : MonoBehaviour
{
    private Material skyboxShader;
    public float skyboxSpeed;
    public float sunOrbitRadius = 100f;

    // Start is called before the first frame update
    void Start()
    {
        skyboxShader = RenderSettings.skybox;
        skyboxShader.SetFloat("_Blend", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        float theta = skyboxSpeed * Time.time;
        float z = sunOrbitRadius * Mathf.Sin(theta - 45f); // I'm not sure if offsetting the angle is good or not
        float y = sunOrbitRadius * Mathf.Cos(theta - 45f);
        transform.position = new Vector3(0, y, z);
        transform.LookAt(Vector3.zero);
        skyboxShader.SetFloat("_Blend", (-Mathf.Cos(theta - 45f) + 1) / 0.5f);
    }
}
