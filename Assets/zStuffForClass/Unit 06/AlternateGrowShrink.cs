using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateGrowShrink : MonoBehaviour {
	Vector3[] base_verts;
	Vector3[] vts;
	Vector3[] normals;
	Mesh this_mesh;

	public float rate = 1;
	float prate = 0;

	bool pointIsInCircle ( int i ) {
		return i > 47;
	}

	// r1 = top radius, r2 is bottom radius
	void SetCylinder ( float r1, float r2 ) {
		for ( int i = 0; i < base_verts.Length; i++ ) {

			// set the top verts to be multiplied by r1 and the bottom by r2
			if ( base_verts[ i ].y > 0 ) {
				vts[ i ].x = base_verts[ i ].x * r1;
				vts[ i ].z = base_verts[ i ].z * r1;
			} else if ( base_verts[ i ].y < 0 ) {
				vts[ i ].x = base_verts[ i ].x * r2;
				vts[ i ].z = base_verts[ i ].z * r2;
			}
			vts[ i ].y = base_verts[ i ].y;

			// Normal calc
			if ( ( base_verts[ i ].x == 0 && base_verts[ i ].z == 0 ) ||
				( pointIsInCircle( i ) ) ) {
				// vertices pointing up and down
				normals[ i ] = new Vector3( 0, base_verts[ i ].y, 0 );
				} else {
					// vertices pointing towards the sides
					normals[ i ].x = base_verts[ i ].x;
					normals[ i ].z = base_verts[ i ].z;
					normals[ i ].y = ( r2 - r1 ) * r2 / 2;
					Vector3.Normalize( normals[ i ] );
				}
		}

		this_mesh.vertices = vts;
		this_mesh.normals = normals;
		prate = rate * 2 * Mathf.PI;
	}

	// Start is called before the first frame update
	void Start () {
		this_mesh = GetComponent<MeshFilter>().mesh;

		vts = new Vector3[ this_mesh.vertices.Length ];
		base_verts = new Vector3[ vts.Length ];
		normals = new Vector3[ vts.Length ];

		this_mesh.vertices.CopyTo( base_verts, 0 );
	}

	// Update is called once per frame
	void Update () {
		// we base our keyframe on a sinusoid.
		float keyframe = Mathf.Cos( Time.time * prate );
		float r1 =  1.0f - keyframe * 0.5f;
		float r2 =  1.0f + keyframe * 0.5f;
		this.SetCylinder( r1, r2 );
	}
}
