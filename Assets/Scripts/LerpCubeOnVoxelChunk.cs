using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCubeOnVoxelChunk : MonoBehaviour {
    public VoxelChunk voxelChunk;
    private Vector3 currentPos;
    private bool lerpBool = false;
    int nextDestination = 0;
    bool reachedDestination = false;
    bool inCoroutine = false;
    Vector3 nextDestinationVector;

    IEnumerator LerpPosition(Vector3 start, Vector3 end, float maxTime)
    {
        inCoroutine = true;

        float t = 0;
        while (t < maxTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / maxTime);
            if (t >= maxTime)
            {
                transform.position = end;
            }
            yield return null;
        }

        inCoroutine = false;
    }

    // Use this for initialization
    void Start()
    {
        currentPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        // changing the values pos values due to the size of the cube and it's center
        for (int i = 0; i < voxelChunk.waypoints.Count; i++)
        {
            voxelChunk.waypoints[i] += currentPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);

        if (Input.GetKeyDown("space"))
        {
            if (lerpBool)
            {
                nextDestination = 0;
                lerpBool = false;
                this.transform.position = new Vector3(0.5f, 4.5f, 1.5f);
                Debug.Log("1 if");
            }
            else
            {
                lerpBool = true;
            }
        }
            
        if (lerpBool && !(inCoroutine) && nextDestination < voxelChunk.waypoints.Count)
        {

            nextDestinationVector = voxelChunk.waypoints[nextDestination] + new Vector3(0, 0.5f, 0);

            StartCoroutine(LerpPosition(currentPos, nextDestinationVector, 1));

            nextDestination++;
        }

    }
}
