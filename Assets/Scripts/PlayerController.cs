using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rPlayerBody;
    BoxCollider2D bPlayerCollider;
    //CharacterController controller;

    public int speed = 5000;
    //public float drag = 20.0f;

    //private Vector2 _moveDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        //controller = GetComponent<CharacterController>();
        rPlayerBody = GetComponent<Rigidbody2D>();
        bPlayerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks which arrow keys are pressed
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);

        
        Vector2 netHorizontalForce = NetForceHorizontal(leftPressed, rightPressed);

        Debug.Log(netHorizontalForce);

        rPlayerBody.AddForce(netHorizontalForce);




        /*
        // movedirection lmao
        _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _moveDirection *= speed;

        
        // jump
        if (Input.GetButton("Jump"))
        {
            _moveDirection.y = jumpSpeed;
        }
        

        // apply gravity
        _moveDirection.y -= drag * Time.deltaTime * Time.deltaTime;

        // move the character
        controller.Move(_moveDirection);
       */

    }

    private Vector2 NetForceHorizontal(bool leftPressed, bool rightPressed)
    {
        if(leftPressed && rightPressed) return Vector2.zero;

        Vector2 netForceApplied = new Vector2();

        if(leftPressed) 
        {
            netForceApplied += Vector2.left*speed*Time.deltaTime;
        }
        if(rightPressed)
        {
            netForceApplied += Vector2.right*speed*Time.deltaTime;
        }
        return netForceApplied;
    }
    
}
