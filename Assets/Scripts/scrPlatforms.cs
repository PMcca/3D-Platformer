using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlatforms : MonoBehaviour {

    public float moveSpeed = 1;
    private float trigCounter = 0;

    public Transform platformx1;

    private Vector3 offset;

    void Start () {

        offset = platformx1.position;
    }
	
	void Update () {
        trigCounter += Time.deltaTime;

        if (platformx1.tag == "ptfmRotate")
            RotatePlatform();
        if (platformx1.tag == "ptfmVertical")
            VerticalPlatform();
	}

    void RotatePlatform()
    {
        platformx1.position = new Vector3(offset.x + Mathf.Cos(trigCounter * moveSpeed), 0, offset.z + Mathf.Sin(trigCounter * moveSpeed));
    }

    void VerticalPlatform()
    {
        platformx1.position = new Vector3(offset.x, offset.y + Mathf.Cos(trigCounter * moveSpeed), offset.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Player") && other.GetComponent<CharacterController>().isGrounded)
        {
            other.transform.parent = transform;
            Debug.Log("worksSs");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Exit" + other.GetComponent<CharacterController>().isGrounded);
            other.transform.parent = null;
        }
    }

}

