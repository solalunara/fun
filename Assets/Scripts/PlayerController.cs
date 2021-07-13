using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rPlayerBody;
    BoxCollider2D bPlayerCollider;

    public const int speed = 5000;

    // Start is called before the first frame update
    void Start()
    {
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

        //Debug.Log(netHorizontalForce);

        rPlayerBody.AddForce(netHorizontalForce);

        if (rPlayerBody.velocity.magnitude * Time.deltaTime >= 0.1)
        {
            CollisionHelper();
        }


    }

    private void CollisionHelper()
    {
        //Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        //This would cast rays only against colliders in layer 8, so we just inverse the mask.
        layerMask = ~layerMask;

        RaycastHit2D thingHit = Physics2D.Raycast((Vector2)this.transform.position, rPlayerBody.velocity.normalized, rPlayerBody.velocity.magnitude * Time.deltaTime, layerMask);
        if (thingHit)
        {
            Debug.Log("Possible Collision with: " + thingHit.collider.tag);
            Vector2 playerDims = GetComponent<SpriteRenderer>().size;

            //float angle = Mathf.Atan2(rPlayerBody.velocity.y, rPlayerBody.velocity.x);
            //float criticalAngle = Mathf.Atan2(playerDims.y, playerDims.x);

            this.transform.position = thingHit.point + ((Vector2)transform.position)*-.1f;
            rPlayerBody.velocity = -rPlayerBody.velocity;

            //note: not complete yet
        }
    }

    private Vector2 NetForceHorizontal(bool leftPressed, bool rightPressed)
    {
        if (leftPressed && rightPressed) return Vector2.zero;

        Vector2 netForceApplied = new Vector2();

        if (leftPressed)
        {
            netForceApplied += Vector2.left * speed * Time.deltaTime;
        }
        if (rightPressed)
        {
            netForceApplied += Vector2.right * speed * Time.deltaTime;
        }
        return netForceApplied;
    }

    private void OnCollisionEnter2D(Collision2D cOther)
    {
        //Debug.Log( "this object just collided" );
    }
    private void OnCollisionStay2D(Collision2D cOther)
    {
        //Debug.Log( "collision staying" );
    }
    private void OnCollisionExit2D(Collision2D cOther)
    {
        //Debug.Log( "this object is no longer colliding with an object" );
    }

}

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

