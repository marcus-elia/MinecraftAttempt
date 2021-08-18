using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType { Grass, Stone, Wood, Limestone, Darkglass, Limestonefence };


public class Inventory : MonoBehaviour
{
    private static Dictionary<BlockType, int> numBlocks = new Dictionary<BlockType, int>();
    public static int numBlockTypes;

    public GameObject grassIcon, stoneIcon, woodIcon, limestoneIcon, darkglassIcon, limestonefenceIcon;
    public Texture grassTex, grassTexH, stoneTex, stoneTexH, woodTex, woodTexH, limestoneTex, limestoneTexH,
        darkglassTex, darkglassTexH, limestonefenceTex, limestonefenceTexH;

    public static Dictionary<BlockType, TexturePair> texDict = new Dictionary<BlockType, TexturePair>();
    public static Dictionary<BlockType, GameObject> imageDict = new Dictionary<BlockType, GameObject>();
    public static Dictionary<BlockType, TextMeshProUGUI> countTextDict = new Dictionary<BlockType, TextMeshProUGUI>();


    // What the player is selecting
    public static BlockType selectedBlockType = BlockType.Grass;

    // The text counting each block type
    public TextMeshProUGUI grassCountText, stoneCountText, woodCountText, limestoneCountText, darkglassCountText, limestonefenceCountText;

    // Start is called before the first frame update
    void Start()
    {
        // Make the dictionary of textures
        texDict[BlockType.Grass] = new TexturePair(grassTex, grassTexH);
        texDict[BlockType.Stone] = new TexturePair(stoneTex, stoneTexH);
        texDict[BlockType.Wood] = new TexturePair(woodTex, woodTexH);
        texDict[BlockType.Limestone] = new TexturePair(limestoneTex, limestoneTexH);
        texDict[BlockType.Darkglass] = new TexturePair(darkglassTex, darkglassTexH);
        texDict[BlockType.Limestonefence] = new TexturePair(limestonefenceTex, limestonefenceTexH);

        // And map to the game objects of the images themselves
        imageDict[BlockType.Grass] = grassIcon;
        imageDict[BlockType.Stone] = stoneIcon;
        imageDict[BlockType.Wood] = woodIcon;
        imageDict[BlockType.Limestone] = limestoneIcon;
        imageDict[BlockType.Darkglass] = darkglassIcon;
        imageDict[BlockType.Limestonefence] = limestonefenceIcon;

        // And map to the UI text
        countTextDict[BlockType.Grass] = grassCountText;
        countTextDict[BlockType.Stone] = stoneCountText;
        countTextDict[BlockType.Wood] = woodCountText;
        countTextDict[BlockType.Limestone] = limestoneCountText;
        countTextDict[BlockType.Darkglass] = darkglassCountText;
        countTextDict[BlockType.Limestonefence] = limestonefenceCountText;

        // Set the textures of the icons in the UI and initialize the counts (how many
        // of each block the player has) to 0.
        foreach (BlockType bType in System.Enum.GetValues(typeof(BlockType)))
        {
            numBlocks[bType] = 0;
            imageDict[bType].GetComponent<RawImage>().texture = texDict[bType].tex;
        }
        numBlockTypes = System.Enum.GetValues(typeof(BlockType)).Length;

        // Grass is highlighted in the UI to start
        grassIcon.GetComponent<RawImage>().texture = grassTexH;
    }

    // Update is called once per frame
    void Update()
    {
        imageDict[selectedBlockType].GetComponent<RawImage>().texture = texDict[selectedBlockType].tex;

        // When the user scrolls the mouse wheel, update which block is selected.
        selectedBlockType = (BlockType)(Mod((int)selectedBlockType + (int)Input.mouseScrollDelta.y, numBlockTypes));
        imageDict[selectedBlockType].GetComponent<RawImage>().texture = texDict[selectedBlockType].highlight;
    }

    public static bool PlaceBlock()
    {
        if(numBlocks[selectedBlockType] == 0)
        {
            return false;
        }
        else
        {
            numBlocks[selectedBlockType] -= 1;
            countTextDict[selectedBlockType].text = numBlocks[selectedBlockType].ToString();
            return true;
        }
    }

    public static void BreakBlock(BlockType blockType)
    {
        numBlocks[blockType] += 1;
        countTextDict[blockType].text = numBlocks[blockType].ToString();
    }

    public static int Mod(int a, int m)
    {
        int b = a % m;
        if(b < 0)
        {
            b += m;
        }
        return b;
    }
}
