using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCamera : MonoBehaviour {

    private Vector3 offset;
    public Transform playerTrans;

    public float
        sensX = 4f,
        sensY = 7f;

    private float
        currentX,
        currentY,
        timer;

    private const float Y_MIN = -15f, Y_MAX = 50f;

	void Start () {
        offset = transform.position - playerTrans.position;
		
	}

    private void Update()
    {
        //Both X and Y are given by the mouse/stick
        currentX += Input.GetAxisRaw("Mouse X") * sensX;
        currentY += Input.GetAxisRaw("Mouse Y") * sensY;

        //Ensure Y axis can't "flip" when being held.
        currentY = Mathf.Clamp(currentY, Y_MIN, Y_MAX);
    }

    void LateUpdate () {
        Vector3 direction = offset;

        //Make quaternion based on the mouse/stick input. i.e. Rotate camera around Z axis by the units given by currentY. 
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        //Slerp to make it smoother. New position is player's pos + the rotation * the offset/direction.
        transform.position = Vector3.Slerp(transform.position, (playerTrans.position + rotation * direction), Time.deltaTime * 10);

        //Look at the player every frame.
        transform.LookAt(playerTrans.position);

    }
}
