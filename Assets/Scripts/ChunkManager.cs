using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

public struct TexturePair
{
    public Texture tex;
    public Texture highlight;

    public TexturePair(Texture input, Texture inputH)
    {
        tex = input;
        highlight = inputH;
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
    // Keep track of chunks that need to be enabled (true) or disabled (false)
    private Dictionary<int, bool> chunkLoadingJobs;

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
    public Texture limestoneTex;
    public Texture limestoneHighlightTex;
    public Texture limestonefenceTex;
    public Texture limestonefenceHighlightTex;
    public Texture darkglassTex;
    public Texture darkglassHighlightTex;
    public static Dictionary<BlockType, TexturePair> texDict = new Dictionary<BlockType, TexturePair>();

    public GameObject chunkBorderPrefab;
    public GameObject worldBorderPrefab;
    private bool chunkBordersShown = false;

    private int numNotreDameBlocks = 0;

    // Keep track of where the player is to decide which Chunks
    // to be active
    public Transform playerTransform;
    public float playerHeight;
    public CharacterController controller; // Need to move the player if out of bounds

    // Need the camera for raycasting
    public new Camera camera;
    public float raycastDistance = 7f;

    // Explosions
    public static int blastRadius = 10;
    public static float explosionProb = 1f;

    // Perlin noise parameters for terrain height
    public int mapWidth = Chunk.blocksPerSide;
    public int mapHeight = Chunk.blocksPerSide;
    public int seed;
    public float scale = 2f;
    public int octaves = 8;
    public float persistence = 1f;
    public float lacunarity = 1f;

    // Properties that are different for the start menu chunk manager
    public bool isInteractable;
    public bool makeWorldBorder;
    public bool userCanChooseGeneration;
    public bool generateCathedral;
    public bool generateHouses;
    public bool dramaticTerrain;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform.position = Vector3.zero;

        // Make the dictionary of textures
        texDict[BlockType.Grass] = new TexturePair(grassTex, grassHighlightTex);
        texDict[BlockType.Stone] = new TexturePair(stoneTex, stoneHighlightTex);
        texDict[BlockType.Wood] = new TexturePair(woodTex, woodHighlightTex);
        texDict[BlockType.Limestone] = new TexturePair(limestoneTex, limestoneHighlightTex);
        texDict[BlockType.Darkglass] = new TexturePair(darkglassTex, darkglassHighlightTex);
        texDict[BlockType.Limestonefence] = new TexturePair(limestonefenceTex, limestonefenceHighlightTex);

        // Initiate Perlin noise
        seed = Mathf.FloorToInt(Random.Range(0, int.MaxValue));

        // Initiate chunks
        currentPlayerChunkID = getChunkIDContainingPoint(playerTransform.position, Chunk.blocksPerSide);
        allSeenChunks = new Dictionary<int, GameObject>();
        currentChunkIDs = new SortedSet<int>();
        chunkLoadingJobs = new Dictionary<int, bool>();
                
        if(userCanChooseGeneration)
        {
            switch(StartMenuHandler.buildingMode)
            {
                case BuildingGenerationMode.All:
                    generateCathedral = true;
                    generateHouses = false;
                    dramaticTerrain = false;
                    break;
                case BuildingGenerationMode.Small:
                    generateCathedral = false;
                    generateHouses = true;
                    dramaticTerrain = false;
                    break;
                case BuildingGenerationMode.None:
                    generateCathedral = false;
                    generateHouses = false;
                    dramaticTerrain = false;
                    break;
                default:
                    generateCathedral = false;
                    generateHouses = false;
                    dramaticTerrain = true;
                    break;
            }
        }

        // Create the square of chunks first. Do the work up front to avoid lag.
        GenerateSquareOfChunks(numberOfChunks);

        // Generate structures
        if(generateCathedral)
        {
            this.GenerateStructureFromFile("notredame.txt");
            this.numNotreDameBlocks = CountUniqueBlocksInFile("notredame.txt");
            WinLoseStatus.numBlocks = this.numNotreDameBlocks;
        }
        //if(generateHouses)
        //{
         //   for(int _ = 0; _ < 15; _++)
         //   {
            //    this.GenerateStructureFromFile("house.txt");
          //  }
        //}

        // Set the radius of visible chunks
        //updateChunks();    

        // Put the player on the ground
        float groundLevel = allSeenChunks[currentPlayerChunkID].GetComponent<Chunk>().GetGroundLevel(playerTransform.position);
        playerTransform.position = playerTransform.position + Vector3.up * (2 + groundLevel + playerHeight / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseMenu.IsPaused)
        {
            return;
        }

        // Update chunks if the player moved
        if (updateCurrentPlayerChunkID())
        {
            //updateChunks();
        }
        //DoChunkLoadingJobs(1);
        if(isInteractable)
        {
            Raycast();
            ReactToClick();
            ReactToRightClick();
        }

        // End the game if the player is out of the world
        if(playerTransform.position.y < -10)
        {
            if(allSeenChunks.ContainsKey(currentPlayerChunkID))
            {
                float groundLevel = allSeenChunks[currentPlayerChunkID].GetComponent<Chunk>().GetGroundLevel(playerTransform.position);
                controller.Move(new Vector3(1000, 0, 0));
                controller.Move(new Vector3(0, 100 - playerTransform.position.y, 0));
                controller.Move(new Vector3(-1000, 0, 0));
                controller.Move(new Vector3(0, -90, 0));
            }
            else
            {
                WinLoseStatus.resultString = "You escaped the world. Good job, Kevin.";
                SceneManager.LoadScene("StartMenu");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
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
                //allSeenChunks[i].GetComponent<Chunk>().DisableChunk();
                chunkLoadingJobs[i] = false;
            }
        }
        // If a new chunk was not in the previous radius, enable it
        foreach (int i in newCurrentChunkIDs)
        {
            if (!currentChunkIDs.Contains(i) && allSeenChunks.ContainsKey(i))
            {
                //allSeenChunks[i].GetComponent<Chunk>().EnableChunk();
                chunkLoadingJobs[i] = true;
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
        if(dramaticTerrain)
        {
            c.GetComponent<Chunk>().SetTerrainHeights(Noise.GenerateNoiseMap(mapWidth, mapHeight, seed,
                                                    20, 7, 3, 1.2f, offset,
                                                    Noise.NormalizeMode.Global), 2.5f);
        }
        else
        {
            c.GetComponent<Chunk>().SetTerrainHeights(Noise.GenerateNoiseMap(mapWidth, mapHeight, seed,
                                                        scale, octaves, persistence, lacunarity, offset,
                                                        Noise.NormalizeMode.Global));
        }
        c.GetComponent<Chunk>().InitializeBlocks();
        c.GetComponent<Chunk>().CreateChunkBorders(chunkBorderPrefab);

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
                if(i == -squareSize && this.makeWorldBorder)
                {
                    c.GetComponent<Chunk>().AddWorldBorderWest(worldBorderPrefab);
                }
                else if(i == squareSize && this.makeWorldBorder)
                {
                    c.GetComponent<Chunk>().AddWorldBorderEast(worldBorderPrefab);
                }
                if (j == -squareSize && this.makeWorldBorder)
                {
                    c.GetComponent<Chunk>().AddWorldBorderSouth(worldBorderPrefab);
                }
                else if (j == squareSize && this.makeWorldBorder)
                {
                    c.GetComponent<Chunk>().AddWorldBorderNorth(worldBorderPrefab);
                }
            }
        }
    }

