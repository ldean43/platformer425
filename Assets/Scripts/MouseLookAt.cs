using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAt : MonoBehaviour
{
	private float mx, my;
	private float shiftTrans = 1.0f;

	public float Sensitivity = 80.0f;
	public float MoveSensitivity = 0.1f;
	public float ShiftMul = 3.0f;
	public float ctrlTrans = 1.0f / 3.0f;

	public float max_mouse = 20.0f;

	void Awake () {
		Vector3 euler = transform.rotation.eulerAngles;
		mx = euler.x;
		my = euler.y;
		shiftTrans = ShiftMul * MoveSensitivity;
	}

	void Update () {
		float x = Input.GetAxis( "Mouse X" );
		float y = Input.GetAxis( "Mouse Y" );
		mx += ( Mathf.Abs( x ) > max_mouse ? 0 : x ) * ( Sensitivity * Time.deltaTime );
		my -= ( Mathf.Abs( y ) > max_mouse ? 0 : y ) * ( Sensitivity * Time.deltaTime );

		transform.rotation = Quaternion.Euler( my, mx, 0.0f );

		float moveMul = Input.GetKey( "left shift" ) ? shiftTrans : MoveSensitivity;
		moveMul *= Input.GetKey( "left ctrl" ) ? ctrlTrans : 1.0f;

		transform.position += transform.forward * ( Input.GetKey( "w" ) ? moveMul : 0 );
		transform.position -= transform.forward * ( Input.GetKey( "s" ) ? moveMul : 0 );
		transform.position += transform.right * ( Input.GetKey( "d" ) ? moveMul : 0 );
		transform.position -= transform.right * ( Input.GetKey( "a" ) ? moveMul : 0 );
		transform.position += transform.up * ( Input.GetKey( "e" ) ? moveMul : 0 );
		transform.position -= transform.up * ( Input.GetKey( "q" ) ? moveMul : 0 );
	}
}
