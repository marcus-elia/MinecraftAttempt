using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        resultText.text = WinLoseStatus.resultString;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
