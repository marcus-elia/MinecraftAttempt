using System.Collections;
using System.Collections.Generic;
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

public class ChunkManager : MonoBehaviour
{
    private int currentPlayerChunkID;
    public int renderRadius = 2;
    private Dictionary<int, GameObject> allSeenChunks;
    private List<GameObject> currentChunks;

    private int lookedAtChunkID = 0;

    public Texture tex;
    public Texture highlightedTex;

    public GameObject chunkBorderPrefab;

    // Keep track of where the player is to decide which Chunks
    // to be active
    public Transform playerTransform;
    public float playerHeight;

    // Need the camera for raycasting
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerChunkID = getChunkIDContainingPoint(playerTransform.position, Chunk.blocksPerSide);
        allSeenChunks = new Dictionary<int, GameObject>();
        currentChunks = new List<GameObject>();
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
        // Disable previous chunks from drawing, and remove them from the arraylist
        for (int i = 0; i < currentChunks.Count; i++)
        {
            currentChunks[i].GetComponent<Chunk>().DisableChunk();
        }
        currentChunks = new List<GameObject>();

        // Go through the new chunks and add/create them
        List<int> chunkIDs = getChunkIDsAroundID(currentPlayerChunkID, renderRadius);
        for (int i = 0; i < chunkIDs.Count; i++)
        {
            int id = (chunkIDs[i]);
            if (allSeenChunks.ContainsKey(id))
            {
                currentChunks.Add(allSeenChunks[id]);
                allSeenChunks[id].GetComponent<Chunk>().EnableChunk();
            }
            else
            {
                // Make the new Chunk
                GameObject c = new GameObject();
                c.AddComponent<Chunk>();
                c.GetComponent<Chunk>().SetChunkID(id);
                c.GetComponent<Chunk>().SetTexture(tex, highlightedTex);
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
                int westID = GetEastChunkID(id);
                if (allSeenChunks.ContainsKey(westID))
                {
                    c.GetComponent<Chunk>().SetWestNeighbor(allSeenChunks[westID].GetComponent<Chunk>());
                    allSeenChunks[westID].GetComponent<Chunk>().SetEastNeighbor(c.GetComponent<Chunk>());
                }

                c.GetComponent<Chunk>().EnableChunk();
                allSeenChunks.Add(id, c);
                currentChunks.Add(c);
                //Debug.Log("new chunk " + id + " has exposed faces: " + allSeenChunks[id].GetComponent<Chunk>().CountExposedFaces());
            }
        }
    }

    public void UpdateRenderRadius(int newRadius)
    {
        this.renderRadius = newRadius;
        this.updateChunks();
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
    public static List<int> getChunkIDsAroundPoint(Point2D p, int radius)
    {
        List<int> result = new List<int>();

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
    public static List<int> getChunkIDsAroundID(int id, int radius)
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

        if (Physics.Raycast(ray, out hit))
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
                    lookedAtChunk.ReactToRaycastHit(objectHit);
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
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (allSeenChunks.ContainsKey(lookedAtChunkID))
                {
                    Chunk lookedAtChunk = allSeenChunks[lookedAtChunkID].GetComponent<Chunk>();
                    if (lookedAtChunk.GetIsActive())
                    {
                        lookedAtChunk.ReactToRightClick(hit.transform);
                    }
                }
            }
        }
    }
}

