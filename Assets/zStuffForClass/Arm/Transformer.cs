using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour {
	public Vector3 translationRate;
	public Vector3 rotationRate;
	public string key;

    void Update () {
		if ( Input.GetKey( key ) ) {
			int lshft = Input.GetKey( "left shift" ) ? -1 : 1;

			Vector3 appliedRotation = rotationRate * Time.deltaTime;
			Vector3 appliedTranslation = translationRate * Time.deltaTime;

			appliedRotation *= lshft;
			appliedTranslation *= lshft;

			gameObject.transform.localRotation *= Quaternion.Euler( appliedRotation );
			gameObject.transform.localPosition += appliedTranslation;
		}
    }
}
