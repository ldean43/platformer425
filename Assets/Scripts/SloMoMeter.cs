using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SloMoMeter : MonoBehaviour {
    private RectTransform meter;
    private PlayerController plr;
    private float increment;

    [Header("References")]
    public GameObject player;

    // Start is called before the first frame update
    void Start() {
        meter = gameObject.GetComponent<RectTransform>();
        plr = player.GetComponent<PlayerController>();
        increment = meter.rect.width/2;
    }

    // Update is called once per frame
    void Update() {
        meter.sizeDelta = new Vector2(increment * plr.timer, meter.rect.height);
    }
}
