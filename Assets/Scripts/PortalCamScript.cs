using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamScript : MonoBehaviour {

	public GameObject player;

	public GameObject player_cam_obj;
	SphereCollider play_col;
	Camera play_cam;

	public GameObject oth_port;
	Camera port_cam;

	RenderTexture rend_tex;
	MeshRenderer mesh_front, mesh_back;

	public bool disableView = false;

	public float defaultOpacity = 0.3f;

	float fade_keyframe = 0;
	public float fadeTime = 1;
	IEnumerator fadeIn;
	IEnumerator fadeOut;

	MaterialPropertyBlock matBlock;

	public void HandleOthPort () {
		if ( oth_port != null ) {
			port_cam = oth_port.transform.GetChild( 2 ).gameObject.GetComponent<Camera>();
			port_cam.projectionMatrix = play_cam.projectionMatrix;
			port_cam.targetTexture = rend_tex;
		} else {
			OpacityChange( 1 );
		}
	}

	private void CreateRenderTex () {
		rend_tex = new RenderTexture( Screen.width, Screen.height, 1 );

		mesh_front.material.SetTexture( "_MainTex", rend_tex );
		mesh_back.material.SetTexture( "_MainTex", rend_tex );

		if ( oth_port != null )
			port_cam.targetTexture = rend_tex;
	}

	// Start is called before the first frame update
	void Start () {
		matBlock = new MaterialPropertyBlock();

		play_col = player_cam_obj.GetComponent<SphereCollider>();

		mesh_front = gameObject.transform.GetChild( 0 ).gameObject.GetComponent<MeshRenderer>();
		mesh_back = gameObject.transform.GetChild( 1 ).gameObject.GetComponent<MeshRenderer>();

		play_cam = player_cam_obj.GetComponent<Camera>();

		CreateRenderTex();

		HandleOthPort();
    }

    void OpacityChange ( float keyframe ) {

		mesh_front.GetPropertyBlock( matBlock );
		matBlock.SetFloat( "_ColorOpacity", defaultOpacity * ( 1 - keyframe ) + keyframe );
		mesh_front.SetPropertyBlock( matBlock );

		mesh_back.GetPropertyBlock( matBlock );
		matBlock.SetFloat( "_ColorOpacity", defaultOpacity * ( 1 - keyframe ) + keyframe );
		mesh_back.SetPropertyBlock( matBlock );
	}

	public void FadeInView () {
		fadeIn = PortalFadeIn();
		disableView = false;
		StartCoroutine( fadeIn );
		if ( fadeOut != null )
			StopCoroutine( fadeOut );
	}

	public void FadeOutView () {
		fadeOut = PortalFadeOut();
		StartCoroutine( fadeOut );
		if ( fadeIn != null )
			StopCoroutine( fadeIn );
	}

	IEnumerator PortalFadeOut () {
		while ( fade_keyframe < 1 ) {
			fade_keyframe += Time.deltaTime / fadeTime;
			OpacityChange( fade_keyframe );
			yield return new WaitForEndOfFrame();
		}
		OpacityChange( 1 );
		fade_keyframe = 1;
		disableView = true;
		yield return null;
	}

	IEnumerator PortalFadeIn () {
		while ( fade_keyframe > 0 ) {
			fade_keyframe -= Time.deltaTime / fadeTime;
			OpacityChange( fade_keyframe );
			yield return new WaitForEndOfFrame();
		}
		OpacityChange( 0 );
		fade_keyframe = 0;
		yield return null;
	}

//     private void OnRectTransformDimensionsChange () {
// 		CreateRenderTex();
// 	}

//     TODO: OPTIMIZE THIS
    void Update () {
		// pointless if no portal
		if ( oth_port != null && ( !disableView ) ) {
			// we rotate
			Quaternion diffRot = Quaternion.Inverse( gameObject.transform.rotation ) * oth_port.transform.rotation;
			port_cam.transform.rotation = player_cam_obj.transform.rotation * diffRot;

			// remove roll
			port_cam.transform.rotation = Quaternion.Euler( port_cam.transform.rotation.eulerAngles.x,
															port_cam.transform.rotation.eulerAngles.y, 0 );

			// we rotate player (camera) pos around the port
			Vector3 temp = player.transform.position - gameObject.transform.position;
			temp = diffRot * temp;

			// we move player
			port_cam.transform.position = temp + oth_port.transform.position +
					port_cam.transform.forward * play_col.radius * 2;
		}
    }
}
