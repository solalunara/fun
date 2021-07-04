using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    public float speed = 5.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 20.0f;
    public float maxJumps = 2;

    private Vector2 _moveDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // movedirection lmao
        _moveDirection = new Vector2(speed, Input.GetAxis("Vertical"));
        _moveDirection *= speed;

        // jump
        if (Input.GetButton("Jump"))
        {
            _moveDirection.y = jumpSpeed;
        }
        
        // apply gravity
        _moveDirection.y -= gravity * Time.deltaTime * Time.deltaTime;

        // move the character
        controller.Move(_moveDirection);

    }
    
}
