using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;      // assign in inspector or will fallback to Camera.main
    public Light flashlight;         // the light attached to player/camera to enable after pickup

    [Header("Pickup")]
    public float pickupRange = 3f;   // max pickup distance
    public KeyCode pickupKey = KeyCode.E;
    public LayerMask pickupLayers = ~0; // which layers raycast/overlap should consider (default = everything)

    private bool hasFlashlight = false;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (flashlight != null) flashlight.enabled = false;
    }

    void Update()
    {
        // Follow camera when held
        if (hasFlashlight && flashlight != null && playerCamera != null)
        {
            flashlight.transform.position = playerCamera.transform.position;
            flashlight.transform.rotation = playerCamera.transform.rotation;
        }

        if (Input.GetKeyDown(pickupKey) && !hasFlashlight)
        {
            TryPickupFlashlight();
        }
    }

    void TryPickupFlashlight()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("[FlashlightPickup] No playerCamera assigned and Camera.main is null.");
            return;
        }

        // 1) Primary: Raycast from camera forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        bool rayHit = Physics.Raycast(ray, out hit, pickupRange, pickupLayers, QueryTriggerInteraction.Collide);

        if (rayHit)
        {
            Debug.Log("[FlashlightPickup] Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Flashlight"))
            {
                Pickup(hit.collider.gameObject);
                return;
            }
            
        }
        

        // 2) Fallback: proximity check (find nearest tagged flashlight within range)
        Collider[] cols = Physics.OverlapSphere(playerCamera.transform.position, pickupRange, pickupLayers, QueryTriggerInteraction.Collide);
        float nearestDist = float.MaxValue;
        GameObject nearest = null;
        foreach (var c in cols)
        {
            if (c.CompareTag("Flashlight"))
            {
                float d = Vector3.Distance(playerCamera.transform.position, c.transform.position);
                if (d < nearestDist) { nearestDist = d; nearest = c.gameObject; }
            }
        }

        if (nearest != null)
        {
            Debug.Log("[FlashlightPickup] Found nearby flashlight via OverlapSphere: " + nearest.name + " at distance " + nearestDist);
            Pickup(nearest);
            return;
        }

        Debug.Log("[FlashlightPickup] No flashlight to pick up. Ensure the object is tagged 'Flashlight', has a collider, and is within " + pickupRange + " units.");
    }

    void Pickup(GameObject worldFlashlight)
    {
        hasFlashlight = true;

        // Enable player's flashlight (if assigned)
        if (flashlight != null)
        {
            flashlight.enabled = true;
        }
        else
        {
            Debug.LogWarning("[FlashlightPickup] No 'flashlight' Light assigned in inspector.");
        }

        // Destroy the world model (optionally use SetActive(false) if you prefer)
        Destroy(worldFlashlight);
        Debug.Log("[FlashlightPickup] Picked up flashlight and destroyed world object: " + worldFlashlight.name);
    }

    // visualize pickup radius in Scene view
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.transform.position, pickupRange);
        }
    }
}
