using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    public VoxelChunk voxelChunk;
    public Text playerUI;

    // used to know which block the user chose to place
    private int chosenBlock = 1;

    bool ActionOnBlock(out Vector3 v, float dist, bool destroy, int blockType)
    {
        v = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dist))
        {
            // if the raycasted object is a collectable
            if (hit.collider.gameObject.tag == "Collectable")
                return false;

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

        if (Input.GetKey(KeyCode.Alpha1))
            chosenBlock = 1;
        if (Input.GetKey(KeyCode.Alpha2))
            chosenBlock = 2;
        if (Input.GetKey(KeyCode.Alpha3))
            chosenBlock = 3;
        if (Input.GetKey(KeyCode.Alpha4))
            chosenBlock = 4;

        playerUI.text = "Chosen block: " + chosenBlock;

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 v;
			if(ActionOnBlock(out v, 4, false, 0))
            {
                voxelChunk.SetBlock(v, 0);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 v;
			if (ActionOnBlock(out v, 4, true, chosenBlock))
            {
                Debug.Log(v);
                voxelChunk.SetBlock(v, chosenBlock);
            }
        }

	}
}
