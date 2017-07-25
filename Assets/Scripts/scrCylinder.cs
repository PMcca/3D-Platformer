using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCylinder : MonoBehaviour {

    private Vector3 pos;
    private static float exponent = 0.1f;

    public Transform cubeTrans;
    private Vector3 offset, originalPos;
    float velocity = 0f;

    // Use this for initialization
    void Start () {
        pos = transform.position;
        offset = cubeTrans.transform.position - transform.position;
        originalPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        /*Vector3 PA = new Vector3(5, 0.7f, 3.2f);
        Vector3 PB = new Vector3(-5, 0.7f, 3.2f);

        if (Input.GetKeyDown(KeyCode.T))
        {
            originalPos = transform.position;
            exponent = 0.1f;
            pos = PA;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            originalPos = transform.position;
            exponent = 0.1f;
            pos = PB;
        }
        transform.position = Vector3.Slerp(originalPos, pos, exponent);
        if (transform.position != pos)
            exponent += 0.02f;
        Debug.Log(exponent);*/
        Vector3 targetPos = Vector3.zero;

        if (Input.GetKey(KeyCode.T))
        {
             targetPos = new Vector3(5, 0.7f, 3.2f);
        }
        
        float smoothTime = 0.3f;
        
        float newPos = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocity, smoothTime);
        transform.position = new Vector3(newPos, transform.position.y, transform.position.z);


        
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.gameObject.name);
    }
}

