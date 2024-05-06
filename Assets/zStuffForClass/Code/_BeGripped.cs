using UnityEngine;

public class _BeGripped : MonoBehaviour
{
    public Transform hand;

    Rigidbody rb;

    int tCount;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter()
    {
        Debug.Log("OnTriggerEnter!");
        tCount = tCount + 1;
        Debug.Log(tCount);
        if (tCount == 3)
        {
            transform.SetParent(hand);
            rb.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit!");

        transform.parent = null;
        tCount = 0;
        rb.isKinematic = false;

        Debug.Log(tCount);
    }
}