    public void DoChunkLoadingJobs(int n)
    {
        foreach(int id in chunkLoadingJobs.Keys)
        {
            if(chunkLoadingJobs[id])
            {
                allSeenChunks[id].GetComponent<Chunk>().EnableChunk();
            }
            else
            {
                allSeenChunks[id].GetComponent<Chunk>().DisableChunk();
            }
            chunkLoadingJobs.Remove(id);
            n--;
            if(n == 0)
            {
                return;
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
    // Or the x,y,z could be ranges
    // For example 1-5,2,1-5,stone,t would make 25 blocks
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
            Debug.LogError("Block instruction string must have 4 commas. Received " + blockInstruction);
            return false;
        }
        int x, y, z, xmin, xmax, ymin, ymax, zmin, zmax;
        bool success = true;
        success = int.TryParse(args[0], out x);
        if(success)
        {
            xmin = x; xmax = x;
        }
        else
        {
            string[] xargs = args[0].Split('-');
            if(xargs.Length != 2)
            {
                Debug.LogError("Invalid range specified: " + args[0]);
            }
            xmin = int.Parse(xargs[0]);
            xmax = int.Parse(xargs[1]);
        }
        success = int.TryParse(args[1], out y);
        if (success)
        {
            ymin = y; ymax = y;
        }
        else
        {
            string[] yargs = args[1].Split('-');
            if (yargs.Length != 2)
            {
                Debug.LogError("Invalid range specified: " + args[1]);
            }
            ymin = int.Parse(yargs[0]);
            ymax = int.Parse(yargs[1]);
        }
        success = int.TryParse(args[2], out z);
        if (success)
        {
            zmin = z; zmax = z;
        }
        else
        {
            string[] zargs = args[2].Split('-');
            if (zargs.Length != 2)
            {
                Debug.LogError("Invalid range specified: " + args[2]);
            }
            zmin = int.Parse(zargs[0]);
            zmax = int.Parse(zargs[1]);
        }
        if(xmin > xmax || ymin > ymax || zmin > zmax)
        {
            Debug.LogError("Specified range has min > max");
            return false;
        }

        string texture = args[3];
        bool isBreakable = (args[4] == "T" || args[4] == "t" || args[4] == "true" || args[4] == "true");
        for(x = xmin; x <= xmax; x++)
        {
            for(y = ymin; y <= ymax; y++)
            {
                for(z = zmin; z <= zmax; z++)
                {
                    InsertBlockAtWorldCoords(x + xBase, y + yBase, z + zBase, texture, isBreakable);
                }
            }
        }
        return true;
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
            // Fill in grass below the footprint
            for(int a = 0; a < xSize; a++)
            {
                for(int b = 1; b < groundLevel; b++)
                {
                    for(int c = 0; c < zSize; c++)
                    {
                        InsertBlockAtWorldCoords(a + x, b, c + z, "grass", true);
                    }
                }
            }
            structureFootprints.Add(new StructureFootprint(x, z, xSize, zSize));
            return;
        }  
    }

