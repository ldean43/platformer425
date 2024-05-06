using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsChanger : MonoBehaviour
{

    public Toggle AutoBHop;
    public Slider FOV;
    public Slider SloMoLength;

    public TextMeshProUGUI FOVtext;
    public TextMeshProUGUI sloMotext;

    void Start()
    {
        FOV.value = GameSettings.fov;
        SloMoLength.value = GameSettings.sloMoLength;
        AutoBHop.isOn = GameSettings.autoBhopControl;
    }


    public void setFOV()
    {
        GameSettings.fov = FOV.value;
        FOVtext.text = FOV.value.ToString();
    }

    public void setSloMoLength()
    {
        GameSettings.sloMoLength = SloMoLength.value;
        sloMotext.text = SloMoLength.value.ToString();
    }

    public void setAutoBHop()
    {
        
        GameSettings.autoBhopControl = AutoBHop.isOn;
    }
}

public static class GameSettings
{
    public static float sloMoLength = 2;
    public static bool autoBhopControl = true;
    public static float fov = 80;

}
