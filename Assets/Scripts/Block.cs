using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Quads
    private GameObject topFace;
    private GameObject bottomFace;
    private GameObject northFace;
    private GameObject eastFace;
    private GameObject southFace;
    private GameObject westFace;
    
    // 6 neighboring  blocks
    private GameObject topNeighbor;
    private GameObject bottomNeighbor;
    private GameObject northNeighbor;
    private GameObject southNeighbor;
    private GameObject eastNeighbor;
    private GameObject westNeighbor;

    // Info for the chunk
    private int chunkID;
    private Point3D indexInChunk;

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
        topFace.layer = 6;

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

    public void SetChunkID(int inputChunkID)
    {
        chunkID = inputChunkID;
    }

    public void SetIndexInChunk(int x, int y, int z)
    {
        indexInChunk = new Point3D(x, y, z);
    }

    public void EnableRendering()
    {
        if(topNeighbor == null)
        {
            topFace.SetActive(true);
        }
        if (bottomNeighbor == null)
        {
            bottomFace.SetActive(true);
        }
        if (northNeighbor == null)
        {
            northFace.SetActive(true);
        }
        if (southNeighbor == null)
        {
            southFace.SetActive(true);
        }
        if (eastNeighbor == null)
        {
            eastFace.SetActive(true);
        }
        if (westNeighbor == null)
        {
            westFace.SetActive(true);
        }
    }
    public void DisableRendering()
    {
        topFace.SetActive(false);
        bottomFace.SetActive(false);
        northFace.SetActive(false);
        southFace.SetActive(false);
        eastFace.SetActive(false);
        westFace.SetActive(false);
    }

    public void SetTopNeighbor(GameObject neighbor)
    {
        topNeighbor = neighbor;
        if(topNeighbor)
        {
            topFace.SetActive(false);
        }
        else
        {
            topFace.SetActive(true);
        }
    }
    public void SetBottomNeighbor(GameObject neighbor)
    {
        bottomNeighbor = neighbor;
        if (bottomNeighbor)
        {
            bottomFace.SetActive(false);
        }
        else
        {
            bottomFace.SetActive(true);
        }
    }
    public void SetNorthNeighbor(GameObject neighbor)
    {
        northNeighbor = neighbor;
        if (northNeighbor)
        {
            northFace.SetActive(false);
        }
        else
        {
            northFace.SetActive(true);
        }
    }
    public void SetSouthNeighbor(GameObject neighbor)
    {
        southNeighbor = neighbor;
        if (southNeighbor)
        {
            southFace.SetActive(false);
        }
        else
        {
            southFace.SetActive(true);
        }
    }
    public void SetEastNeighbor(GameObject neighbor)
    {
        eastNeighbor = neighbor;
        if (eastNeighbor)
        {
            eastFace.SetActive(false);
        }
        else
        {
            eastFace.SetActive(true);
        }
    }
    public void SetWestNeighbor(GameObject neighbor)
    {
        westNeighbor = neighbor;
        if (westNeighbor)
        {
            westFace.SetActive(false);
        }
        else
        {
            westFace.SetActive(true);
        }
    }

    // Count how many faces are exposed
    public int GetNumExposedFaces()
    {
        int count = 0;
        if(topNeighbor == null)
        {
            count += 1;
        }
        if (bottomNeighbor == null)
        {
            count += 1;
        }
        if (northNeighbor == null)
        {
            count += 1;
        }
        if (southNeighbor == null)
        {
            count += 1;
        }
        if (eastNeighbor == null)
        {
            count += 1;
        }
        if (westNeighbor == null)
        {
            count += 1;
        }
        return count;
    }

    public int GetChunkID()
    {
        return chunkID;
    }
    public Point3D GetIndexInChunk()
    {
        return indexInChunk;
    }

    public void RemoveSelf()
    {
        Debug.Log("broke block located at ");
        Debug.Log(transform.position);
        if(topNeighbor)
        {
            topNeighbor.GetComponent<Block>().SetBottomNeighbor(null);
        }
        if (bottomNeighbor)
        {
            bottomNeighbor.GetComponent<Block>().SetTopNeighbor(null);
        }
        if (topNeighbor)
        {
            topNeighbor.GetComponent<Block>().SetBottomNeighbor(null);
        }
        if (northNeighbor)
        {
            northNeighbor.GetComponent<Block>().SetSouthNeighbor(null);
        }
        if (southNeighbor)
        {
            southNeighbor.GetComponent<Block>().SetNorthNeighbor(null);
        }
        if (eastNeighbor)
        {
            eastNeighbor.GetComponent<Block>().SetWestNeighbor(null);
        }
        if (westNeighbor)
        {
            westNeighbor.GetComponent<Block>().SetEastNeighbor(null);
        }
        this.DisableRendering();
        Destroy(topFace);
        Destroy(bottomFace);
        Destroy(northFace);
        Destroy(southFace);
        Destroy(eastFace);
        Destroy(westFace);
    }
}
