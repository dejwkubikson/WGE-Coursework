using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllersScript : MonoBehaviour
{
    float timeOn = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        timeOn += Time.deltaTime;

        // If the controllers are shown for more than 10 seconds
        if (timeOn > 10)
            Destroy(gameObject);
    }
}
