using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsScript : MonoBehaviour
{
    float timeOn = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        timeOn += Time.deltaTime;

        if (timeOn > 10)
            gameObject.SetActive(false);
    }
}
