using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Point2D
{
    public int x;
    public int z;

    public Point2D(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public struct Point3D
{
    public int x;
    public int y;
    public int z;

    public Point3D(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public struct StructureFootprint
{
    public int x, z, xWidth, zWidth;
    public StructureFootprint(int x, int z, int xWidth, int zWidth)
    {
        this.x = x;           this.z = z;
        this.xWidth = xWidth; this.zWidth = zWidth;
    }
}

public class ChunkManager : MonoBehaviour
{
    // Always keep track of where the player is
    private int currentPlayerChunkID;

    // A dictionary of every chunk that has been generated
    private Dictionary<int, GameObject> allSeenChunks;
    // The ID's of chunks currently within the render radius
    private SortedSet<int> currentChunkIDs;

    private List<StructureFootprint> structureFootprints = new List<StructureFootprint>();
    private const int STRUCTURE_TRIES = 5;

    // The player can change the render radius
    public int renderRadius = 2;

    private int lookedAtChunkID = 0;

    // Determines the total world size (this number is half of the sidelength of the world)
    private static int numberOfChunks = 3;
    private static int worldBorderLength = numberOfChunks * Chunk.blocksPerSide;

    // Textures
    public Texture grassTex;
    public Texture grassHighlightTex;
    public Texture stoneTex;
    public Texture stoneHighlightTex;
    public Texture woodTex;
    public Texture woodHighlightTex;

    public GameObject chunkBorderPrefab;
    public GameObject worldBorderPrefab;

    // Keep track of where the player is to decide which Chunks
    // to be active
    public Transform playerTransform;
    public float playerHeight;

    // Need the camera for raycasting
    public Camera camera;
    public float raycastDistance = 7f;

    // Perlin noise parameters for terrain height
    public int mapWidth = Chunk.blocksPerSide;
    public int mapHeight = Chunk.blocksPerSide;
    public int seed;
    public float scale = 2f;
    public int octaves = 8;
    public float persistence = 1f;
    public float lacunarity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Initiate Perlin noise
        seed = Mathf.FloorToInt(Random.Range(0, int.MaxValue));

        // Initiate chunks
        currentPlayerChunkID = getChunkIDContainingPoint(playerTransform.position, Chunk.blocksPerSide);
        allSeenChunks = new Dictionary<int, GameObject>();
        currentChunkIDs = new SortedSet<int>();
        // Create the square of chunks first. Do the work up front to avoid lag.
        GenerateSquareOfChunks(numberOfChunks);
        // Test generating structures
        this.GenerateStructureFromFile("house.txt");

        // Set the radius of visible chunks
        updateChunks();    

        // Put the player on the ground
        float groundLevel = allSeenChunks[currentPlayerChunkID].GetComponent<Chunk>().GetGroundLevel(playerTransform.position);
        playerTransform.position = playerTransform.position + Vector3.up * (groundLevel + playerHeight / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // Update chunks if the player moved
        if (updateCurrentPlayerChunkID())
        {
            updateChunks();
        }
        Raycast();
        ReactToClick();
        ReactToRightClick();
    }

    // If the player enters a new chunk, return true and update the chunk id
    private bool updateCurrentPlayerChunkID()
    {
        int newChunkID = getChunkIDContainingPoint(playerTransform.position, Chunk.blocksPerSide);
        if (newChunkID != currentPlayerChunkID)
        {
            currentPlayerChunkID = newChunkID;
            return true;
        }
        return false;
    }

    private void updateChunks()
    {
        // Calculate the list of new chunkIDs
        SortedSet<int> newCurrentChunkIDs = GetChunkIDsAroundID(currentPlayerChunkID, renderRadius);

        // If a previous chunk is not in the new radius, disable it
        foreach(int i in currentChunkIDs)
        {
            if (!newCurrentChunkIDs.Contains(i) && allSeenChunks.ContainsKey(i))
            {
                allSeenChunks[i].GetComponent<Chunk>().DisableChunk();
            }
        }
        // If a new chunk was not in the previous radius, enable it
        foreach (int i in newCurrentChunkIDs)
        {
            if (!currentChunkIDs.Contains(i) && allSeenChunks.ContainsKey(i))
            {
                allSeenChunks[i].GetComponent<Chunk>().EnableChunk();
            }
        }

        // Update the list
        currentChunkIDs = newCurrentChunkIDs;

        // Go through the new chunks and add/create them

        /*for (int i = 0; i < chunkIDs.Count; i++)
        {
            int id = (chunkIDs[i]);
            if (allSeenChunks.ContainsKey(id))
            {
                currentChunks.Add(allSeenChunks[id]);
                allSeenChunks[id].GetComponent<Chunk>().EnableChunk();
            }
            else
            {
                //CreateChunk(id);
            }
        }*/
    }

    public void UpdateRenderRadius(int newRadius)
    {
        this.renderRadius = newRadius;
        this.updateChunks();
    }

    public GameObject CreateChunk(int id)
    {
        GameObject c = new GameObject();
        c.AddComponent<Chunk>();
        c.GetComponent<Chunk>().SetChunkID(id);
        Vector2 offset = c.GetComponent<Chunk>().GetTerrainOffset();
        c.GetComponent<Chunk>().SetTerrainHeights(Noise.GenerateNoiseMap(mapWidth, mapHeight, seed,
                                                    scale, octaves, persistence, lacunarity, offset,
                                                    Noise.NormalizeMode.Global));
        c.GetComponent<Chunk>().SetGrassTextures(grassTex, grassHighlightTex);
        c.GetComponent<Chunk>().SetStoneTextures(stoneTex, stoneHighlightTex);
        c.GetComponent<Chunk>().SetWoodTextures(woodTex, woodHighlightTex);
        c.GetComponent<Chunk>().InitializeBlocks();
        //c.GetComponent<Chunk>().CreateChunkBorders(chunkBorderPrefab);

        // Set the neighbors, if they exist
        // (and the set reverse neighbor direction)
        int northID = GetNorthChunkID(id);
        if (allSeenChunks.ContainsKey(northID))
        {
            c.GetComponent<Chunk>().SetNorthNeighbor(allSeenChunks[northID].GetComponent<Chunk>());
            allSeenChunks[northID].GetComponent<Chunk>().SetSouthNeighbor(c.GetComponent<Chunk>());
        }
        int southID = GetSouthChunkID(id);
        if (allSeenChunks.ContainsKey(southID))
        {
            c.GetComponent<Chunk>().SetSouthNeighbor(allSeenChunks[southID].GetComponent<Chunk>());
            allSeenChunks[southID].GetComponent<Chunk>().SetNorthNeighbor(c.GetComponent<Chunk>());
        }
        int eastID = GetEastChunkID(id);
        if (allSeenChunks.ContainsKey(eastID))
        {
            c.GetComponent<Chunk>().SetEastNeighbor(allSeenChunks[eastID].GetComponent<Chunk>());
            allSeenChunks[eastID].GetComponent<Chunk>().SetWestNeighbor(c.GetComponent<Chunk>());
        }
        int westID = GetWestChunkID(id);
        if (allSeenChunks.ContainsKey(westID))
        {
            c.GetComponent<Chunk>().SetWestNeighbor(allSeenChunks[westID].GetComponent<Chunk>());
            allSeenChunks[westID].GetComponent<Chunk>().SetEastNeighbor(c.GetComponent<Chunk>());
        }

        c.GetComponent<Chunk>().EnableChunk();
        allSeenChunks.Add(id, c);
        currentChunkIDs.Add(id);
        return c;
        //Debug.Log("new chunk " + id + " has exposed faces: " + allSeenChunks[id].GetComponent<Chunk>().CountExposedFaces());
    }

    public void GenerateSquareOfChunks(int squareSize)
    {
        for(int i = -squareSize; i <= squareSize; i++)
        {
            for (int j = -squareSize; j <= squareSize; j++)
            {
                int id = ChunkManager.chunkCoordsToChunkID(i, j);
                GameObject c = CreateChunk(id);
                if(i == -squareSize)
                {
                    c.GetComponent<Chunk>().AddWorldBorderWest(worldBorderPrefab);
                }
                else if(i == squareSize)
                {
                    c.GetComponent<Chunk>().AddWorldBorderEast(worldBorderPrefab);
                }
                if (j == -squareSize)
                {
                    c.GetComponent<Chunk>().AddWorldBorderSouth(worldBorderPrefab);
                }
                else if (j == squareSize)
                {
                    c.GetComponent<Chunk>().AddWorldBorderNorth(worldBorderPrefab);
                }
            }
        }
    }

    // ==========================================================
    //
    //                     Math Things
    //
    // ==========================================================
    public static int nearestPerfectSquare(int n)
    {
        int squareJumpAmount = 3;
        int curSquare = 1;
        int prevSquare = 0;
        while (curSquare < n)
        {
            prevSquare = curSquare;
            curSquare += squareJumpAmount;
            squareJumpAmount += 2;  // the difference between consecutive squares is odd integer
        }
        if (n - prevSquare > curSquare - n)
        {
            return curSquare;
        }
        else
        {
            return prevSquare;
        }
    }
    // Assuming n is a perfect square, return the square root of n as an int
    public static int isqrt(int n)
    {
        return (int)Mathf.Round(Mathf.Sqrt(n));
    }
    // Convert a ChunkID to the coordinates of the chunk
    public static Point2D chunkIDtoPoint2D(int n)
    {
        int s = nearestPerfectSquare(n);
        int sq = isqrt(s);
        if (s % 2 == 0)
        {
            if (n >= s)
            {
                return new Point2D(sq / 2, -sq / 2 + n - s);
            }
            else
            {
                return new Point2D(sq / 2 - s + n, -sq / 2);
            }
        }
        else
        {
            if (n >= s)
            {
                return new Point2D(-(sq + 1) / 2, (sq + 1) / 2 - 1 - n + s);
            }
            else
            {
                return new Point2D(-(sq + 1) / 2 + s - n, (sq + 1) / 2 - 1);
            }
        }
    }
    // Wrapper
    public static Vector2 chunkIDtoVector2(int n)
    {
        Point2D converted = chunkIDtoPoint2D(n);
        return new Vector2(converted.x, converted.z);
    }
    // Convert the coordinates of the chunk to the ChunkID
    public static int chunkCoordsToChunkID(int a, int b)
    {
        // Bottom Zone
        if (b > 0 && a >= -b && a < b)
        {
            return 4 * b * b + 3 * b - a;
        }
        // Left Zone
        else if (a < 0 && b < -a && b >= a)
        {
            return 4 * a * a + 3 * a - b;
        }
        // Top Zone
        else if (b < 0 && a <= -b && a > b)
        {
            return 4 * b * b + b + a;
        }
        // Right Zone
        else if (a > 0 && b <= a && b > -a)
        {
            return 4 * a * a + a + b;
        }
        // Only a=0, b=0 is not in a zone
        else
        {
            return 0;
        }
    }
    // Wrapper function
    public static int point2DtoChunkID(Point2D p)
    {
        return chunkCoordsToChunkID(p.x, p.z);
    }
    public static float distanceFormula(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
    public static float distance2d(Vector3 v1, Vector3 v2)
    {
        return distanceFormula(v1.x, v1.z, v2.x, v2.z);
    }
    // Get a diamond shape of chunk ids centered around the given point
    public static SortedSet<int> getChunkIDsAroundPoint(Point2D p, int radius)
    {
        SortedSet<int> result = new SortedSet<int>();

        // Start at the bottom of the diamond and work up from there
        for (int b = p.z + radius; b >= p.z - radius; b--)
        {
            int distanceFromZ = Mathf.Abs(b - p.z);
            for (int a = p.x - (radius - distanceFromZ); a <= p.x + (radius - distanceFromZ); a++)
            {
                result.Add(chunkCoordsToChunkID(a, b));
            }
        }
        return result;
    }
    // Wrapper
    public static SortedSet<int> GetChunkIDsAroundID(int id, int radius)
    {
        return getChunkIDsAroundPoint(chunkIDtoPoint2D(id), radius);
    }
    // Get the chunkID of the chunk containing a given point
    public int getChunkIDContainingPoint(Vector3 p, int chunkSize)
    {
        int x = (int)Mathf.Floor(p.x / chunkSize);
        int z = (int)Mathf.Floor(p.z / chunkSize);
        return chunkCoordsToChunkID(x, z);
    }

    // Get IDs of neighboring chunks
    public static int GetNorthChunkID(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        chunkCoords.z += 1;
        return point2DtoChunkID(chunkCoords);
    }
    public static int GetSouthChunkID(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        chunkCoords.z -= 1;
        return point2DtoChunkID(chunkCoords);
    }
    public static int GetEastChunkID(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        chunkCoords.x += 1;
        return point2DtoChunkID(chunkCoords);
    }
    public static int GetWestChunkID(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        chunkCoords.x -= 1;
        return point2DtoChunkID(chunkCoords);
    }
    
    
    // Raycast stuff for getting the block the player is looking at
    public void Raycast()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        string[] collidableLayers = { "Block Faces", "Ground" };

        LayerMask mask = LayerMask.GetMask(collidableLayers);

        if (Physics.Raycast(ray, out hit, raycastDistance, mask))
        {
            Transform objectHit = hit.transform;
            int newLookedAtChunkID = ChunkManager.GetIDOfChunkContainingPoint(objectHit);
            if(newLookedAtChunkID != lookedAtChunkID)
            {
                allSeenChunks[lookedAtChunkID].GetComponent<Chunk>().unHighlight();
                lookedAtChunkID = newLookedAtChunkID;
            }

            if(allSeenChunks.ContainsKey(lookedAtChunkID))
            {
                Chunk lookedAtChunk = allSeenChunks[lookedAtChunkID].GetComponent<Chunk>();
                if(lookedAtChunk.GetIsActive())
                {
                    lookedAtChunkID = lookedAtChunk.ReactToRaycastHit(objectHit);
                }
            }
        }
        else
        {
            if (allSeenChunks.ContainsKey(lookedAtChunkID))
            {
                Chunk lookedAtChunk = allSeenChunks[lookedAtChunkID].GetComponent<Chunk>();
                if (lookedAtChunk.GetIsActive())
                {
                    lookedAtChunk.unHighlight();
                }
            }
        }
    }
    public static int GetIDOfChunkContainingPoint(Transform loc)
    {
        int chunkSize = Chunk.blocksPerSide;
        int x = Mathf.FloorToInt(loc.position.x / chunkSize);
        int z = Mathf.FloorToInt(loc.position.z / chunkSize);
        return ChunkManager.chunkCoordsToChunkID(x, z);
    }

    public void ReactToClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(allSeenChunks.ContainsKey(lookedAtChunkID))
            {
                Chunk lookedAtChunk = allSeenChunks[lookedAtChunkID].GetComponent<Chunk>();
                if (lookedAtChunk.GetIsActive())
                {
                    lookedAtChunk.ReactToClick();
                }
            }
        }
    }

    public void ReactToRightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (allSeenChunks.ContainsKey(lookedAtChunkID))
            {
                Chunk lookedAtChunk = allSeenChunks[lookedAtChunkID].GetComponent<Chunk>();
                if (lookedAtChunk.GetIsActive())
                {
                    lookedAtChunk.ReactToRightClick();
                }
            }
        }
    }

    // ====================================================
    //
    //        Functions for Generating Structures
    //
    // ====================================================
    public bool InsertBlockAtWorldCoords(int x, int y, int z, string texture, bool isBreakable)
    {
        int chunkID = this.getChunkIDContainingPoint(new Vector3(x, y, z), Chunk.blocksPerSide);
        // If the chunk is outside the world border, stop
        if(!allSeenChunks.ContainsKey(chunkID))
        {
            return false;
        }
        Chunk c = allSeenChunks[chunkID].GetComponent<Chunk>();
        return c.InsertBlockAtWorldCoords(x, y, z, texture, isBreakable);
    }

    // The string must be of the form
    // "x,y,z,texture,T/F"
    public bool ParseStringAndInsertBlock(int xBase, int yBase, int zBase, string blockInstruction)
    {
        // Ignore comments
        if(blockInstruction.StartsWith("//"))
        {
            return false;
        }
        // If not a comment, split on commas. Expect 5 pieces.
        string[] args = blockInstruction.Split(',');
        if(args.Length != 5)
        {
            Debug.LogError("Block instruction string must have 4 commas");
            return false;
        }
        int x, y, z;
        bool success = true;
        success = int.TryParse(args[0], out x) && success;
        success = int.TryParse(args[1], out y) && success;
        success = int.TryParse(args[2], out z) && success;
        if(!success)
        {
            Debug.LogError("Could not parse ints " + args[0] + " " + args[1] + " " + args[2]);
            return false;
        }

        string texture = args[3];
        bool isBreakable = (args[4] == "T" || args[4] == "t" || args[4] == "true" || args[4] == "true");
        return InsertBlockAtWorldCoords(x + xBase, y + yBase, z + zBase, texture, isBreakable);
    }

    public void GenerateStructureFromFile(string filename)
    {
        // This is based on a tutorial by PrefixWiz https://www.youtube.com/watch?v=1OOWHB-BOAY
        string filepath = Application.streamingAssetsPath + "/StructureFiles/" + filename;
        string[] fileLines = File.ReadAllLines(filepath);

        // The first line should be the "x,y,z" dimensions of the structure
        string[] args = fileLines[0].Split(',');
        if (args.Length != 3)
        {
            Debug.LogError("Structure dimension string must have 2 commas, but received " + fileLines[0]);
            return;
        }
        int xSize, ySize, zSize;
        bool success = true;
        success = int.TryParse(args[0], out xSize) && success;
        success = int.TryParse(args[1], out ySize) && success;
        success = int.TryParse(args[2], out zSize) && success;
        if (!success)
        {
            Debug.LogError("Could not parse ints " + args[0] + " " + args[1] + " " + args[2]);
            return;
        }

        // Choose where to put the structure based on its dimensions
        for(int i = 0; i < STRUCTURE_TRIES; i++)
        {
            // Get a random location to try building the structure
            int x = Random.Range(-worldBorderLength + xSize, worldBorderLength - xSize);
            int z = Random.Range(-worldBorderLength + zSize, worldBorderLength - zSize);
            int groundLevel = GetGroundLevelAtWorldCoords(x, z);

            // If the ground is too high, no
            if(groundLevel + ySize > Chunk.worldHeight)
            {
                continue;
            }

            // Check if it would overlap with an existing structure
            bool overlaps = false;
            foreach(StructureFootprint fp in structureFootprints)
            {
                if(OverlapsFootprint(x, z, xSize, zSize, fp))
                {
                    overlaps = true;
                }
            }
            if(overlaps)
            {
                continue;
            }

            // If there were no problems, make the new structure
            for (int j = 1; j < fileLines.Length; j++)
            {
                this.ParseStringAndInsertBlock(x, groundLevel, z, fileLines[j]);
            }
            structureFootprints.Add(new StructureFootprint(x, z, xSize, zSize));
            return;
        }  
    }

    public int GetGroundLevelAtWorldCoords(int x, int z)
    {
        Vector3 location = new Vector3(x, 1, z);
        int chunkID = this.getChunkIDContainingPoint(location, Chunk.blocksPerSide);
        return (int)allSeenChunks[chunkID].GetComponent<Chunk>().GetGroundLevel(location);
    }

    // Check if these parameters describe a structure that would overlap an existing footprint
    public bool OverlapsFootprint(int x, int z, int xWidth, int zWidth, StructureFootprint footprint)
    {
        return !(x > footprint.x + footprint.xWidth || x + xWidth < footprint.x ||
            z > footprint.z + footprint.zWidth || z + zWidth < footprint.z);
    }
}

