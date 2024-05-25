using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // https://docs.unity3d.com/ScriptReference/Input.GetAxis.html 
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(xInput) > 0)
        {
            /*
         * Vector is just a object that has magnitude and direction https://mathinsight.org/vector_introduction
         * Vector2 is a Unity object / class. That's why we create a new one and assign it to a property of the body
         */
            body.velocity = new Vector2(xInput * speed, body.velocity.y);
        }

        /*
         * Input.GetAxis returns a number different than zero if the user is giving an input.
         * So the only axis we want to be effected is the axis the user is trying to control.
         * The axis not being controlled should just match whatever the body's current velocity is for that axis.
         */
        if (Mathf.Abs(yInput) > 0)
        {
            body.velocity = new Vector2(body.velocity.x, yInput * speed);
        }
    }
}