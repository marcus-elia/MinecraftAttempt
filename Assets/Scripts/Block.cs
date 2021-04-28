using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private GameObject topFace;
    private GameObject bottomFace;
    private GameObject northFace;
    private GameObject eastFace;
    private GameObject southFace;
    private GameObject westFace;

    public static int blockSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateFaces()
    {
        topFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        topFace.transform.position = transform.position + blockSize / 2f * Vector3.up;
        topFace.transform.Rotate(Vector3.right, 90f);

        bottomFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bottomFace.transform.position = transform.position + blockSize / 2f * Vector3.down;
        bottomFace.transform.Rotate(Vector3.right, -90f);

        northFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        northFace.transform.position = transform.position + blockSize / 2f * Vector3.forward;
        northFace.transform.Rotate(Vector3.up, 180f);

        southFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        southFace.transform.position = transform.position + blockSize / 2f * Vector3.back;
        // Don't need to rotate this one

        eastFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        eastFace.transform.position = transform.position + blockSize / 2f * Vector3.right;
        eastFace.transform.Rotate(Vector3.up, -90f);

        westFace = GameObject.CreatePrimitive(PrimitiveType.Quad);
        westFace.transform.position = transform.position + blockSize / 2f * Vector3.left;
        westFace.transform.Rotate(Vector3.up, 90f);
    }

    public void SetTexture(Texture tex)
    {
        topFace.GetComponent<Renderer>().material.mainTexture = tex;
        bottomFace.GetComponent<Renderer>().material.mainTexture = tex;
        northFace.GetComponent<Renderer>().material.mainTexture = tex;
        southFace.GetComponent<Renderer>().material.mainTexture = tex;
        eastFace.GetComponent<Renderer>().material.mainTexture = tex;
        westFace.GetComponent<Renderer>().material.mainTexture = tex;
    }
}
