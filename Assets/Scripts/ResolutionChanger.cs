using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionChanger : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    public Toggle fullscreen;

    public static bool isFullscreen = true;

    private Resolution[] resolutions;
    private List<Resolution> fitleredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        isFullscreen = Screen.fullScreen;
        fullscreen.isOn = isFullscreen;

        resolutions = Screen.resolutions;
        fitleredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                fitleredResolutions.Add(resolutions[i]);
            }
        }
        
        List<string> options = new List<string>();
        for(int i = 0; i < fitleredResolutions.Count; i++)
        {
            string resolutionOption = fitleredResolutions[i].width + "x" + fitleredResolutions[i].height;
            options.Add(resolutionOption);
            if (fitleredResolutions[i].width == Screen.width && fitleredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        } 

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = fitleredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen.isOn);
    }


    // Update is called once per frame
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        isFullscreen = Screen.fullScreen;
    }
}
