using UnityEngine;
using System.Collections;

/*
 * This Character Controller assumes the following:
 *  - The Character game object (aka the character) it is attached to has:
 *      - A child game object indicating the position of the 'GroundCheck' point
 *      - A child game object indicating the position of the 'HeadCheck' point
 *      - An Animator that has the following parameters: Speed (float), Duck (bool)
 *        vspeed (float), Ground (bool) and hit (bool)
 *   
 * The character is moved horizontally by detecting movement on the horizontal input axis
 * and multiplying this value (between -1 and 1) by the horizontalSpeed the user sets via
 * the inspector. This calculated value will be a number between -HorizontalSpeed and
 * +HorizontalSpeed. We inturn use this value to calculate the desired velocity (velocity 
 * being speed in a particular direction - think arrows :-) ).
 * 
 * The character can only jump if on the ground. The way we determine whether or not the character
 * is on the ground is by getting all Collider2D object (if any), within a certain radius of 
 * a given point, that are attached to game object on a certain layer.  The point we use is the 
 * transform of the child 'GroundCheck' game object. The layer that the game objects have to be
 * on is stored in whatIsGround which is set via the inspector. We use the function 
 * Physics2D.OverlapCircle to get the Collider2D object.
 * 
 * It the player 'taps' the spacebar then the character will do a low jump. If the player holds
 * down the spacebar then the character will do a higher jump. The way we do this is by detecting
 * if the player is holding down the spacebar key. If they are not then we increase gravity so that 
 * the character is pulled to the ground faster than usual.
 * 
 */
public class AdventureController : MonoBehaviour
{
    public float jumpSpeed;
    public float fallMultiplier = 5.0f;
    public float lowJumpMultiplier = 3.0f;
    public float horizontalSpeed = 10;
    public LayerMask whatIsGround;
    public Transform groundcheck;
    private float groundRadius = 0.5f;
    private bool grounded;
    private bool jump;
    bool facingRight = true;
    private float hAxis;
    private Rigidbody2D theRigidBody;
    private Animator theAnimator;
    private bool attacking;


    void Start()
    {

        jump = false;
        grounded = false;

        theRigidBody = GetComponent<Rigidbody2D>();
        theAnimator = GetComponent<Animator>();

    }

    void Update()
    {

        jump = Input.GetKeyDown(KeyCode.Space);
        attacking = Input.GetKey (KeyCode.L);

        hAxis = Input.GetAxis("Horizontal");

        theAnimator.SetFloat("hspeed", Mathf.Abs(hAxis));

        Collider2D colliderWeCollidedWith = Physics2D.OverlapCircle(groundcheck.position, groundRadius, whatIsGround);

        grounded = (bool)colliderWeCollidedWith;

        theAnimator.SetBool("ground", grounded);

        float yVelocity = theRigidBody.velocity.y;
        theAnimator.SetFloat("vspeed", yVelocity);

        if (attacking)
        {
            theAnimator.SetBool("attack", true);
        }
        else
        {
            theAnimator.SetBool("attack", false);
        }


        if (grounded && attacking == false)
        {
            if ((hAxis > 0) && (facingRight == false))
            {
                Flip();
            }
            else if ((hAxis < 0) && (facingRight == true))
            {
                Flip();
            }
        }

    }
    void FixedUpdate()
    {
     
        if (grounded && !jump && attacking == false)
        {
            theRigidBody.velocity = new Vector2(horizontalSpeed * hAxis, theRigidBody.velocity.y);
        }
        else if (grounded && jump && attacking == false)
        {
            theRigidBody.velocity = new Vector2(theRigidBody.velocity.x, jumpSpeed);
        }


        if (theRigidBody.velocity.y < 0)
        {
            theRigidBody.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;

        }
        else if ((theRigidBody.velocity.y > 0) && (!Input.GetKey(KeyCode.Space)))
        {
            theRigidBody.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
        }

    }

    private void Flip()
    {
        if (attacking == false)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;
            transform.localScale = theScale;
        }

    }


}

