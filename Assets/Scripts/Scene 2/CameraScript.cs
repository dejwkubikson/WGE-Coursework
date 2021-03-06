﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script should be attached to the main camera game object. Performs actions on camera (moving, zooming).
public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed = 0.2f; // camera's speed following the player
    public Transform cameraHolder;

    public bool shakeCameraOnce = false;
    public bool cameraShakeActive = false;
    public bool followPlayer = true; // if true the camera will follow the player
    public bool followNPC = false; // if true the camera will follow the NPC
    public bool zoomCamera = false; // if true the camera will zoom on the following character
    public float zoomDesired = 0.0f;
    public bool cameraComingFromZoom = false; // set to true when camera is 'coming back' from a dialogue (zooming out)

    private float leftBorder = -2.4f; // the left border the camera shouldn't move out of
    private float rightBorder = 26.9f; // the right border the camera shouldn't move out of
    private Vector3 rightBorderPos; // vector3 position of the right border

    private bool getNPCOffsetOnce = false;
    private Vector3 offsetNPC; // offset to the NPC
    private Vector3 offset; // offset to the player
    private float zoomAtStart; // getting the ortographic zoom at the start
    private Camera camera;
    private float timeCounter = 0; // used for smooth transition

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        camera = gameObject.GetComponent<Camera>();
        zoomAtStart = camera.orthographicSize;
    }

    // Switches the focus, if following player, the camera will switch to follow the NPC
    public void SwitchCameraFocus()
    {
        if (followNPC)
        {
            followNPC = false;
            followPlayer = true;
        }
        else
        {
            followPlayer = false;
            followNPC = true;
        }
    }

    // Returns a perlin float between -1 and 1, based off the time counter.
    float GetSeed(float seed)
    {
        return (Mathf.PerlinNoise(seed, timeCounter) * 2f);
    }

    // Generates new Vector3 coords
    Vector3 GetVect3()
    {
        // Used to get more clean code
        float cameraHolderX = cameraHolder.transform.position.x;
        float cameraHolderY = cameraHolder.transform.position.y;

        return new Vector3(GetSeed(cameraHolderX) + cameraHolderX, GetSeed(cameraHolderY) + cameraHolderY, -15);
    }

    // Shakes camera. Changes a boolean value so that the camera shake isn't performed every frame the player is walking on the ground
    public IEnumerator ShakeCamera()
    {
        shakeCameraOnce = true;

        cameraShakeActive = true;
        yield return new WaitForSeconds(0.1f);
        cameraShakeActive = false;
    }

    // In LateUpdate() the player object was a bit 'woobly' when moving
    void FixedUpdate()
    {
        // If the camera should be following the player
        if (followPlayer)
        {
            Vector3 playerPos = player.transform.position + offset;
            Vector3 lerpedPos = Vector3.Lerp(transform.position, playerPos, cameraSpeed * cameraSpeed);
            transform.position = lerpedPos;
        }

        // If the camera should be following the NPC
        if(followNPC)
        {
            if(!(getNPCOffsetOnce))
            {
                getNPCOffsetOnce = true;
                offsetNPC = transform.position - GameObject.FindGameObjectWithTag("NPC").gameObject.transform.position;
            }

            Vector3 npcPos = GameObject.FindGameObjectWithTag("NPC").gameObject.transform.position + offset;
            Vector3 lerpedNpcPos = Vector3.Lerp(transform.position, npcPos, cameraSpeed * cameraSpeed);
            transform.position = lerpedNpcPos;
        }

        // Mixing the position of right border (X) with the camera's current Y and Z to lerp it when the dialogue ends
        rightBorderPos = new Vector3(rightBorder, transform.position.y, transform.position.z);

        // Zooming camera
        if (zoomCamera)
        {
            // Smooth zoom
            if (camera.orthographicSize > zoomDesired)
                camera.orthographicSize -= Time.deltaTime * 2;
            else
                camera.orthographicSize = zoomDesired;
        }
        else
        {
            // Letting the camera zoom out before moving it back to position
            if (camera.orthographicSize < zoomAtStart)
                camera.orthographicSize += Time.deltaTime * 2;
            else
            {
                // Setting the zoom to initial
                camera.orthographicSize = zoomAtStart;

                // If the camera is coming back from an ended dialogue we want it to move to its position smoothly
                if (cameraComingFromZoom)
                {
                    // Not following player until its in the right position
                    followPlayer = false;
                    // Lerping camera's position with the right border position
                    Vector3 lerpedFromZoom = Vector3.Lerp(transform.position, rightBorderPos, cameraSpeed * cameraSpeed);
                    transform.position = lerpedFromZoom;

                    // If the camera is in its initial position (before the dialogue) it will follow the player again
                    if (transform.position == rightBorderPos)
                    {
                        cameraComingFromZoom = false;
                        followPlayer = true;
                    }
                }
                else
                {
                    // Blocking the camera from moving to far left, thus giving the player a hint that he's supposed to go right
                    if (transform.position.x < leftBorder)
                        transform.position = new Vector3(leftBorder, transform.position.y, transform.position.z);

                    // Blocking the camera from moving to far right, thus giving the player a hint that it's the end of the map
                    if (transform.position.x > rightBorder)
                        transform.position = new Vector3(rightBorder, transform.position.y, transform.position.z);
                }
            }
        }

        // The camera shake will be quick, not too big and smooth
        if (cameraShakeActive)
        {
            timeCounter += Time.deltaTime;
            Vector3 newPos = GetVect3();
            Vector3 lerpedShake = Vector3.Lerp(cameraHolder.transform.position, newPos, 0.05f);
            cameraHolder.transform.position = lerpedShake;
        }
    }
}
