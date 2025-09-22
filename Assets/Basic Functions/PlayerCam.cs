using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX; // x and y sensitivity
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;





    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // making sure cursor is locked in the center and invisible
    }

    // Update is called once per frame
    void Update()
    {
        //getting mouse imput:
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY; //this is how unity handles rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // stops going up at 90 degrees

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }
}
