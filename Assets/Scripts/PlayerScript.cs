using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public VoxelChunk voxelChunk;

    /* bool PickEmptyBlock(out Vector3 v, float dist)
     {
         v = new Vector3();
         Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
         RaycastHit hit;

         if(Physics.Raycast(ray, out hit, dist))
         {
             // offset towards the centre of the neighbouring block
             v = hit.point + hit.normal / 2;
             // round down to get the index of the empty;
             v.x = Mathf.Floor(v.x);
             v.y = Mathf.Floor(v.y);
             v.z = Mathf.Floor(v.z);
             return true;
         }
         return false;
     }

     bool PickThisBlock(out Vector3 v, float dist)
     {
         v = new Vector3();
         Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit, dist))
         {
             // offset towards the centre of the block hit
             v = hit.point - hit.normal / 2;
             // round down to get the index of the block hit
             v.x = Mathf.Floor(v.x);
             v.y = Mathf.Floor(v.y);
             v.z = Mathf.Floor(v.z);
             return true;
         }
         return false;
     }
     */

    bool PickBlock(out Vector3 v, float dist, bool destroy)
    {
        v = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dist))
        {
            // offset towards the centre of the neighbouring block
            if (destroy)
                v = hit.point + hit.normal / 2;
            else
                v = hit.point - hit.normal / 2;

            // round down to get the index of the empty;
            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return true;
        }
        return false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetButtonDown("Fire1"))
        {
            Vector3 v;
            if(PickBlock(out v, 4, false))
            {
                voxelChunk.SetBlock(v, 0);
            }
            /*if (PickThisBlock(out v, 4))
            {
                voxelChunk.SetBlock(v, 0);
            }*/
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 v;
            if (PickBlock(out v, 4, true))
            {
                Debug.Log(v);
                voxelChunk.SetBlock(v, 1);
            }
            /*if (PickEmptyBlock(out v, 4))
            {
                Debug.Log(v);
                voxelChunk.SetBlock(v, 1);
            }*/
        }

	}
}
