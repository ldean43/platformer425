using UnityEngine;

public class BeGripped : MonoBehaviour
{
	public Transform hand;

	Rigidbody rb;

	int tCount;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		tCount += 1;
		Debug.Log( "Tcount: " + tCount );
		if ( tCount >= 3 )
			this.transform.parent = hand;
		rb.constraints = 	RigidbodyConstraints.FreezePositionZ |
							RigidbodyConstraints.FreezePositionY |
							RigidbodyConstraints.FreezePositionX |
							RigidbodyConstraints.FreezeRotationZ |
							RigidbodyConstraints.FreezeRotationY |
							RigidbodyConstraints.FreezeRotationX;
	}

	private void OnTriggerExit(Collider other)
	{
		tCount -= 1;
		this.transform.parent = null;
		rb.constraints = RigidbodyConstraints.None;
	}
}
