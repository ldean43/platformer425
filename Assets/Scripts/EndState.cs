using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndState : MonoBehaviour
{
    // Start is called before the first frame update
    public int nextScene;
    public PlayerController plr;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (plr.isGrounded()) {
            SceneManager.LoadScene(nextScene);
        }
    }
}
