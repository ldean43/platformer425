using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignCameraWithPlayer : MonoBehaviour {

	public int axis_inv = 1;
    public Camera cam, player;

	void Start () {
        
    }

    void Update () {
		Vector3 diff = cam.gameObject.transform.position;
		diff -= player.gameObject.transform.position;
		diff *= axis_inv;
		Vector3 up = Vector3.up;
		cam.gameObject.transform.rotation = Quaternion.LookRotation( diff, up );
    }
}
