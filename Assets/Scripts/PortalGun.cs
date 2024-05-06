using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    private int animDir = -1;

    [Header("Configurations")]
    public GameObject chargeOverlay;
    public float charge;
    public bool inAnimation;
    public bool canFire;
    public float rechargeTime = 0.4f; // time to recharge
    public float fireAnimTime = 0.5f; // total time for the animation to process
    public float recoilPortion = 0.2f; // proportion of animation time spent going backwards
    public float minSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // TODO set up text to appear on the screen
        //rechargeTime = 0.4f;
        //fireAnimTime = 0.5f;
        //recoilPortion = 0.2f;
        charge = 1.0f;
        inAnimation = false;
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (charge < 1.0f || inAnimation)
        {
            charge += Time.deltaTime / rechargeTime;
            if (charge >= 1.0f)
            {
                charge = 1.0f;
            }
            chargeOverlay.transform.localScale = new Vector3(1.01f, Mathf.Floor(10 * charge) / 10, 1.01f);
            if (inAnimation)
            {
                Vector3 pos = transform.localPosition;
                if (animDir < 0)
                {
                    pos.y -= Time.deltaTime * 0.1f / (recoilPortion * fireAnimTime);
                    if (pos.y <= -0.1f)
                    {
                        animDir = 1;
                        pos.y = -0.1f;
                    }
                }
                else
                {
                    pos.y += Time.deltaTime * 0.1f / ((1-recoilPortion) * fireAnimTime);
                    if (pos.y >= 0.0f)
                    {
                        animDir = -1;
                        pos.y = 0f;
                        inAnimation = false;
                    }
                }
                transform.localPosition = pos;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse0) && canFire)
            {
                charge = 0.0f;
                inAnimation = true;
                // TODO do whatever else we need to do
            }
        }
    }
}
