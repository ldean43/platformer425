using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {

	public GameObject player;
	SphereCollider play_col;
	Rigidbody play_rb;

	public GameObject oth_port;
	Vector3 portalDiff;
	Quaternion portalRot;
	PortalScript oth_port_script;

	public float reactivation_distance = 1;
	protected bool canActivate = false;

    // Start is called before the first frame update
	void Start () {
		play_col = player.GetComponent<SphereCollider>();
		play_rb = player.GetComponent<Rigidbody>();

		portalDiff = oth_port.transform.position - gameObject.transform.position;
		portalRot = Quaternion.Inverse( gameObject.transform.rotation ) * oth_port.transform.rotation;

		oth_port_script = oth_port.GetComponent<PortalScript>();

		reactivation_distance *= reactivation_distance;
    }

    void Update () {
		canActivate = canActivate || ( player.transform.position -
				gameObject.transform.position ).sqrMagnitude > reactivation_distance;
	}

    void OnTriggerEnter ( Collider oth ) {
		Debug.Log( "hi" );
		if ( canActivate /* some other cond can go here, ex name of gameobj */ ) {
			oth.transform.position += portalDiff + oth_port.transform.forward *
					play_col.radius * Vector3.Dot( oth_port.transform.forward, player.transform.forward );
			oth.transform.rotation *= portalRot;
			// this is to ensure the portals don't just keep continually activating
			oth_port_script.canActivate = false;
		}
	}
}
