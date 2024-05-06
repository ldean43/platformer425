using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {

	public GameObject player;

	public GameObject player_camera_obj;
	SphereCollider play_col;
	Rigidbody play_rb;

	public GameObject oth_port;
	PortalScript oth_port_script;

	public float reactivation_buffer_distance = 1;
	protected bool canActivate = false;

// 	controls growing. Also effectively a cooldown before the next portal use.
	public float growTime = 1;
	float grow_keyframe = 0;
	bool isGrowing = false;
	Vector3 this_scale;

	Vector3 othScl;

	public void HandleOthPort () {
		if ( oth_port != null )
			oth_port_script = oth_port.GetComponent<PortalScript>();
	}

    // Start is called before the first frame update
	void Start () {
		play_col = player_camera_obj.GetComponent<SphereCollider>();
		play_rb = player.GetComponent<Rigidbody>();

		HandleOthPort();

		reactivation_buffer_distance *= reactivation_buffer_distance;
		reactivation_buffer_distance += play_col.radius * play_col.radius;

		this_scale = gameObject.transform.localScale;
		this.growPortal();
    }

    void Update () {
		canActivate = canActivate || ( oth_port != null && !isGrowing && ( player_camera_obj.transform.position -
				gameObject.transform.position ).sqrMagnitude > reactivation_buffer_distance );
	}

    void OnTriggerEnter ( Collider oth ) {
		if ( canActivate /* some other cond can go here, ex name of gameobj */ ) {
			// we rotate
			Quaternion diffRot = Quaternion.Inverse( gameObject.transform.rotation ) * oth_port.transform.rotation;
			// fix both camera and player cause they're not the same.
			player_camera_obj.transform.rotation *= diffRot;
			player.transform.rotation = player_camera_obj.transform.rotation;
			// fix velocity problems
			play_rb.velocity = diffRot * play_rb.velocity;

			// we rotate player pos around the port
			Vector3 temp = player.transform.position - gameObject.transform.position;
			temp = diffRot * temp;

			// we move player (assume camera will follow)
			player.transform.position = temp + oth_port.transform.position +
					player.transform.forward * play_col.radius * 2;

			// this is to ensure the portals don't just keep continually activating
			oth_port_script.canActivate = false;

			this.growPortal();
			oth_port_script.growPortal();
		}
	}

	public void growPortal () {
		gameObject.transform.localScale = Vector3.zero;
		grow_keyframe = 0;
		isGrowing = true;

		StartCoroutine( PortalGrow() );
	}

	public void ShrinkAndDestroy () {
		isGrowing = true;
		StartCoroutine( PortalShrink() );
	}

	float interpFn ( float q ) {
		return Mathf.Atan( Mathf.Tan( 1 ) * q );
	}

	IEnumerator PortalGrow () {
		while ( grow_keyframe < 1 ) {
			grow_keyframe += Time.deltaTime / growTime;
			gameObject.transform.localScale = this_scale * grow_keyframe;
			yield return new WaitForEndOfFrame();
		}
		gameObject.transform.localScale = this_scale;
		isGrowing = false;
		grow_keyframe = 1;
		yield return null;
	}

	IEnumerator PortalShrink () {
		while ( grow_keyframe > 0 ) {
			grow_keyframe -= Time.deltaTime / growTime;
			gameObject.transform.localScale = this_scale * grow_keyframe;
			yield return new WaitForEndOfFrame();
		}
		gameObject.transform.localScale = Vector3.zero;
		Destroy( this.gameObject );
		yield return null;
	}
}
