using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Player")]
    public PlayerController player;

    [Header("Body Parts")]
    public GameObject head;
    public GameObject torso;
    public GameObject rArmUpper;
    public GameObject rArmLower;
    public GameObject lArmUpper;
    public GameObject lArmLower;
    public GameObject rLegUpper;
    public GameObject rLegLower;
    public GameObject lLegUpper;
    public GameObject lLegLower;

    [Header("Animation Rate Settings")]
    public float minSpeed;
    public float baseRate;
    public float speedMultiplier;

    private float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float animSpeed = baseRate + speedMultiplier * player.speed;
        if (player.speed > minSpeed)
        {
            time += animSpeed * Time.deltaTime;
            rArmUpper.transform.localRotation = Quaternion.Euler(60 * Mathf.Sin(time), 0, 0);
            rArmLower.transform.localRotation = Quaternion.Euler(-10 - 20 * (1 - Mathf.Sin(time)), 0, 0);
            lArmUpper.transform.localRotation = Quaternion.Euler(-60 * Mathf.Sin(time), 0, 0);
            lArmLower.transform.localRotation = Quaternion.Euler(-10 - 20 * (1 + Mathf.Sin(time)), 0, 0);
            rLegUpper.transform.localRotation = Quaternion.Euler(-60 * Mathf.Sin(time), 0, 0);
            rLegLower.transform.localRotation = Quaternion.Euler(35 * (1 - Mathf.Sin(time)), 0, 0);
            lLegUpper.transform.localRotation = Quaternion.Euler(60 * Mathf.Sin(time), 0, 0);
            lLegLower.transform.localRotation = Quaternion.Euler(35 * (1 + Mathf.Sin(time)), 0, 0);
        } else
        {
            time = 0.0f;
            rArmUpper.transform.localRotation = Quaternion.Euler(0, 0, 0);
            rArmLower.transform.localRotation = Quaternion.Euler(-10, 0, 0);
            lArmUpper.transform.localRotation = Quaternion.Euler(0, 0, 0);
            lArmLower.transform.localRotation = Quaternion.Euler(-10, 0, 0);
            rLegUpper.transform.localRotation = Quaternion.Euler(0, 0, 0);
            rLegLower.transform.localRotation = Quaternion.Euler(0, 0, 0);
            lLegUpper.transform.localRotation = Quaternion.Euler(0, 0, 0);
            lLegLower.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        head.transform.rotation = Quaternion.Euler(player.xRot, player.yRot, 0);
        Vector2 planeV = new Vector2(player.rb.velocity.x, player.rb.velocity.z);
        float torsoY = planeV.magnitude > 0.01 ? Quaternion.LookRotation(player.rb.velocity).eulerAngles.y : player.yRot;
        torso.transform.rotation = Quaternion.Euler(0, torsoY, 0);
    }
}