    public int CountUniqueBlocksInFile(string filename)
    {
        // This is based on a tutorial by PrefixWiz https://www.youtube.com/watch?v=1OOWHB-BOAY
        string filepath = Application.streamingAssetsPath + "/StructureFiles/" + filename;
        string[] fileLines = File.ReadAllLines(filepath);

        // The first line should be the "x,y,z" dimensions of the structure
        string[] firstLineArgs = fileLines[0].Split(',');
        if (firstLineArgs.Length != 3)
        {
            Debug.LogError("Structure dimension string must have 2 commas, but received " + fileLines[0]);
            return -1;
        }
        int xSize, ySize, zSize;
        bool success = true;
        success = int.TryParse(firstLineArgs[0], out xSize) && success;
        success = int.TryParse(firstLineArgs[1], out ySize) && success;
        success = int.TryParse(firstLineArgs[2], out zSize) && success;
        if (!success)
        {
            Debug.LogError("Could not parse ints " + firstLineArgs[0] + " " + firstLineArgs[1] + " " + firstLineArgs[2]);
            return -1;
        }

        bool[,,] blockLocations = new bool[xSize, ySize, zSize];

        for (int j = 1; j < fileLines.Length; j++)
        {
            string blockInstruction = fileLines[j];
            // Ignore comments
            if (blockInstruction.StartsWith("//"))
            {
                continue;
            }
            // If not a comment, split on commas. Expect 5 pieces.
            string[] args = blockInstruction.Split(',');
            if (args.Length != 5)
            {
                Debug.LogError("Block instruction string must have 4 commas. Received " + blockInstruction);
                return -1;
            }
            int x, y, z, xmin, xmax, ymin, ymax, zmin, zmax;
            success = true;
            success = int.TryParse(args[0], out x);
            if (success)
            {
                xmin = x; xmax = x;
            }
            else
            {
                string[] xargs = args[0].Split('-');
                if (xargs.Length != 2)
                {
                    Debug.LogError("Invalid range specified: " + args[0]);
                }
                xmin = int.Parse(xargs[0]);
                xmax = int.Parse(xargs[1]);
            }
            success = int.TryParse(args[1], out y);
            if (success)
            {
                ymin = y; ymax = y;
            }
            else
            {
                string[] yargs = args[1].Split('-');
                if (yargs.Length != 2)
                {
                    Debug.LogError("Invalid range specified: " + args[1]);
                }
                ymin = int.Parse(yargs[0]);
                ymax = int.Parse(yargs[1]);
            }
            success = int.TryParse(args[2], out z);
            if (success)
            {
                zmin = z; zmax = z;
            }
            else
            {
                string[] zargs = args[2].Split('-');
                if (zargs.Length != 2)
                {
                    Debug.LogError("Invalid range specified: " + args[2]);
                }
                zmin = int.Parse(zargs[0]);
                zmax = int.Parse(zargs[1]);
            }
            if (xmin > xmax || ymin > ymax || zmin > zmax)
            {
                Debug.LogError("Specified range has min > max");
                return -1;
            }

            string texture = args[3];
            bool isBreakable = (args[4] == "T" || args[4] == "t" || args[4] == "true" || args[4] == "true");
            for (x = xmin; x <= xmax; x++)
            {
                for (y = ymin; y <= ymax; y++)
                {
                    for (z = zmin; z <= zmax; z++)
                    {
                        blockLocations[x, y, z] = true;
                    }
                }
            }
        }

        // Now count them all
        int count = 0;
        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                for(int k = 0; k < zSize; k++)
                {
                    if(blockLocations[i, j, k] == true)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
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

    // When the user hits the chunk border button
    public void ToggleChunkBorders()
    {
        if(chunkBordersShown)
        {
            foreach (int chunkID in allSeenChunks.Keys)
            {
                allSeenChunks[chunkID].GetComponent<Chunk>().DeactivateChunkBorders();
            }
        }
        else
        {
            foreach (int chunkID in allSeenChunks.Keys)
            {
                allSeenChunks[chunkID].GetComponent<Chunk>().ActivateChunkBorders();
            }
        }
        chunkBordersShown = !chunkBordersShown;
    }

    public void ExplodePlayer(Vector3 location)
    {
        
    }

    public void DestroyAll()
    {
        foreach(int chunkID in allSeenChunks.Keys)
        {
            allSeenChunks[chunkID].GetComponent<Chunk>().DestroyAll();
        }
    }
}

