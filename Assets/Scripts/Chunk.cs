using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The transform.position of a Chunk is at the SW corner of 
// the Chunk, with a y value of 0. The bottom of the lowest
// layer of blocks is at y = 0. So the center of the lowest
// layer of blocks is at y = 0.5.
// The blocks array stores worldHeight 2d-arrays. So in
// blocks[0], each block is at y = 0.5. At blocks[i], each
// block is at y = i + 0.5;
// So when you are walking on ground level, your feet are on
// y = groundLevel + 1.
// Within each layer of blocks, layer[0][0] is the southwest
// corner, and layer[blocksPerSide-1][blocksPerSide-1] is the
// northeast corner.
// So the indexing of blocks is [y, x, z].
public class Chunk : MonoBehaviour
{
    // Basic chunk properties
    private int chunkID;
    private Point2D chunkCoords; // corresponds to south west corner
    public static int blocksPerSide = 16;
    public static int worldHeight = 10;
    public static int groundLevel = 1;
    private int perlinValue;

    // Neighboring chunks
    private Chunk northNeighbor;
    private Chunk eastNeighbor;
    private Chunk southNeighbor;
    private Chunk westNeighbor;

    // For now, every block gets this
    private Texture tex;

    private GameObject[,,] blocks = new GameObject[worldHeight, blocksPerSide, blocksPerSide];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableChunk()
    {
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < blocksPerSide; x++)
            {
                for(int z = 0; z < blocksPerSide; z++)
                {
                    if(blocks[y, x, z] != null)
                    {
                        blocks[y, x, z].GetComponent<Block>().EnableRendering();
                    }
                }
            }
        }
    }
    public void DisableChunk()
    {
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < blocksPerSide; x++)
            {
                for (int z = 0; z < blocksPerSide; z++)
                {
                    if (blocks[y, x, z] != null)
                    {
                        blocks[y, x, z].GetComponent<Block>().DisableRendering();
                    }
                }
            }
        }
    }

    // Setters and Initializers
    public void SetChunkID(int inputID)
    {
        chunkID = inputID;
        chunkCoords = ChunkManager.chunkIDtoPoint2D(chunkID);
        CalculatePosition();
    }
    private void CalculatePosition()
    {
        transform.position = new Vector3(chunkCoords.x * blocksPerSide, 0, chunkCoords.z * blocksPerSide);
    }
    public void SetTexture(Texture input)
    {
        tex = input;
    }

    public void InitializeBlocks()
    {
        for(int y = 0; y <= groundLevel; y++)
        {
            for(int x = 0; x < blocksPerSide; x++)
            {
                for(int z = 0; z < blocksPerSide; z++)
                {
                    GameObject block = new GameObject();
                    block.AddComponent<Block>();
                    block.transform.SetParent(transform);
                    block.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    block.GetComponent<Block>().CreateFaces();
                    block.GetComponent<Block>().SetTexture(tex);
                    blocks[y, x, z] = block;
                }
            }
        }
        SetVerticalBlockNeighbors();
        SetInteriorBlockNeighbors();
    }

    // The top and bottom neighbors of the blocks are only within this chunk
    private void SetVerticalBlockNeighbors()
    {
        for(int y = 0; y < worldHeight - 1; y++)
        {
            for(int x = 0; x < blocksPerSide; x++)
            {
                for(int z = 0; z < blocksPerSide; z++)
                {
                    // Set the top neighbor
                    if (blocks[y, x, z] != null)
                    {
                        blocks[y, x, z].GetComponent<Block>().SetTopNeighbor(blocks[y + 1, x, z]);
                    }
                    // Set the bottom neighbor of the one above this
                    if (blocks[y + 1, x, z] != null)
                    {
                        blocks[y + 1, x, z].GetComponent<Block>().SetBottomNeighbor(blocks[y, x, z]);
                    }
                }
            }
        }
    }
    // The interior block neighbors also are only within this chunk
    private void SetInteriorBlockNeighbors()
    {
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < blocksPerSide - 1; x++)
            {
                for(int z = 0; z < blocksPerSide - 1; z++)
                {
                    // Set the north neighbor
                    if (blocks[y, x, z] != null)
                    {
                        blocks[y, x, z].GetComponent<Block>().SetNorthNeighbor(blocks[y, x, z + 1]);
                    }
                    // Set the south neighbor of the one north of this
                    if (blocks[y, x, z + 1] != null)
                    {
                        blocks[y, x, z + 1].GetComponent<Block>().SetSouthNeighbor(blocks[y, x, z]);
                    }
                    // Set the east neighbor
                    if (blocks[y, x, z] != null)
                    {
                        blocks[y, x, z].GetComponent<Block>().SetEastNeighbor(blocks[y, x + 1, z]);
                    }
                    // Set the west neighbor of the one east of this
                    if (blocks[y, x + 1, z] != null)
                    {
                        blocks[y, x + 1, z].GetComponent<Block>().SetWestNeighbor(blocks[y, x, z]);
                    }
                }
            }
        }
        // Set the neighbors of the north and east edges
        for(int y = 0; y < worldHeight; y++)
        {
            // East edge
            for(int z = 0; z < blocksPerSide - 1; z++)
            {
                if(blocks[y, blocksPerSide - 1, z] != null)
                {
                    blocks[y, blocksPerSide - 1, z].GetComponent<Block>().SetNorthNeighbor(blocks[y, blocksPerSide - 1, z + 1]);
                }
                if (blocks[y, blocksPerSide - 1, z + 1] != null)
                {
                    blocks[y, blocksPerSide - 1, z + 1].GetComponent<Block>().SetSouthNeighbor(blocks[y, blocksPerSide - 1, z]);
                }
            }
            // North edge
            for(int x = 0; x < blocksPerSide - 1; x++)
            {
                if(blocks[y, x, blocksPerSide - 1] != null)
                {
                    blocks[y, x, blocksPerSide - 1].GetComponent<Block>().SetEastNeighbor(blocks[y, x + 1, blocksPerSide - 1]);
                }
                if (blocks[y, x + 1, blocksPerSide - 1] != null)
                {
                    blocks[y, x + 1, blocksPerSide - 1].GetComponent<Block>().SetWestNeighbor(blocks[y, x, blocksPerSide - 1]);
                }
            }
        }
    }

    public void SetNorthNeighbor(Chunk neighbor)
    {
        northNeighbor = neighbor;

        // Give the blocks their neighbors
        int z = blocksPerSide - 1;
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < blocksPerSide; x++)
            {
                if(blocks[y, x, z] != null)
                {
                    GameObject neighborBlock = northNeighbor.GetComponent<Chunk>().GetBlocks()[y, x, 0];
                    blocks[y, x, z].GetComponent<Block>().SetNorthNeighbor(neighborBlock);
                }
            }
        }
    }
    public void SetSouthNeighbor(Chunk neighbor)
    {
        southNeighbor = neighbor;

        // Give the blocks their neighbors
        int z = 0;
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < blocksPerSide; x++)
            {
                if (blocks[y, x, z] != null)
                {
                    GameObject neighborBlock = southNeighbor.GetComponent<Chunk>().GetBlocks()[y, x, blocksPerSide - 1];
                    blocks[y, x, z].GetComponent<Block>().SetSouthNeighbor(neighborBlock);
                }
            }
        }
    }
    public void SetEastNeighbor(Chunk neighbor)
    {
        eastNeighbor = neighbor;

        // Give the blocks their neighbors
        int x = blocksPerSide - 1;
        for (int y = 0; y < worldHeight; y++)
        {
            for (int z = 0; z < blocksPerSide; z++)
            {
                if (blocks[y, x, z] != null)
                {
                    GameObject neighborBlock = eastNeighbor.GetComponent<Chunk>().GetBlocks()[y, 0, z];
                    blocks[y, x, z].GetComponent<Block>().SetEastNeighbor(neighborBlock);
                }
            }
        }
    }
    public void SetWestNeighbor(Chunk neighbor)
    {
        westNeighbor = neighbor;

        // Give the blocks their neighbors
        int x = 0;
        for (int y = 0; y < worldHeight; y++)
        {
            for (int z = 0; z < blocksPerSide; z++)
            {
                if (blocks[y, x, z] != null)
                {
                    GameObject neighborBlock = westNeighbor.GetComponent<Chunk>().GetBlocks()[y, blocksPerSide - 1, z];
                    blocks[y, x, z].GetComponent<Block>().SetWestNeighbor(neighborBlock);
                }
            }
        }
    }


    // Getter
    public GameObject[,,] GetBlocks()
    {
        return blocks;
    }
    public int CountExposedFaces()
    {
        int count = 0;
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < blocksPerSide; x++)
            {
                for(int z = 0; z < blocksPerSide; z++)
                {
                    if(blocks[y, x, z] != null)
                    {
                        count += blocks[y, x, z].GetComponent<Block>().GetNumExposedFaces();
                    }
                }
            }
        }
        return count;
    }
}
