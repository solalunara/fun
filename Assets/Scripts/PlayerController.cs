using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    public float speed = 5.0f;
    public float drag = 20.0f;

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
        _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _moveDirection *= speed;

        /**
        // jump
        if (Input.GetButton("Jump"))
        {
            _moveDirection.y = jumpSpeed;
        }
        **/

        // apply gravity
        _moveDirection.y -= drag * Time.deltaTime * Time.deltaTime;

        // move the character
        controller.Move(_moveDirection);

    }
    
}
