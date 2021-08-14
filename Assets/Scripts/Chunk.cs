using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, North, South, East, West};

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
    public static int blocksPerSideSquared = blocksPerSide * blocksPerSide;
    public static int terrainHeight = 4;
    public static int worldHeight = 60;
    //public static int groundLevel = 1;
    private int perlinValue;
    private Vector3 bottomLeft;

    // Neighboring chunks
    private Chunk northNeighbor;
    private Chunk eastNeighbor;
    private Chunk southNeighbor;
    private Chunk westNeighbor;

    // Is the chunk currently loaded?
    private bool isActive = true;

    // The terrain height
    private float[,] terrainHeights;

    // We keep track of if a block is being looked at
    private GameObject highlightedBlock = null;
    private Point3D highlightedIndex;
    private Direction blockHighlightSide;

    // Textures
    private Texture grassTex;
    private Texture grassHighlightTex;
    private Texture stoneTex;
    private Texture stoneHighlightTex;
    private Texture woodTex;
    private Texture woodHighlightTex;
    private Texture limestoneTex;
    private Texture limestoneHighlightTex;
    private Texture limestonefenceTex;
    private Texture limestonefenceHighlightTex;

    private GameObject[,,] blocks = new GameObject[worldHeight + 1, blocksPerSide, blocksPerSide];
    private HashSet<int> activeBlockLocations = new HashSet<int>();

    private GameObject chunkBorderPrefab;
    private GameObject northChunkBorder;
    private GameObject southChunkBorder;
    private GameObject eastChunkBorder;
    private GameObject westChunkBorder;

    private GameObject northWorldBorder;
    private GameObject southWorldBorder;
    private GameObject eastWorldBorder;
    private GameObject westWorldBorder;

    // The size of a block for checking collisions
    private static Vector3 blockHalfExtents = new Vector3(0.4f, 0.4f, 0.4f);

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
        foreach (int index in activeBlockLocations)
        {
            int x, y, z;
            IndexToBlockCoords(index, out x, out y, out z);
            blocks[y, x, z].GetComponent<Block>().EnableRendering();
        }
        /*for (int y = 0; y <= worldHeight; y++)
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
        }*/
        if(northWorldBorder)
        {
            northWorldBorder.SetActive(true);
        }
        if (southWorldBorder)
        {
            southWorldBorder.SetActive(true);
        }
        if (eastWorldBorder)
        {
            eastWorldBorder.SetActive(true);
        }
        if (westWorldBorder)
        {
            westWorldBorder.SetActive(true);
        }
        isActive = true;
    }
    public void DisableChunk()
    {
        foreach(int index in activeBlockLocations)
        {
            int x, y, z;
            IndexToBlockCoords(index, out x, out y, out z);
            blocks[y, x, z].GetComponent<Block>().DisableRendering();
        }
        /*for (int y = 0; y <= worldHeight; y++)
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
        }*/
        if (northWorldBorder)
        {
            northWorldBorder.SetActive(false);
        }
        if (southWorldBorder)
        {
            southWorldBorder.SetActive(false);
        }
        if (eastWorldBorder)
        {
            eastWorldBorder.SetActive(false);
        }
        if (westWorldBorder)
        {
            westWorldBorder.SetActive(false);
        }
        isActive = false;
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
        // Store the bottom left of the chunk in world coordinates
        bottomLeft = new Vector3(chunkCoords.x * blocksPerSide, 0, chunkCoords.z * blocksPerSide);
    }
    
    public void SetTextures(Dictionary<string, Texture> textureDict)
    {
        grassTex = textureDict["grass"];
        grassHighlightTex = textureDict["grassH"];
        stoneTex = textureDict["stone"];
        stoneHighlightTex = textureDict["stoneH"];
        woodTex = textureDict["wood"];
        woodHighlightTex = textureDict["woodH"];
        limestoneTex = textureDict["limestone"];
        limestoneHighlightTex = textureDict["limestoneH"];
        limestonefenceTex = textureDict["limestonefence"];
        limestonefenceHighlightTex = textureDict["limestonefenceH"];
    }
    public void SetTerrainHeights(float[,] input)
    {
        terrainHeights = input;
    }

    public void InitializeBlocks()
    {
        // The unbreakable layer at the bottom
        for(int x = 0; x < blocksPerSide; x++)
        {
            for(int z = 0; z < blocksPerSide; z++)
            {
                GameObject block = new GameObject();
                block.AddComponent<Block>();
                block.transform.SetParent(transform);
                block.transform.localPosition = new Vector3(x + 0.5f, 0.5f, z + 0.5f);
                block.GetComponent<Block>().CreateFaces();
                block.GetComponent<Block>().SetTextures(stoneTex, stoneHighlightTex);
                block.GetComponent<Block>().ApplyMainTexture();
                block.GetComponent<Block>().SetCanBeBroken(false);
                block.GetComponent<Block>().SetChunkID(chunkID);
                block.GetComponent<Block>().SetIndexInChunk(x, 0, z);
                blocks[0, x, z] = block;
                this.activeBlockLocations.Add(BlockCoordsToIndex(x, 0, z));
            }
        }

        // Create everything above the bottom
        for(int x = 0; x < blocksPerSide; x++)
        {
            for(int z = 0; z < blocksPerSide; z++)
            {
                // Get the terrain height at this spot in the chunk
                // No idea why 15 - z is necessary instead of just z, but that's what makes it work
                int groundLevel = Mathf.Min(terrainHeight, Mathf.FloorToInt(1.5f* terrainHeight * this.terrainHeights[x, 15 - z]) + 1);
                for (int y = 1; y <= groundLevel; y++)
                {
                    GameObject block = new GameObject();
                    block.AddComponent<Block>();
                    block.transform.SetParent(transform);
                    block.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    block.GetComponent<Block>().CreateFaces();
                    block.GetComponent<Block>().SetTextures(grassTex, grassHighlightTex);
                    block.GetComponent<Block>().ApplyMainTexture();
                    block.GetComponent<Block>().SetCanBeBroken(true);
                    block.GetComponent<Block>().SetChunkID(chunkID);
                    block.GetComponent<Block>().SetIndexInChunk(x, y, z);
                    blocks[y, x, z] = block;
                    this.activeBlockLocations.Add(BlockCoordsToIndex(x, y, z));
                }
            }
        }
        SetVerticalBlockNeighbors();
        SetInteriorBlockNeighbors();
    }

    // The top and bottom neighbors of the blocks are only within this chunk
    private void SetVerticalBlockNeighbors()
    {
        for(int y = 0; y <= worldHeight - 1; y++)
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
        for(int y = 0; y <= worldHeight; y++)
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
        for(int y = 0; y <= worldHeight; y++)
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
        northNeighbor.SetSouthNeighborChunkOnly(this);

        // Give the blocks their neighbors
        int z = blocksPerSide - 1;
        for(int y = 0; y <= worldHeight; y++)
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
        southNeighbor.SetNorthNeighborChunkOnly(this);

        // Give the blocks their neighbors
        int z = 0;
        for (int y = 0; y <= worldHeight; y++)
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
        eastNeighbor.SetWestNeighborChunkOnly(this);

        // Give the blocks their neighbors
        int x = blocksPerSide - 1;
        for (int y = 0; y <= worldHeight; y++)
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
        westNeighbor.SetEastNeighborChunkOnly(this);

        // Give the blocks their neighbors
        int x = 0;
        for (int y = 0; y <= worldHeight; y++)
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

    public void SetNorthNeighborChunkOnly(Chunk input)
    {
        northNeighbor = input;
    }
    public void SetSouthNeighborChunkOnly(Chunk input)
    {
        southNeighbor = input;
    }
    public void SetEastNeighborChunkOnly(Chunk input)
    {
        eastNeighbor = input;
    }
    public void SetWestNeighborChunkOnly(Chunk input)
    {
        westNeighbor = input;
    }

    public void CreateChunkBorders(GameObject inputBorderPrefab)
    {
        chunkBorderPrefab = inputBorderPrefab;
        northChunkBorder = Instantiate(chunkBorderPrefab);
        northChunkBorder.transform.SetParent(transform);
        northChunkBorder.transform.localScale = new Vector3(blocksPerSide, blocksPerSide, 1);
        northChunkBorder.transform.localPosition = new Vector3(blocksPerSide / 2, blocksPerSide / 2, blocksPerSide);
        southChunkBorder = Instantiate(chunkBorderPrefab);
        southChunkBorder.transform.SetParent(transform);
        southChunkBorder.transform.localScale = new Vector3(blocksPerSide, blocksPerSide, 1);
        southChunkBorder.transform.localPosition = new Vector3(blocksPerSide / 2, blocksPerSide / 2, 0);
        southChunkBorder.transform.Rotate(Vector3.up, 180f);
        eastChunkBorder = Instantiate(chunkBorderPrefab);
        eastChunkBorder.transform.SetParent(transform);
        eastChunkBorder.transform.localScale = new Vector3(blocksPerSide, blocksPerSide, 1);
        eastChunkBorder.transform.localPosition = new Vector3(blocksPerSide, blocksPerSide / 2, blocksPerSide/2);
        eastChunkBorder.transform.Rotate(Vector3.up, 90f);
        westChunkBorder = Instantiate(chunkBorderPrefab);
        westChunkBorder.transform.SetParent(transform);
        westChunkBorder.transform.localScale = new Vector3(blocksPerSide, blocksPerSide, 1);
        westChunkBorder.transform.localPosition = new Vector3(0, blocksPerSide / 2, blocksPerSide/2);
        westChunkBorder.transform.Rotate(Vector3.up, 270f);
    }

    // Helper function for creating world borders
    public void AddWorldBorderNorth(GameObject prefab)
    {
        northWorldBorder = Instantiate(prefab);
        northWorldBorder.transform.SetParent(transform);
        northWorldBorder.transform.localScale = new Vector3(blocksPerSide, worldHeight, 1);
        northWorldBorder.transform.localPosition = new Vector3(blocksPerSide / 2, worldHeight / 2, blocksPerSide);
    }
    public void AddWorldBorderSouth(GameObject prefab)
    {
        southWorldBorder = Instantiate(prefab);
        southWorldBorder.transform.SetParent(transform);
        southWorldBorder.transform.localScale = new Vector3(blocksPerSide, worldHeight, 1);
        southWorldBorder.transform.localPosition = new Vector3(blocksPerSide / 2, worldHeight / 2, 0);
        southWorldBorder.transform.Rotate(Vector3.up, 180f);
    }
    public void AddWorldBorderEast(GameObject prefab)
    {
        eastWorldBorder = Instantiate(prefab);
        eastWorldBorder.transform.SetParent(transform);
        eastWorldBorder.transform.localScale = new Vector3(blocksPerSide, worldHeight, 1);
        eastWorldBorder.transform.localPosition = new Vector3(blocksPerSide, worldHeight / 2, blocksPerSide / 2);
        eastWorldBorder.transform.Rotate(Vector3.up, 90f);
    }
    public void AddWorldBorderWest(GameObject prefab)
    {
        westWorldBorder = Instantiate(prefab);
        westWorldBorder.transform.SetParent(transform);
        westWorldBorder.transform.localScale = new Vector3(blocksPerSide, worldHeight, 1);
        westWorldBorder.transform.localPosition = new Vector3(0, worldHeight / 2, blocksPerSide / 2);
        westWorldBorder.transform.Rotate(Vector3.up, 270f);
    }


    // Getters
    public GameObject[,,] GetBlocks()
    {
        return blocks;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
    public bool HasBlockAt(int x, int y, int z)
    {
        return (blocks[y, x, z] != null);
    }
    public int CountExposedFaces()
    {
        int count = 0;
        for(int y = 0; y <= worldHeight; y++)
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
    public Vector3 GetBottomLeft()
    {
        return bottomLeft;
    }
    public Vector2 GetTerrainOffset()
    {
        return new Vector2(bottomLeft.x, bottomLeft.z);
    }

    // Return the index (x, z) of the block that the given position
    // is on, in the xz-plane
    public Point2D GetBlockIndex(Vector3 position)
    {
        Vector3 localPosition = position - transform.position;
        return new Point2D(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.z));
    }
    // Calculate what actual ground level is for this Chunk at the given position
    public float GetGroundLevel(Vector3 position)
    {
        Point2D indices = GetBlockIndex(position);
        int x = indices.x;
        int z = indices.z;
        int y = worldHeight;
        while(blocks[y,x,z] == null)
        {
            y--;
        }
        return y + 1;
    }
    public int GetChunkID()
    {
        return chunkID;
    }

    public Vector3 BlockIndexToWorldCoordinates(Point3D blockCoords)
    {
        float x = bottomLeft.x + blockCoords.x + 0.5f;
        float y = blockCoords.y + 0.5f;
        float z = bottomLeft.z + blockCoords.z + 0.5f;
        return new Vector3(x, y, z);
    }

    // When the player is looking at a location in this chunk
    // It could be that the hit is on the Chunk border and should be
    // in a neighboring Chunk. This returns the ID of the true Chunk
    public int ReactToRaycastHit(Transform hit)
    {
        Vector3 localHit = hit.position - bottomLeft;
        int x, y, z;

        // If the face is the top face
        if (hit.localRotation.eulerAngles.x == 90)
        {
            x = Mathf.FloorToInt(localHit.x);
            y = Mathf.FloorToInt(localHit.y - 0.5f);
            z = Mathf.FloorToInt(localHit.z);
            this.blockHighlightSide = Direction.Up;
        }
        // Bottom face
        else if(hit.localRotation.eulerAngles.x == 270)
        {
            x = Mathf.FloorToInt(localHit.x);
            y = Mathf.FloorToInt(localHit.y);
            z = Mathf.FloorToInt(localHit.z);
            this.blockHighlightSide = Direction.Down;
        }
        // North face
        else if(hit.localRotation.eulerAngles.y == 180)
        {
            if(localHit.z == 0) // Check if it should be neighbor chunk
            {
                return southNeighbor.ReactToRaycastHit(hit);
            }
            x = Mathf.FloorToInt(localHit.x - 0.5f);
            y = Mathf.FloorToInt(localHit.y);
            z = Mathf.FloorToInt(localHit.z - 0.5f);
            this.blockHighlightSide = Direction.North;
        }
        // East face
        else if(hit.localRotation.eulerAngles.y == 270)
        {
            if (localHit.x == 0) // Check if it should be neighbor chunk
            {
                return westNeighbor.ReactToRaycastHit(hit);
            }
            x = Mathf.FloorToInt(localHit.x - 0.5f);
            y = Mathf.FloorToInt(localHit.y);
            z = Mathf.FloorToInt(localHit.z - 0.5f);
            this.blockHighlightSide = Direction.East;
        }
        // West face
        else if(hit.localRotation.eulerAngles.y == 90)
        {
            if (localHit.x == blocksPerSide) // Check if it should be neighbor chunk
            {
                return eastNeighbor.ReactToRaycastHit(hit);
            }
            x = Mathf.FloorToInt(localHit.x);
            y = Mathf.FloorToInt(localHit.y);
            z = Mathf.FloorToInt(localHit.z);
            this.blockHighlightSide = Direction.West;
        }
        // South face
        else
        {
            if (localHit.z == blocksPerSide) // Check if it should be neighbor chunk
            {
                return northNeighbor.ReactToRaycastHit(hit);
            }
            x = Mathf.FloorToInt(localHit.x);
            y = Mathf.FloorToInt(localHit.y);
            z = Mathf.FloorToInt(localHit.z);
            this.blockHighlightSide = Direction.South;
        }
        
        highlightBlock(x, y, z);
        return this.chunkID;
    }

    private int highlightBlock(int x, int y, int z)
    {
        if(highlightedBlock)
        {
            highlightedBlock.GetComponent<Block>().ApplyMainTexture();
        }
        highlightedBlock = blocks[y, x, z];
        highlightedIndex = new Point3D(x, y, z);
        if(highlightedBlock == null)
        {
            Debug.Log("there is no block at " + x + " " + y + " " + z);
            return 1;
        }
        highlightedBlock.GetComponent<Block>().ApplyHighlightTexture();
        return 0;
    }
    public void unHighlight()
    {
        if (highlightedBlock)
        {
            highlightedBlock.GetComponent<Block>().ApplyMainTexture();
        }
        highlightedBlock = null;
    }

    public void ReactToClick()
    {
        if (highlightedBlock && highlightedBlock.GetComponent<Block>().CanBeBroken())
        {
            highlightedBlock.GetComponent<Block>().RemoveSelf();
            blocks[highlightedIndex.y, highlightedIndex.x, highlightedIndex.z] = null;
            Destroy(highlightedBlock.GetComponent<Collider>());
            Destroy(highlightedBlock);
            highlightedBlock = null;
            activeBlockLocations.Remove(BlockCoordsToIndex(highlightedIndex.x, highlightedIndex.y, highlightedIndex.z));
        }
    }

    public void ReactToRightClick()
    {
        if (highlightedBlock != null && this.CanPlace())
        {
            int x = highlightedIndex.x;
            int y = highlightedIndex.y;
            int z = highlightedIndex.z;
            if(this.blockHighlightSide == Direction.Up)
            {
                PlaceBlock(x, y + 1, z);
                SetBlockNeighborConnections(x, y + 1, z);
            }
            else if(this.blockHighlightSide == Direction.Down)
            {
                PlaceBlock(x, y - 1, z);
                SetBlockNeighborConnections(x, y - 1, z);
            }
            else if(this.blockHighlightSide == Direction.North)
            {
                // If the block is being placed in the neighbor chunk
                if(z == blocksPerSide - 1)
                {
                    northNeighbor.PlaceBlock(x, y, 0);
                    northNeighbor.SetBlockNeighborConnections(x, y, 0);
                }
                else
                {
                    PlaceBlock(x, y, z + 1);
                    SetBlockNeighborConnections(x, y, z + 1);
                }
            }
            else if(this.blockHighlightSide == Direction.South)
            {
                // If the block is being placed in the neighbor chunk
                if (z == 0)
                {
                    southNeighbor.PlaceBlock(x, y, blocksPerSide - 1);
                    southNeighbor.SetBlockNeighborConnections(x, y, blocksPerSide - 1);
                }
                else
                {
                    PlaceBlock(x, y, z - 1);
                    SetBlockNeighborConnections(x, y, z - 1);
                }
            }
            else if(this.blockHighlightSide == Direction.East)
            {
                // If the block is being placed in the neighbor chunk
                if (x == blocksPerSide - 1)
                {
                    eastNeighbor.PlaceBlock(0, y, z);
                    eastNeighbor.SetBlockNeighborConnections(0, y, z);
                }
                else
                {
                    PlaceBlock(x + 1, y, z);
                    SetBlockNeighborConnections(x + 1, y, z);
                }
            }
            else if (this.blockHighlightSide == Direction.West)
            {
                // If the block is being placed in the neighbor chunk
                if (x == 0)
                {
                    westNeighbor.PlaceBlock(blocksPerSide - 1, y, z);
                    westNeighbor.SetBlockNeighborConnections(blocksPerSide - 1, y, z);
                }
                else
                {
                    PlaceBlock(x - 1, y, z);
                    SetBlockNeighborConnections(x - 1, y, z);
                }
            }
            else
            {
                Debug.LogError("highlightedBlock != null but blockHighlightSide = null");
            }
        }
    }

    public void PlaceBlock(int x, int y, int z)
    {
        GameObject block = new GameObject();
        block.AddComponent<Block>();
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        block.GetComponent<Block>().CreateFaces();
        block.GetComponent<Block>().SetTextures(grassTex, grassHighlightTex);
        block.GetComponent<Block>().SetChunkID(chunkID);
        block.GetComponent<Block>().SetCanBeBroken(true);
        block.GetComponent<Block>().SetIndexInChunk(x, y, z);
        block.GetComponent<Block>().ApplyMainTexture();
        blocks[y, x, z] = block;
        activeBlockLocations.Add(BlockCoordsToIndex(x, y, z));
    }

    // This assumes the x y z block has been created, but doesn't have neighbors set
    public void SetBlockNeighborConnections(int x, int y, int z)
    {
        GameObject neighbor;
        // West
        if(x == 0)
        {
            neighbor = this.westNeighbor.GetBlocks()[y, blocksPerSide - 1, z];
        }
        else
        {
            neighbor = blocks[y, x - 1, z];
        }
        if(neighbor != null)
        {
            neighbor.GetComponent<Block>().SetEastNeighbor(blocks[y, x, z]);
            blocks[y, x, z].GetComponent<Block>().SetWestNeighbor(neighbor);
        }
        // East
        if(x == blocksPerSide - 1)
        {
            neighbor = this.eastNeighbor.GetBlocks()[y, 0, z];
        }
        else
        {
            neighbor = blocks[y, x + 1, z];
        }
        if(neighbor != null)
        {
            neighbor.GetComponent<Block>().SetWestNeighbor(blocks[y, x, z]);
            blocks[y, x, z].GetComponent<Block>().SetEastNeighbor(neighbor);
        }
        // North
        if (z ==  blocksPerSide - 1)
        {
            neighbor = this.northNeighbor.GetBlocks()[y, x, 0];
        }
        else
        {
            neighbor = blocks[y, x, z + 1];
        }
        if(neighbor != null)
        {
            neighbor.GetComponent<Block>().SetSouthNeighbor(blocks[y, x, z]);
            blocks[y, x, z].GetComponent<Block>().SetNorthNeighbor(neighbor);
        }
        // South
        if (z == 0)
        {
            neighbor = this.southNeighbor.GetBlocks()[y, x, blocksPerSide - 1];
        }
        else
        {
            neighbor = blocks[y, x, z - 1];
        }
        if(neighbor != null)
        {
            neighbor.GetComponent<Block>().SetNorthNeighbor(blocks[y, x, z]);
            blocks[y, x, z].GetComponent<Block>().SetSouthNeighbor(neighbor);
        }
        // Top
        if (y != worldHeight)
        {
            neighbor = blocks[y + 1, x, z];
            if(neighbor != null)
            {
                neighbor.GetComponent<Block>().SetBottomNeighbor(blocks[y, x, z]);
                blocks[y, x, z].GetComponent<Block>().SetTopNeighbor(neighbor);
            }
        }
        // Bottom
        if (y != 0)
        {
            neighbor = blocks[y - 1, x, z];
            if(neighbor != null)
            {
                neighbor.GetComponent<Block>().SetTopNeighbor(blocks[y, x, z]);
                blocks[y, x, z].GetComponent<Block>().SetBottomNeighbor(neighbor);
            }
        }
    }

    // Checks if a block can be placed when the player is currently highlighting
    // Returns true if the target space is empty and within the y-limits of the game
    // and the block will not overlap with the player
    private bool CanPlace()
    {
        Point3D targetIndex;  // Where we want to place a block

        // First, check if the space has a block in it
        int y = highlightedIndex.y;
        Block b = highlightedBlock.GetComponent<Block>();
        if(this.blockHighlightSide == Direction.Up)
        {
            if(b.GetTopNeighbor() != null || y == worldHeight)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x, y + 1, highlightedIndex.z);
        }
        else if(this.blockHighlightSide == Direction.Down)
        {
            if(b.GetBottomNeighbor() != null || y == 0)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x, y - 1, highlightedIndex.z);
        }
        else if(this.blockHighlightSide == Direction.North)
        {
            if(b.GetNorthNeighbor() != null)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x, y, highlightedIndex.z + 1);
        }
        else if(this.blockHighlightSide == Direction.South)
        {
            if (b.GetSouthNeighbor() != null)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x, y, highlightedIndex.z - 1);
        }
        else if(this.blockHighlightSide == Direction.East)
        {
            if (b.GetEastNeighbor() != null)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x + 1, y, highlightedIndex.z);
        }
        else if(this.blockHighlightSide == Direction.West)
        {
            if (b.GetWestNeighbor() != null)
            {
                return false;
            }
            targetIndex = new Point3D(highlightedIndex.x - 1, y, highlightedIndex.z);
        }
        else
        {
            Debug.Log("there is not a highlighted block");
            return false;
        }

        // Then, check if it overlaps with the player
        Vector3 blockCenter = this.BlockIndexToWorldCoordinates(targetIndex);
        return !Physics.CheckBox(blockCenter, blockHalfExtents, Quaternion.identity);
    }

    // Insert a block when generating a structure
    public bool InsertBlockAtWorldCoords(int x, int y, int z, string texture, bool isBreakable)
    {
        // Convert world coords to local coords
        x = (int)(x - bottomLeft.x);
        z = (int)(z - bottomLeft.z);

        // Make sure the height is ok
        if(y < 0 || y > worldHeight)
        {
            return false;
        }

        Texture mainTex, highlightTex;
        // Get the texture
        if(texture == "stone")
        {
            mainTex = stoneTex; highlightTex = stoneHighlightTex;
        }
        else if(texture == "grass")
        {
            mainTex = grassTex; highlightTex = grassHighlightTex;
        }
        else if(texture == "wood")
        {
            mainTex = woodTex; highlightTex = woodHighlightTex;
        }
        else if(texture == "limestone")
        {
            mainTex = limestoneTex; highlightTex = limestoneHighlightTex;
        }
        else if(texture == "limestonefence")
        {
            mainTex = limestonefenceTex; highlightTex = limestonefenceHighlightTex;
        }
        else
        {
            Debug.Log("Invalid texture " + texture + ". Defaulting to grass.");
            mainTex = grassTex; highlightTex = grassHighlightTex;
        }

        // Create the new block
        GameObject block = new GameObject();
        block.AddComponent<Block>();
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        block.GetComponent<Block>().CreateFaces();
        block.GetComponent<Block>().SetTextures(mainTex, highlightTex);
        block.GetComponent<Block>().ApplyMainTexture();
        block.GetComponent<Block>().SetCanBeBroken(isBreakable);
        block.GetComponent<Block>().SetChunkID(chunkID);
        block.GetComponent<Block>().SetIndexInChunk(x, y, z);
        if(blocks[y, x, z] != null)
        {
            blocks[y, x, z].GetComponent<Block>().RemoveSelf();
        }
        blocks[y, x, z] = block;
        activeBlockLocations.Add(BlockCoordsToIndex(x, y, z));
        return true;
    }

    // Convert blocks to indices
    public static int BlockCoordsToIndex(int x, int y, int z)
    {
        return y * blocksPerSideSquared + x * blocksPerSide + z;
    }
    public static void IndexToBlockCoords(int index, out int x, out int y, out int z)
    {
        z = index % blocksPerSide;
        index -= z;
        index /= blocksPerSide;
        x = index % blocksPerSide;
        index -= x;
        index /= blocksPerSide;
        y = index;
    }
}
