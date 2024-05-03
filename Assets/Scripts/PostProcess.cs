using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From https://lindenreidblog.com/2018/02/05/camera-shaders-unity/
// Basically applies the post process shader to the camera.
[ExecuteInEditMode]
public class PostProcess : MonoBehaviour {

	public Material material;

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, material);
	}
}
