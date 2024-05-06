using System.Collections.Generic;
using UnityEngine;

public class _Transformer : MonoBehaviour
{
    public Vector3 translationRate;
    public Vector3 rotationRate;
    public string key;

    void Update()
    {
        Vector3 tempTranslationRate = translationRate/2;
        Vector3 tempRotationRate = rotationRate/2;
        

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(key))
        {
            tempRotationRate = tempRotationRate * -1;
            tempTranslationRate = tempTranslationRate * -1;
            transform.rotation *= Quaternion.Euler(tempRotationRate * Time.deltaTime);
            transform.Translate(tempTranslationRate * Time.deltaTime, Space.Self);
            Debug.Log("Shift click");
        }

        else if (Input.GetKey(key))
        {
           
            transform.rotation *= Quaternion.Euler(tempRotationRate * Time.deltaTime);
            transform.Translate(tempTranslationRate * Time.deltaTime, Space.Self);
            Debug.Log("Click");
        }

        
    }
        
}
