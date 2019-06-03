using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(scr_Controller2D))]
public class scr_Player : MonoBehaviour
{
    public float jumpHeight = 4f, secondsToJumpApex = 0.4f;

    float moveSpeed = 12f, jumpSpeed;
    float grav;
    float moveAcceleration = .06f, moveSmoothVal;
    Vector3 velocity;

    scr_Controller2D controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<scr_Controller2D>();

        grav = -(2 * jumpHeight) / Mathf.Pow(secondsToJumpApex, 2);
        jumpSpeed = Mathf.Abs(grav) * secondsToJumpApex;        
    }

    // Update is called once per frame
    void Update()
    {
        //Reset gravity on collisions
        if (controller.colInfo.above || controller.colInfo.below)
            velocity.y = 0;

        //Get left/right input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Get jump input
        // 
        if (Input.GetKeyDown(KeyCode.Space) && controller.colInfo.below)
        {
            velocity.y += jumpSpeed;
        }

        //Apply force based on input
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref moveSmoothVal, moveAcceleration);

        //Apply gravity
        velocity.y += grav * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
