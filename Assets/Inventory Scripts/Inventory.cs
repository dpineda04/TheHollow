using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory instance; //only one inventory will exist

    [Header("Inventory Settings")]
    public int slotCount = 1;

    [Tooltip("Fixed-size inventory array")]
    public Item[] items; //fixed-size inventory (curr have 5 objects)
    [HideInInspector]
    public bool[] slotOccupied; //tracking used slots

    public event Action onInventoryChanged;
//event that ui and other systems can subscribe to

    public event Action OnInventoryFull;          // others (spawner) can subscribe to this

    private bool hasFiredFullEvent = false;      // ensure full event only fires once until inventory no longer full


    private void Awake()

    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //initlaize arrays 
        items = new Item[slotCount];
        slotOccupied = new bool[slotCount];
    }
    
    #region Basic API

    public bool Add(Item newItem, int slotIndex)
    {
       
       if (newItem == null)
        {
            Debug.LogWarning("Tried to add a null item.");
            return false;
        }


        //check that slot index is in range
        if (slotIndex < 0 || slotIndex >= slotCount)
        {
            Debug.LogWarning("Invalid slot index!");
            return false;
        }

        //if slot is occupied

        if (slotOccupied[slotIndex])
        {
            Debug.Log("Slot " + slotIndex + " is occupied" );
            return false;
        }

        //if slot idx in range & empty, now we place item into slot

        items[slotIndex] = newItem;
        slotOccupied[slotIndex] = true;
        onInventoryChanged?.Invoke();
        //Debug.Log("Item added. Item: " + newItem + ". slot index: " + slotIndex);

        //*** in here, place any calls into outer functions or UI, like destroying object
        // i want to destroy the object
        //then at some point, have a bool to see if the list is full, if it is:
            //then start the final sequence. 

          // --- check for full and raise event once ---
        if (IsFull() && !hasFiredFullEvent)
        {
            hasFiredFullEvent = true;
            Debug.Log("[Inventory] Inventory is full — raising OnInventoryFull.");
            OnInventoryFull?.Invoke();
        }

        return true;
    }


    public Item GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return null;
        return items[slotIndex];
    }




      public bool IsFull()
    {
        for (int i = 0; i < slotCount; i++)
            if (!slotOccupied[i]) return false;
        return true;
    }


     public void PrintInventory()
    {
        Debug.Log("=== Inventory Contents ===");

        for (int i = 0; i < items.Length; i++)
        {
            if (slotOccupied[i] && items[i] != null)
                Debug.Log($"Slot {i}: {items[i].itemName}");
            else
                Debug.Log($"Slot {i}: [Empty]");
        }
}

    #endregion
}
