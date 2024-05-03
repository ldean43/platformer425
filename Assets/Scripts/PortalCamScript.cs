using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamScript : MonoBehaviour {

	public GameObject player;
	SphereCollider play_col;
	Camera play_cam;

	public GameObject oth_port;
	Camera port_cam;

	RenderTexture rend_tex;
	MeshRenderer mesh_front, mesh_back;
	Vector3 camera_offset;

// 	public int dimnX = 512, dimnY = 512;

	private void CreateRenderTex () {
		rend_tex = new RenderTexture( Screen.width, Screen.height, 1 );

		mesh_front.material.SetTexture( "_MainTex", rend_tex );
		mesh_back.material.SetTexture( "_MainTex", rend_tex );

		port_cam.targetTexture = rend_tex;
	}

	// Start is called before the first frame update
	void Start () {
		play_col = player.GetComponent<SphereCollider>();
		camera_offset = oth_port.transform.forward * play_col.radius;

		mesh_front = gameObject.transform.GetChild( 0 ).gameObject.GetComponent<MeshRenderer>();
		mesh_back = gameObject.transform.GetChild( 1 ).gameObject.GetComponent<MeshRenderer>();

		port_cam = oth_port.transform.GetChild( 2 ).gameObject.GetComponent<Camera>();
		play_cam = player.GetComponent<Camera>();

		CreateRenderTex();

		Matrix4x4 projMat = play_cam.projectionMatrix;
// 		float prevAspectRatio = projMat[ 1, 1 ] / projMat[ 0, 0 ];
// 		float newAspectRatio = gameObject.transform.localScale.x / gameObject.transform.localScale.y;
// 		projMat[ 0, 0 ] *= prevAspectRatio;
// 		projMat[ 1, 1 ] *= newAspectRatio;
		port_cam.projectionMatrix = projMat;
    }

    private void OnRectTransformDimensionsChange () {
		CreateRenderTex();
	}

    // Update is called once per frame
    void Update () {
		Vector3 diff = oth_port.transform.position - gameObject.transform.position +
				camera_offset * Vector3.Dot( oth_port.transform.forward, play_cam.transform.forward );
		Quaternion diffRot = Quaternion.Inverse( gameObject.transform.rotation ) * oth_port.transform.rotation;

		port_cam.transform.position = play_cam.transform.position + diff;
		port_cam.transform.rotation = play_cam.transform.rotation * diffRot;
    }
}
