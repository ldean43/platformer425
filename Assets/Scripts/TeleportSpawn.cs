using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpawn : MonoBehaviour
{
    public Transform teleportDestination;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Check_pls");
        if(other.gameObject.name == "Player")
        {
            other.transform.position = teleportDestination.position;
        }
    }
}
