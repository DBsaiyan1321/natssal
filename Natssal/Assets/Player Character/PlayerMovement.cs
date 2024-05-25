using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float groundSpeed;
    public float jumpSpeed;
    public Rigidbody2D body;
    [Range(0f, 1f)] // Adds a slider to control this property in Unity Inspector
    public float groundDecay;
    public bool isGrounded;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;

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
        GetInput();
        MovePlayerWithInput();
    }

    // FixedUpdate is called with the same time between each interval
    void FixedUpdate()
    {
        CheckGround();

        // Hoenstly, the functionality seems no different when I comment ApplyFriction() out. So I hope I did it right lol.
        ApplyFriction();
    }

    void GetInput()
    {
        // https://docs.unity3d.com/ScriptReference/Input.GetAxis.html 
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void MovePlayerWithInput()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            /*
             * Vector is just a object that has magnitude and direction https://mathinsight.org/vector_introduction
             * Vector2 is a Unity object / class. That's why we create a new one and assign it to a property of the body
             */
            body.velocity = new Vector2(xInput * groundSpeed, body.velocity.y);

            // TODO: Look more into this code and understand it. This is how we change the direction of the player.
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }

        /*
         * Input.GetAxis returns a number different than zero if the user is giving an input.
         * So the only axis we want to be effected is the axis the user is trying to control.
         * The axis not being controlled should just match whatever the body's current velocity is for that axis.
         */
        if (Mathf.Abs(yInput) > 0 && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, 1 * jumpSpeed);
        }
    }

    void CheckGround()
    {
        // TODO: What does this line do?
        isGrounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction()
    {
        bool isNoHorizontalInput = xInput == 0;
        bool isNoVerticalInput = yInput == 0;

        // We only want to apply the ground decay when the user stop running AKA lets go of the horizontal input.
        if (isGrounded && isNoHorizontalInput && isNoVerticalInput)
        {
            body.velocity *= groundDecay;
        }
    }
}
