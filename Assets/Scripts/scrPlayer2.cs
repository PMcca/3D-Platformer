﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class scrPlayer2 : MonoBehaviour
{
    public float
        gravity = 28f,
        jumpForce = 8f,
        wallJumpForce = 6f,
        terminalVelocity = -20f,
        maxSpeed = 4f,
        turnSpeed = 5f;
    public Transform camTrans, cubeTrans1, cubeTrans2;

    private float
        yVelocity,
        velocity = 0f,
        airForward,
        airRight,
        acceleration = 2.5f,
        extraSpeed = 1;
    private bool
        startedJump,
        isHoldingJump,
        isSliding,
        canSlide,
        canWallJump;

    private CharacterController cont;
    private Vector3 originalPos, localMove, colNormal, airMovement;


    private void Start()
    {
        cont = GetComponent<CharacterController>();

        //Get position that character starts game at 
        originalPos = transform.position;
        colNormal = Vector3.zero;
    }


    private void Update()
    {
        //Set walljump variable to false. If there is a collision with a wall, it will be set to true. 
        canWallJump = false;

        //Calculate localMove, the vector that is relative to the camera's perspective.
        CameraPerspective();

        if (Input.GetButtonDown("Jump"))
            Jump();

        
        if (velocity == 0)
        {
            NoInputRotate();
        }


        //Move player along ground if the are grounded.
        if (cont.isGrounded)
        {
            MoveCharGround();
        }

        else if (!cont.isGrounded)
        {
            canSlide = false;
            MoveCharAir();
        }

        if (transform.position.y <= -1.3)
            Restart();

            cont.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
            Gravity();

        if (colNormal.y > 0.01f)
        {
            Debug.Log(colNormal.y);
        }


        //Sliding 
        if(cont.isGrounded && !isSliding)
        {
            canSlide = true;
        }

        if (Input.GetButtonDown("RB") && canSlide == true)
        {
            Slide();
        }

        if(isSliding == true)
        {
            WhileSliding();
        }
        
        //If the player is colliding with a wall, allow them to walljump.
        if (canWallJump)
            WallJump();

        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, localMove, Color.green);

        //colNormal.y = 0;
        Debug.DrawRay(cubeTrans1.position, colNormal, Color.cyan);
    }


    //Move the character by the given vector movement
    void MoveCharGround()
    {
        //Prevents speed from increasing when going diagonally
        if (localMove.sqrMagnitude > 1f)
        {
            localMove = localMove.normalized;
        }

        //Damp rotation of character
        Damp(localMove);

        //Get new speed value
        velocity = DampSpeed(velocity);

        cont.Move(((transform.forward * Time.deltaTime) * velocity) * extraSpeed);

        //If there is no input from the player, and char's speed is < 0.3, then set speed to 0.
        if (!checkForInput() && velocity <= 0.3f)
            velocity = 0;

        //If the angle between the stick and the player's direction is larger than 150 degrees, incur a skid penalty.
        if (Vector3.Angle(transform.forward, localMove) >= 130 && velocity != 0 && cont.isGrounded)
            SkidTurnaround();
    }

    void MoveCharAir()
    {
        if (localMove.sqrMagnitude > 1f)
        {
            localMove = localMove.normalized;
        }

        velocity = DampSpeedAir(velocity);

        airForward = Vector3.Dot(transform.forward, localMove);
        airRight = Vector3.Dot(transform.right, localMove);
        
        airMovement = (transform.forward * velocity) + ((transform.right) * (airRight * 0.9f));

        cont.Move(airMovement * Time.deltaTime);
    }

    //Dampen turn rate when moving stick from one angle to another.
    void Damp(Vector3 original)
    {

        //newRatio is 90% of the time between the last frame.
        float newRatio = 0.9f * Time.deltaTime * 8;
        float oldRatio = 1 - newRatio;

        //The new direction to face is the current direction * by the oldRatio added to the direction of the stick * the new ratio
        Vector3 facingRot = ((transform.forward * oldRatio) + (original.normalized * newRatio)).normalized;
        facingRot.y = 0;

        transform.forward = facingRot;
    }

    
    float DampSpeed(float originalVelocity)
    {
        //Double the character's deceleration when there is no input and the character is grounded.
        if (!checkForInput() && cont.isGrounded)
            acceleration = 10;
        else
            acceleration = 2.5f;

        //Get the magnitude of the stick's tilt * by maxSpeed to get the desired speed.
        float desiredSpeed = maxSpeed * new Vector3(getXAxis(), 0, getZAxis()).magnitude;

        float new_ratio = 0.9f * Time.deltaTime * acceleration;
        float old_ratio = 1 - new_ratio;

        float newSpeed = (originalVelocity * old_ratio) + (desiredSpeed * new_ratio);

        newSpeed = Mathf.Clamp(newSpeed, -maxSpeed * extraSpeed, maxSpeed * extraSpeed);

        return newSpeed;
    }

    //Overloaded DampSpeed
    float DampSpeedAir(float originalVelocity)
    {
        acceleration = 1.5f;

        float desiredSpeed = maxSpeed * airForward;

        float new_ratio = 0.9f * Time.deltaTime * acceleration;
        float old_ratio = 1 - new_ratio;

        float newSpeed = (originalVelocity * old_ratio) + (desiredSpeed * new_ratio);

        newSpeed = Mathf.Clamp(newSpeed, -maxSpeed * extraSpeed, maxSpeed * extraSpeed);

        return newSpeed;
    }


    //Slow the character if stick tilt is at a big enough difference to forward movement direction.
    void SkidTurnaround()
    {
            transform.forward *= -1;
            float skidVelocity = (velocity * -1);

            velocity = skidVelocity / 2;   
    }


    //Translate character's movement from world to camera perspective 
    void CameraPerspective()
    {
        //Get the forward vector of camera (what it's looking at)
        Vector3 lookDir = camTrans.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        //Get the right vector of the camera
        Vector3 right = camTrans.right;
        right.y = 0;
        right = right.normalized;


        //X movement is given by the camera's right.x and forward.x which are multipled by left/right and up/down stick input respectively.
        //If the camera is looking to the side of the character, then cam.forward.x = 1, and cam.right.x = 0. So vertical * forward.x = movement across x plane.
        localMove.x = (Input.GetAxisRaw("Horizontal") * right.x) + (Input.GetAxis("Vertical") * lookDir.x);
        //Same with Z except dictates how he will move across the Z plane
        localMove.z = (Input.GetAxisRaw("Horizontal") * right.z) + (Input.GetAxis("Vertical") * lookDir.z);

        //Vertical movement not applicable.
        localMove.y = 0;

        //Normalize as only direction is needed.
        localMove = localMove.normalized;

    }

    //Rotate's the character immediately toward the stick tilt if there is no input prior
    void NoInputRotate()
    {
        //If the character is grounded and the stick is tilted, rotate the character in that direction.
        if (cont.isGrounded && checkForInput())
        {
            transform.forward = localMove;
        }

        //Ensure character keeps last known rotation
        else if (!checkForInput())
        {
            transform.forward = transform.forward;
        }
    }

    //Apply gravity to the character
    void Gravity()
    {
        //Set isHoldingJump relative to if the player is holding the jump button
        if(Input.GetButton("Jump"))
        {
            isHoldingJump = true;
        }
        else
        {
            isHoldingJump = false;
        }


        //If character is grounded and he isn't jumping, apply a miniscule amount of gravity.
        if (cont.isGrounded && !startedJump)
        {
            gravity = 14f;
            yVelocity = -gravity * Time.deltaTime;
        }

        //If the player is jumping (not falling) and they aren't holding the jump button, increase their gravity
        //Creates short-hop like jump
        if(!isFalling() && !cont.isGrounded)
        {
            if (!isHoldingJump)
            {
                gravity = 24f;
            }
            else
                gravity = 14f;
        }

        //If character is airborne, subtract gravity from velocity.
        if(!cont.isGrounded)
        {
            yVelocity -= gravity * Time.deltaTime;
            startedJump = false;
        }
        
        //If the character is falling (not jumping), revert gravity to normal.
        if (isFalling())
        {
            gravity = 14f;
        }

        //Ensure the character doesn't fall faster than their terminalVelocity.
        if(isFalling() && yVelocity < terminalVelocity)
        {
            yVelocity = terminalVelocity;
        }
    }

    void WallJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            transform.forward = colNormal.normalized;
            yVelocity = wallJumpForce;
            extraSpeed = 1.2f;
        }
    }

    //Change the value of yVelocity, allowing the character to jump when MoveChar() is called
    void Jump()
    {
        if (cont.isGrounded)
        {
            yVelocity = jumpForce;
            startedJump = true;
        }
        
    }

    void Slide()
    {
        extraSpeed = 2.7f;
        isSliding = true;
        canSlide = false;
    }

    void WhileSliding()
    {
        extraSpeed = Mathf.Lerp(extraSpeed, 1, 0.1f);

        if (extraSpeed <= 1.02f)
        {
            extraSpeed = 1;
        }
        

        if(extraSpeed == 1)
        {
            isSliding = false;
            canSlide = true;
        }
    }

    //Reset character's position to original.
    void Restart()
    {
        transform.position = originalPos;
    }

    float getXAxis()
    {
        return Input.GetAxis("Horizontal");
    }

    float getZAxis()
    {
        return Input.GetAxisRaw("Vertical");
    }

    //Check stick axis to check if there is any input from the player
    //True = input, false = no input
    bool checkForInput()
    {
        if (getXAxis() != 0 || getZAxis() != 0)
            return true;
        else
            return false;
    }

    //Check if character is falling
    bool isFalling()
    {
        if (!cont.isGrounded && yVelocity <= 0)
        {
            return true;
        }
        else return false;
    }
    
    
    //If the player collides with a "Wall" object, allow the player to walljump.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Wall")
        {
            if (!cont.isGrounded)
            {
                canWallJump = true;
            }
            colNormal = hit.normal.normalized;
        }

        if(hit.gameObject.tag == "ptfmRotate" || hit.gameObject.tag == "ptfmVertical" || hit.gameObject.tag == "ptfmHoriz")
        {

        }

    }
    

}
