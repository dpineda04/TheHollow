using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    
    private float currentSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround; // this is for the drag, to make movement feel less slippery, only on the ground
    bool grounded;

    [Header("Input Settings")]
    public Transform orientation;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode interactKey = KeyCode.F; 


    private bool isSprinting;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;




    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentSpeed = moveSpeed;
        
    }

    // Update is called once per frame
    private void Update()
    {
        //checking if there is ground by casting ray from half of players height and a little bit more down
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.magenta);
        MyInput();

        //handling the drag:
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

       isSprinting = Input.GetKey(sprintKey);
     

       // handling sprint:
       HandleSprinting();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical"); // getting inputs
    }


    private void HandleSprinting()
    {
        if (isSprinting && grounded && moveDirection.magnitude > 0.1f) 
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }


    private void MovePlayer()
    {
        //calculating movement direction - you move where ur facing

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
    }

    //checking if shift is being held:
    public bool IsSprinting()
    { 
        return currentSpeed == sprintSpeed;
    }

}
