using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // assign the SO in inspector
    [Tooltip("If true and item.designatedSlot >= 0 then attempt that slot. Otherwise first available slot.")]
    public bool useDesignatedSlot = true;

    // You can hook this to player's pickup event or OnTriggerEnter
    // public void TryPickup(Collider other)
    // {
    //     // naive check: player has Inventory instance
    //     if (Inventory.instance == null) return;

    //     bool added = false;
    //     if (useDesignatedSlot && item != null && item.designatedSlot >= 0)
    //     {
    //         added = Inventory.instance.Add(item, item.designatedSlot);
    //     }
        
    //     if (added)
    //     {
    //         // destroy the pickup — placeholder behaviour
    //         Destroy(gameObject);
    //         Debug.Log("Object added! Slot array: " + Inventory.instance.items);
    //     }
    //     else
    //     {
    //         // inventory full or slot occupied — provide feedback (SFX/UI) here
    //         Debug.Log("Could not pick up item; inventory full or slot occupied.");
    //     }
    // }
}