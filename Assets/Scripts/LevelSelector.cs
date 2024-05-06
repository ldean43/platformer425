using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown levelsDropdown;
    private List<string> options; 
    private int levelIndexToLoad;
    void Start()
    {

        levelsDropdown.ClearOptions();
        options = new List<string>
        {
            "Game",
            "portaltesting",
            "shadertesting",
            "SampleScene"
        };
        levelsDropdown.AddOptions(options);
        levelsDropdown.value = 0;
        levelIndexToLoad = 0;

    }

    public void UpdateLevelToLoad(int levelIndex)
    {
        levelIndexToLoad = levelIndex;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(options[levelIndexToLoad]);
    }

}
