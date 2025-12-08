using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemProximityPickup : MonoBehaviour

{

    public Item item;
    public bool useDesignatedSlot = true;
    private PlayerMovement playerMovement;
    public CollectibleItem collectible;

  // Holds reference to PlayerMovement when player is in range
    

    //  Optional: show a simple UI prompt if you want later
    private void Start()
        {
        // Quick sanity warning:
            if (item == null)
            Debug.LogWarning($"[ItemProximityPickup] '{gameObject.name}' has no Item assigned. Assign an Item ScriptableObject in the Inspector.");
        }





    public void TryPickup()
    {
      
        Debug.Log($"[ItemProximityPickup] TryPickup called on '{gameObject.name}', item={item?.itemName ?? "NULL"}");


        if (Inventory.instance == null)
        {
            Debug.LogError("[ItemProximityPickup] Inventory.instance is null! Make sure an Inventory GameObject exists in the scene.");
            return;
        }

        if (item == null)
        {
            Debug.LogError("[ItemProximityPickup] No Item ScriptableObject assigned on this pickup. Assign it in the Inspector.");
            return;
        }

        bool added = false;

        if (useDesignatedSlot && item.designatedSlot >= 0)
        {
            added = Inventory.instance.Add(item, item.designatedSlot);
            
        }
       

        if (added)
        {
            Debug.Log($"[ItemProximityPickup] {item.name} was picked up!");
            Inventory.instance.PrintInventory();
            PickupHintUI.instance?.HideHint(gameObject);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[ItemProximityPickup] Could not pick up {item.itemName} (inventory full or slot occupied).");
        }
    }



    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // checking that its the player that is close
        {
           // Debug.Log($"{gameObject.name}: Player is near object!");
  
        }

        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null) {playerMovement = pm;}

        //Debug.Log($"Press F to pick up {item.itemName}");
         // Show hint: pass this.gameObject as the source
            PickupHintUI.instance?.ShowHint(gameObject, playerMovement.interactKey, item?.itemName ?? gameObject.name);
        
    }



    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
           // Debug.Log($"{gameObject.name}: Player has moved away from object.");
        }

         PickupHintUI.instance?.HideHint(gameObject);

        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null && pm == playerMovement) playerMovement = null;

       
        
     }



     private void Update()
    {
        if (playerMovement != null)
        {
            if (Input.GetKeyDown(playerMovement.interactKey))
            // should be : playerInRange.interactKey, testing with manual key
            {
                TryPickup();
            }
        }
    }


   
   
}
