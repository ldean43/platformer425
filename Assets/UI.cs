using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private GameObject text, Player;
    private TextMeshProUGUI txt;
    private PlayerController plc;
    // Start is called before the first frame update
    void Start()
    {
        txt = text.GetComponent<TextMeshProUGUI>();
        plc = Player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = "SPEED: " + Mathf.Floor(plc.speed * 100)/100;
    }
}
