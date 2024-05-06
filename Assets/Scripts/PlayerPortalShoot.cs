using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortalShoot : MonoBehaviour {

	// One and two are just in case we want two different
	// kinds of portals (ex. a red and blue one)
	public GameObject portalPrefab1;
	public GameObject portalPrefab2;

	public float toleranceFactor = 1.0f;

	GameObject portalInstance1;
	GameObject portalInstance2;

	public GameObject player;
	public GameObject playerCam;

	int layer_no;
	public string[] layersToDisable;

	public bool disable;

	void Start () {
		layer_no = LayerMask.NameToLayer( "PortalLayer1" );
		layer_no |= LayerMask.GetMask( layersToDisable );
	}

    static void DrawDebugCube ( Vector3 center, Vector3 scale, Quaternion norm ) {
		GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
		cube.transform.position = center;
		cube.transform.localScale = scale;
		cube.transform.rotation = norm;
		cube.GetComponent<MeshRenderer>().material.color = Color.green;
	}

    GameObject SpawnPortal ( GameObject pfb ) {
		Debug.Log( "PortalShoot: in SpawnPortal( " + pfb.ToString() + " )" );
		BoxCollider pfbc = pfb.GetComponent<BoxCollider>();
		Vector3 halfScale = pfb.transform.localScale / 2;
		halfScale.z = pfbc.size.z;
		Vector3 tol = halfScale * toleranceFactor;
		RaycastHit hit;

		// cast a raycast and make sure it hits something
		if ( ! Physics.Raycast( gameObject.transform.position,
				playerCam.transform.forward, out hit,
				Mathf.Infinity, Physics.DefaultRaycastLayers,
				QueryTriggerInteraction.Collide ) ) {
			Debug.Log( "PortalShoot: SpawnPortal: first raycast failed" );
			return null;
		}

		// no portals on top of portals
		if ( ( hit.collider.gameObject.layer | layer_no ) != 0 ) {
			Debug.Log( "PortalShoot: SpawnPortal: portal overlap detected" );
			return null;
		}

		// make sure the collider can fit, aka nothing is in the box
		Quaternion norm = Quaternion.LookRotation( hit.normal );
		Vector3 orig = hit.point + ( halfScale.z + tol.z ) * hit.normal;
		if ( Physics.CheckBox( orig, halfScale, norm, Physics.DefaultRaycastLayers,
				QueryTriggerInteraction.Collide ) ) {
			Debug.Log( "PortalShoot: SpawnPortal: checkbox failed" );
			// comment this out when necessary
// 			DrawDebugCube( orig, halfScale * 2, norm );
			return null;
		}

		// make sure that the portal edges align with the edges of
		// the surface the portal is on. These raycasts should be
		// cheaper since they are only checking the one collider.
		Vector3 v1 = new Vector3(  halfScale.x + tol.x,	 halfScale.y + tol.y,
								   halfScale.z );
		Vector3 v2 = new Vector3(  halfScale.x + tol.x, -halfScale.y - tol.y,
								   halfScale.z );
		Vector3 v3 = new Vector3( -halfScale.x - tol.x,  halfScale.y + tol.y,
								   halfScale.z );
		Vector3 v4 = new Vector3( -halfScale.x - tol.x, -halfScale.y - tol.y,
								   halfScale.z );

		v1 = ( norm * v1 ) / 2;
		v2 = ( norm * v2 ) / 2;
		v3 = ( norm * v3 ) / 2;
		v4 = ( norm * v4 ) / 2;

		Ray r1 = new Ray( orig + v1, -hit.normal );
		Ray r2 = new Ray( orig + v2, -hit.normal );
		Ray r3 = new Ray( orig + v3, -hit.normal );
		Ray r4 = new Ray( orig + v4, -hit.normal );

		RaycastHit stub;
		if ( !( hit.collider.Raycast( r1, out stub, halfScale.z * 4 ) &&
				hit.collider.Raycast( r2, out stub, halfScale.z * 4 ) &&
				hit.collider.Raycast( r3, out stub, halfScale.z * 4 ) &&
				hit.collider.Raycast( r4, out stub, halfScale.z * 4 ) ) ) {
			Debug.Log( "PortalShoot: SpawnPortal: multi raycast failed" );
			// comment this out when necessary
// 			Debug.DrawRay( r1.origin, r1.direction, Color.green, Mathf.Infinity );
// 			Debug.DrawRay( r2.origin, r2.direction, Color.green, Mathf.Infinity );
// 			Debug.DrawRay( r3.origin, r3.direction, Color.green, Mathf.Infinity );
// 			Debug.DrawRay( r4.origin, r4.direction, Color.green, Mathf.Infinity );
// 			if ( Physics.Raycast( r1, out stub, halfScale.z * 4 ) ) {
// 				Debug.Log( stub.collider );
// 				Debug.Log( stub.collider == hit.collider );
// 			}
// 			if ( Physics.Raycast( r2, out stub, halfScale.z * 4 ) ) {
// 				Debug.Log( stub.collider );
// 				Debug.Log( stub.collider == hit.collider );
// 			}
// 			if ( Physics.Raycast( r3, out stub, halfScale.z * 4 ) ) {
// 				Debug.Log( stub.collider );
// 				Debug.Log( stub.collider == hit.collider );
// 			}
// 			if ( Physics.Raycast( r4, out stub, halfScale.z * 4 ) ) {
// 				Debug.Log( stub.collider );
// 				Debug.Log( stub.collider == hit.collider );
// 			}
			return null;
		}

		// now that all tests have been passed, instantiate the prefab
		GameObject portal = Object.Instantiate( pfb, orig, norm );
		portal.name = pfb.name + " (instance)";
		Debug.Log( "PortalShoot: SpawnPortal: created portal at " + orig.ToString() );

		// assign the correct values to the obj's PortalScript and
		// PortalCamScript
		PortalScript portalScript = portal.GetComponent<PortalScript>();
		PortalCamScript portalCamScript = portal.GetComponent
				<PortalCamScript>();

		portalScript.player = player;
		portalScript.player_camera_obj = playerCam;
		portalScript.oth_port = null;

		portalCamScript.player = player;
		portalCamScript.player_cam_obj = playerCam;
		portalCamScript.oth_port = null;

		return portal;
	}

	void UpdateOthPortal ( GameObject portal, GameObject other ) {
		if ( portal == null ) {
			Debug.Log( "PortalShoot: in UpdateOthPortal: portal == null" );
			return;
		}

		Debug.Log( "PortalShoot: in UpdateOthPortal( " + portal.ToString() +
				", " + ( other == null ? "(null)" : other.ToString() ) + " )" );

		PortalScript portalScript = portal.GetComponent<PortalScript>();
		PortalCamScript portalCamScript = portal.GetComponent
				<PortalCamScript>();

		if ( other != null ) {
			portalScript.oth_port = other;
			portalScript.HandleOthPort();
			portalCamScript.oth_port = other;
			portalCamScript.HandleOthPort();

			portalCamScript.FadeInView();
		} else {
			portalCamScript.FadeOutView();
		}
	}

	IEnumerator UpdatePortals () {
		yield return new WaitForEndOfFrame();
		UpdateOthPortal( portalInstance1, portalInstance2 );
		UpdateOthPortal( portalInstance2, portalInstance1 );
		yield return null;
	}

    void Update () {
        if ( Input.GetKeyDown( "mouse 0" ) && ( !disable ) ) {
			Debug.Log( "PortalShoot: lmb pressed" );
			if ( portalInstance1 != null ) {
				Debug.Log( "PortalShoot: Destroying portalInstance1" );
				portalInstance1.GetComponent<PortalScript>().ShrinkAndDestroy();
				portalInstance1 = null;
			}
			Debug.Log( "PortalShoot: Spawning portalInstance1" );
			portalInstance1 = SpawnPortal( portalPrefab1 );
			StartCoroutine( UpdatePortals() );
		}

		if ( Input.GetKeyDown( "mouse 1" ) && ( !disable ) ) {
			Debug.Log( "PortalShoot: rmb pressed" );
			if ( portalInstance2 != null ) {
				Debug.Log( "PortalShoot: Destroying portalInstance2" );
				portalInstance2.GetComponent<PortalScript>().ShrinkAndDestroy();
				portalInstance2 = null;
			}
			Debug.Log( "PortalShoot: Spawning portalInstance2" );
			portalInstance2 = SpawnPortal( portalPrefab2 );
			StartCoroutine( UpdatePortals() );
		}
    }
}
