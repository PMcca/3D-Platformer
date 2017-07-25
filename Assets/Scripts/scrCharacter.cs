using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCharacter : MonoBehaviour
{
    public float
        speed = 6.0f,
        turnSpeed = 50.0f,
        acceleration,
        jumpSpeed = 40f;

    public Vector3 gravity = Vector3.zero;
    private Vector3 movement;
    public float gravityF;

    private CharacterController charCont;

    void Start()
    {
        gravity = new Vector3(0, 10f, 0);
    }

    void Update()
    {
        charCont = GetComponent<CharacterController>();
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        while (charCont.isGrounded)
        {
            movement *= speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.Space))
                movement.y = jumpSpeed;
        }
        movement.y -= gravityF;
        charCont.Move(movement);


        //Movement
        {
            //acceleration = speed * Input.GetAxis("Vertical");
            /*
            if (Input.GetAxis("Vertical") > 0)
                transform.Translate(Vector3.forward * acceleration * Time.deltaTime);
            if (Input.GetAxis("Vertical") < 0)
                transform.Translate(-Vector3.forward * -acceleration * Time.deltaTime);
            if (Input.GetKey(KeyCode.Space))
                transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
                */
            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(-Vector3.up * turnSpeed * Time.deltaTime);
            
        }
        
	
        }
    }

