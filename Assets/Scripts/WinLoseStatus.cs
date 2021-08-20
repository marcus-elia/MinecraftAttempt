using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLoseStatus : MonoBehaviour
{
    public static int numBlocks;
    public static bool correctAnswer;
    public static string resultString = "";

    public TextMeshProUGUI guessText;
    public GameObject chunkManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Guess(string guessString)
    {
        if(guessText.text.StartsWith(WinLoseStatus.numBlocks.ToString()))
        {
            WinLoseStatus.correctAnswer = true;
            WinLoseStatus.resultString = "You Won! That's right! There are " + WinLoseStatus.numBlocks.ToString() + " blocks.";
        }
        else
        {
            WinLoseStatus.correctAnswer = false;
            WinLoseStatus.resultString = "You lost. There were not " + guessText.text + " blocks.";
        }
        chunkManager.GetComponent<ChunkManager>().DestroyAll();
        SceneManager.LoadScene("StartMenu");
    }
}
