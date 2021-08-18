using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BuildingGenerationMode { All, Small, None, Terrain };

public class StartMenuHandler : MonoBehaviour
{
    public static BuildingGenerationMode buildingMode = BuildingGenerationMode.All;
    public Dropdown dropdown;
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        instructionsPanel.SetActive(false);

        // Add listener for when the value of the Dropdown changes, to take action
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropdownValueChanged(Dropdown change)
    {
        switch(change.value)
        {
            case 0:
                buildingMode = BuildingGenerationMode.All;
                break;
            case 2:
                buildingMode = BuildingGenerationMode.Small;
                break;
            case 1:
                buildingMode = BuildingGenerationMode.None;
                break;
            default:
                buildingMode = BuildingGenerationMode.Terrain;
                break;
        }
    }

    public void SwitchToInstructions()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }
    public void SwitchToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
