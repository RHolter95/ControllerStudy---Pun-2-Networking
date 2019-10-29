﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace UnderdogCity
{
public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public FixedJoystick joystickOne;
    public FixedJoystick joystickTwo;

	public Texture2D crosshair;   
    private bool showMessage = false;
    private CharacterController CharacterController;

    [SerializeField]
    private Animator animator;
    private bool isSpacePressed = false;

    //Used to make player Jump
    [SerializeField]
    public float jumpSpeed = 5.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;

    

    [SerializeField]
    //private float turnSpeed = 5f;

     public Camera Camera;
     [SerializeField]
    float walkSpeed = 10.0f;
     [SerializeField]
    float walkBackSpeed = 0.5f;
    [SerializeField] 
    float strafeSpeed = 2f;
    [SerializeField]
    float rotateSpeed = 10.0f;
    [SerializeField]
    private float jumpHeight = 5.5f;
    private bool isGrounded = true;
    float verticalVelocity = 0;
    protected GameObject temp;

    


    void Start()
    {
     temp = GameObject.Find("Character@Idle");
     animator = temp.GetComponentInChildren<Animator>();

     //Raycast isGrounded returns false @ spawn 
     //so manually set it for first frame
     animator.SetBool("IsGrounded",true);

     var collider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    private void Awake()
    { 
        
        //Find the joystick objects
        GameObject tempJoystickOne = this.gameObject.transform.GetChild(3).GetChild(0).gameObject;
        GameObject tempJoystickOTwo = this.gameObject.transform.GetChild(3).GetChild(1).gameObject;

        //Assign Joysticks to script variables [Left,Right]
        if(tempJoystickOne)
        joystickOne = tempJoystickOne.GetComponent<FixedJoystick>();

        if(tempJoystickOTwo)
        joystickTwo = tempJoystickOTwo.GetComponent<FixedJoystick>();

        CharacterController = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {      
        if(Physics.Raycast(transform.position, -Vector3.up, 0.05f) == true)
        {
            isGrounded = true;
        }else{
            isGrounded = false;
        }


        var horizontalOne = joystickOne.Horizontal;
        var verticalOne = joystickOne.Vertical;

        var horizontalTwo = joystickTwo.Horizontal;
        var verticalTwo = joystickTwo.Vertical;

        var moveDirection = new Vector3(horizontalOne, 0f, verticalOne);
        var lookDirection = new Vector3(0f, verticalTwo, 0f);


        if(moveDirection.magnitude > 0.1f)
        {
            moveDirection = moveDirection.normalized;
        }
           
        //This translates location from joystick Left
        transform.Rotate(0, moveDirection.y, 0, Space.Self);
        transform.Rotate(joystickTwo.Horizontal * Vector3.up * Time.deltaTime * rotateSpeed);

        animator.SetFloat("Speed",moveDirection.magnitude);
        animator.SetFloat("JoyStickX",joystickOne.Horizontal);
        animator.SetFloat("JoyStickY",joystickOne.Vertical);

       if(isGrounded && Input.GetButtonDown("Jump"))
       {
           verticalVelocity = jumpSpeed;
       }
        
        //If were walking backwards
        if(joystickOne.Direction.y < -.1f){
        transform.Translate(joystickOne.Horizontal * walkBackSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
        }
        //Limit side to side strafe speed
        else if(joystickOne.Direction.x < -.1f && joystickOne.Direction.y < -.1f  || joystickOne.Direction.x > .1f && joystickOne.Direction.y < -.1f){
        transform.Translate(joystickOne.Horizontal * strafeSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
        }else{
        //Just move if not strafing or walking backwards
        transform.Translate(joystickOne.Horizontal * walkSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkSpeed * Time.deltaTime);
        }
    }
    
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
					      		 Screen.height / 2 - (crosshair.height * 0.5f),
								 crosshair.width, crosshair.height), crosshair);

        //Use this for showing weapon Image
		if(showMessage == true)
		{
        //GUI.Label(new Rect(Screen.width/2-100, Screen.height - 375 , 200, 200), "Hello World!");
		//GUI.Label(new Rect(Screen.width/2-100, Screen.height - 375 , 500, 250), textureToDisplay);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    void FixedUpdate()
    {       
        Vector3 dist = moveDirection * walkSpeed * Time.deltaTime;
        if(isGrounded && verticalVelocity < 0)
        {
            animator.SetBool("IsGrounded", true);
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }else{
            if(Mathf.Abs(verticalVelocity) > jumpSpeed * 0.75f)
            {
                animator.SetBool("IsGrounded", false);
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        dist.y = verticalVelocity * Time.deltaTime;

        CharacterController.Move(dist/2);
    }


}
}
