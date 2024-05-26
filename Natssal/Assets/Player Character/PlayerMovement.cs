using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Adds a slider to control this property in Unity Inspector
    [Range(0f, 1f)]
    public float groundDecay;
    public float acceleration; 
    public float maxSpeed;

    public float jumpSpeed;

    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public Rigidbody2D body;

    public Animator animator;

    public bool isGrounded;

    // Does not need public since we do not need to see it in the Unity Inspector or be accessible anywhere else
    float xInput;
    float yInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        HandleJump();
    }

    // FixedUpdate is called with the same time between each interval
    void FixedUpdate()
    {
        CheckGround();
        HandleHorizontalMovement();

        // TODO: Honestly, the functionality seems no different when I comment ApplyFriction() out. So I hope I did it right lol.
        ApplyFriction();
    }

    void CheckInput()
    {
        // https://docs.unity3d.com/ScriptReference/Input.GetAxis.html 
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void HandleHorizontalMovement()
    {
        float animatorSpeed = 0;
        /*
        * Input.GetAxis returns a number different than zero if the user is giving an input.
        * So the only axis we want to be effected is the axis the user is trying to control.
        * The axis not being controlled should just match whatever the body's current velocity is for that axis.
        */
        if (Mathf.Abs(xInput) > 0)
        {
            // Increment velocity by our acceleration, then clamp within max
            float increment = xInput * acceleration;
            // First argument must stay in between the second and third argument
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -maxSpeed, maxSpeed);

            animatorSpeed = Mathf.Abs(newSpeed);

            /*
             * Vector is just a object that has magnitude and direction https://mathinsight.org/vector_introduction
             * Vector2 is a Unity object / class. That's why we create a new one and assign it to a property of the body
             */
            body.velocity = new Vector2(newSpeed, body.velocity.y);

            UpdateDirection();
        }

        animator.SetFloat("Speed", animatorSpeed);
    }

    void UpdateDirection()
    {
        // Determine whether the input is positive or negative
        float direction = Mathf.Sign(xInput);
        // Update the x value of the player scale. Negative will face left. Positive will face right.
        transform.localScale = new Vector3(direction, 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            animator.SetBool("isJumping", true);
        }
    }

    void CheckGround()
    {
        /*
         * https://docs.unity3d.com/ScriptReference/Physics2D.OverlapAreaAll.html
         * We get all colliders that fall within a rectangular area specified by the first and second argument.
         * In this case, we are looking to see if there are any colliders within the area of our Ground Check collider.
         * Obviously there is a collider in that area, the Ground Check collider itself. This is where the third argument comes in.
         * We can specify a layer mask to only check for objects that have that layer mask. So that means any collider with the 
         * specified mask that is within the area of our Ground Check collider will be returned from this function AKA they are touching.
         * We get an array of colliders as a return value.
         */
        isGrounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;

        /*
         * TODO: Understand why we need the body.velocity.y check. Without it, isJumping gets set to true then back to false instantly.
         * I am pretty sure it has to do when isGround and isJumping gets updated. I'm thinking that the frame
         * isJumping gets set to true, isGrounded is also still true. But somehow velocity y is already increased. But how is that the case?
         */
        if (isGrounded && animator.GetBool("isJumping") && (body.velocity.y < 0.1))
        {
            animator.SetBool("isJumping", false);
        }
    }

    void ApplyFriction()
    {
        bool isNoHorizontalInput = xInput == 0;
        bool isNotJumping = body.velocity.y <= 0;

        // We only want to apply the ground decay when the user stop running AKA lets go of the horizontal input.
        if (isGrounded && isNoHorizontalInput && isNotJumping)
        {
            body.velocity *= groundDecay;
        }
    }

    // TODO: velocity y is negative when falling
}
