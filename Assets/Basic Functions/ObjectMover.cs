using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float moveSpeed = 5f;   // Speed for keyboard movement
    private Camera mainCamera;
    private bool isDragging = false;
    public ChangeColor objectColor;



    void Start()
    {
        mainCamera = Camera.main;
        
       
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleMouseMovement();
    }

    void HandleKeyboardMovement()
    {
        // Get input from arrow keys or WASD
        float moveX = Input.GetAxis("Horizontal");  // Left/Right or A/D
        float moveZ = Input.GetAxis("Vertical");    // Up/Down or W/S

        // Apply movement
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    void HandleMouseMovement()
    {
        // When left mouse button pressed, start dragging
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        // When left mouse button released, stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
           
        }

        // If dragging, move object toward mouse position
        if (isDragging)
        {
           
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            objectColor.ChangeObjectColor(Color.red);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);
                transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * moveSpeed);
            }
        }
    }
}
