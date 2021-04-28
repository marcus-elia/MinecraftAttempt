using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject quad;
    public Texture tex;
    // Start is called before the first frame update
    void Start()
    {
        quad.GetComponent<Renderer>().material.mainTexture = tex;
        GameObject block = new GameObject();
        block.transform.position = Vector3.zero;
        block.AddComponent<Block>();
        block.GetComponent<Block>().CreateFaces(tex);
        block.GetComponent<Block>().SetTexture(tex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
