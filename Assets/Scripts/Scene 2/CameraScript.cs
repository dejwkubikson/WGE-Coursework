using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed = 0.2f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // In LateUpdate() the player object was a bit 'woobly' when moving
    void FixedUpdate()
    {
        Vector3 playerPos = player.transform.position + offset;
        Vector3 lerpedPos = Vector3.Lerp(transform.position, playerPos, cameraSpeed * cameraSpeed);
        transform.position = lerpedPos;
    }
}
