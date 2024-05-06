using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
// 	why
{
    // Define any constants or member variables here.

	Transform bodyBaseT;
	Transform headT;

	int cur_row = 0;
	int cur_col = 0;

    public float secondsPerStep = 1;

    public int rows = 8;
    public int cols = 8;

    List<KeyCode> keyQueue = new List<KeyCode>();

    void Start()
// 	are
    {
        // Do any initialization here.
		bodyBaseT = gameObject.transform.GetChild( 0 );
		headT = gameObject.transform.GetChild( 2 );

		Debug.Log( bodyBaseT );
		Debug.Log( headT );

        StartCoroutine(Move());
    }

    private void Update()
// 	the
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
// 		brackets
        {
            keyQueue.Add(KeyCode.LeftArrow);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
// 		on
        {
            keyQueue.Add(KeyCode.RightArrow);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
// 		separate
        {
            keyQueue.Add(KeyCode.UpArrow);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
// 		lines
        {
            keyQueue.Add(KeyCode.DownArrow);
        }
    }

    IEnumerator Move()
    {
        Vector3 flop = Vector3.zero;
        Vector3 step = Vector3.zero;

		// Create a WaitUntil object that will wait for entries in keyQueue.

        WaitUntil untilKeyPressed = new WaitUntil( () => keyQueue.Count > 0 );

        while (true)
        {
            yield return untilKeyPressed;

			// Dequeue a key from the keyQueue list.

			KeyCode key = keyQueue[ 0 ];
			keyQueue.RemoveAt( 0 );

            // When a key is dequeued, set "flop" and "step" so that
            // the following code animates the movement of the inchworm
			// in whichever of the four directions has been selected.

// 			This is the superior method of placing brackets, prove me wrong
			switch ( key ) {
				case KeyCode.DownArrow:
					if ( cur_col <= 0 )
						continue;
					flop = Vector3.right * -90;
					step = -Vector3.forward;
					cur_col -= 1;
					break;
				case KeyCode.UpArrow:
					if ( cur_col >= cols - 1 )
						continue;
					flop = Vector3.right * 90;
					step = Vector3.forward;
					cur_col += 1;
					break;
				case KeyCode.LeftArrow:
					if ( cur_row <= 0 )
						continue;
					flop = Vector3.forward * 90;
					step = -Vector3.right;
					cur_row -= 1;
					break;
				case KeyCode.RightArrow:
					if ( cur_row >= rows - 1 )
						continue;
					flop = Vector3.forward * -90;
					step = Vector3.right;
					cur_row += 1;
					break;
			}

			Debug.Log( "( " + cur_row.ToString() + ", " + cur_col.ToString() + " )" );

            // Set "flop" and "step" here (this will take several lines).
            // Do NOT change any other code in this method below this line.

            // First part is to shrink the inchworm so its head moves
            // down towards its tail until it is a sphere while staying
            // in one square.

//          uuuuuuuh I feel like the arg here should be NEGATIVE
// 			since we're SHRINKING, according to the instructions
// 			for Inch
            yield return StartCoroutine(Inch(-secondsPerStep));

            // Next, rotate so the animation can be reversed to give the
            // appearance that the inchworm is growing horizontally,
            // with its head moving while its tail stays in one square.

            transform.localEulerAngles = flop;

// 			same thing except positive
            yield return StartCoroutine(Inch(secondsPerStep));

            // Change position and rotation as necessary to shrink again,
            // to give the appearance that inchworm's tail is shrinking
            // towards its head, while its head stays in one square.

            transform.localPosition += step;
            transform.localEulerAngles = -flop;

// 			same thing
            yield return StartCoroutine(Inch(-secondsPerStep));

            // Rotate again so growing the inchworm one more time will
            // animate it from a sphere to a vertical thin capsule shape,
            // by moving its head as its stands in one square.

            // Standing growing

            transform.localEulerAngles = Vector3.zero;

// 			same thing except positive
            yield return StartCoroutine(Inch(secondsPerStep));
        }

        // Add code to this method so it can be called as already written
        // above, to achieve a smooth animation, either starting as a sphere,
        // and growing to full size, or starting at full size and shrinking
        // to a sphere. The one argument indicate how many seconds it should
        // take to completely grow or completely shrink. Passing a positive
        // number means the animation should grow. Passing a negative numbers
        // means the animation should shrink.

        IEnumerator Inch(float secondsPerStep)
        {
            // This method is also called as a coroutine, so you
            // will need at least one yield statment in it somewhere.

// 			FixedUpdate runs 50 times per second (every .02 secs)
			float tmp = secondsPerStep * 50.0f;
			int frames = (int)Mathf.Round( Mathf.Abs( tmp ) );
			float keyfr = 1 / tmp;

			Vector3 tmp2 = Vector3.up * keyfr;

// 			waiting for fixed update ensures smooth, consisten animation
			for ( int i = 0; i < frames; i++ ) {
				bodyBaseT.localScale += tmp2;
				headT.localPosition += 2 * tmp2;
				yield return new WaitForFixedUpdate();
			}

// 			snap result to target position
			float tmp3 = secondsPerStep > 0 ? 1 : 0;
			bodyBaseT.localScale = new Vector3( 1, tmp3, 1 );
			headT.localPosition = Vector3.up * tmp3 * 2;
        }
    }
}
