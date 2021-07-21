using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player components to allow movement
    private Rigidbody2D rigbod; //can interact with environment
    public float speed;//how fast run
    private float moveInput;//detects which way player wants to move (which arrow key)
    private float moveInputVertical;

    //will help with jump mechanic
    private bool isGrounded;//to see if palyer can jump or not
    public Transform feetPos;//helps see if feet touch ground or not
    public float checkRadius;//helps check if feet touch ground or not
    public LayerMask whatIsGround;//sees what is ground(are we jumping or not)

    //make jump more believable
    public float jumpForce;//how high we jump
    private float jumpTimeCounter;//so we don't jump into space
    public float jumpTime;//helps limit jumpforce to not jump into space
    private bool isJumping;//prevents double jumps that stay in midair

    //reference to animator component attached to player character
    private Animator anm;

    //for ladder
    public float distance;
    public LayerMask whatIsLadder;
    public bool isClimbing;

    // Start is called before the first frame update 
    void Start()
    {
        anm = GetComponent<Animator>();//attach animator component to player here
        rigbod = GetComponent<Rigidbody2D>();//attach the rigid body attached to plaeyer to var here
    }

    // Update is called once per frame //for all physics stuff
    void FixedUpdate(){
        moveInput = Input.GetAxisRaw("Horizontal");//left arrow is -1 //right arrow is 1
        rigbod.velocity = new Vector2(moveInput * speed, rigbod.velocity.y);//this part is speed of player
    
        //helps detect where ladder is
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);//startPos,direction,length
    
        //----ladder----//
        /*how the ladder will work: player has ray shooting out to top, if ray collides with ladder, then
        we can climb up but gravity set to 0 on ladder*/

        //has the ray hit the ladder?
        if(hitInfo.collider != null){
            //has the player pressed the up arrow
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                anm.SetBool("checkClimbing",false);
                //climb ladder
                isClimbing = true;
            }
        }
        else{
            anm.SetBool("checkClimbing",false);
            //no ray coolied with ladder
            isClimbing = false;
        }

        if(isClimbing == true){
            moveInputVertical = Input.GetAxisRaw("Vertical");
            rigbod.velocity = new Vector2(rigbod.position.x, moveInputVertical * speed);//you can make a separate climb speed if you want diff from run speed too
            rigbod.gravityScale = 0;
        }
        else{
            rigbod.gravityScale = 4;
        }
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);//determines what ground is

        //-----Running animation----//
        if(moveInput == 0){
            //idle
            anm.SetBool("isRunning", false);
        }
        //if player runs left, rotate sprite left
        else if(moveInput > 0){
             anm.SetBool("isRunning", true);
            //player moves right
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else if(moveInput < 0){
             anm.SetBool("isRunning", true);
             transform.eulerAngles = new Vector3(0,180,0);
        }

        //---------jumps--------------//
        if(isGrounded == true){//we on ground but no jump key pressed.
             anm.SetBool("isJumping", false);
             //we're on the ground and player presses jump key
             if(Input.GetKeyDown(KeyCode.Space)){
                 //so isGround is only true if the inisible circle layer mask collides with some ground
                isJumping = true;
                jumpTimeCounter = jumpTime;//counter is default jump time
                anm.SetTrigger("takeOff");//takeoff animation

                //we jump if ground and jump key pressed
                rigbod.velocity = Vector2.up * jumpForce;
             }
        }
        else{//in air so go from takeoff to jump animation
            anm.SetBool("isJumping", true);
        }

        //----higher jumps
        if((Input.GetKey(KeyCode.Space)) && isJumping == true){
            if(jumpTimeCounter > 0){
                 rigbod.velocity = Vector2.up * jumpForce;
                 jumpTimeCounter -= Time.deltaTime;//so we eventually get not true with jumpt time counter stuff
            }
            else{//if isJumping <=0
                isJumping = false;
            }
            
        }

        if(Input.GetKeyUp(KeyCode.Space)){
            isJumping = false;
        }

    }
}
