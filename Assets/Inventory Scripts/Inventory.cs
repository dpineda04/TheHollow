using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory instance; //only one inventory will exist

    public int slotCount = 4;

    public Item[] items; //fixed-size inventory (curr have 5 objects)
    public bool[] slotOccupied; //tracking used slots

    public delegate void OnInventoryChanged();
    //delegate is a function pointer, calling methods indirectly

    public OnInventoryChanged onInventoryChangedCallback;
    //this variable can hold one or more functions of this type.
    //it can get called whenevr the inventory changes.

    void Awake()

    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        items = new Item[slotCount];
        slotOccupied = new bool[slotCount];

    }

    public bool Add(Item newItem, int slotIndex)
    {
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

        //*** in here, place any calls into outer functions or UI, like destroying object
        // i want to destroy the object
        //then at some point, have a bool to see if the list is full, if it is:
            //then start the final sequence. 

        

        return true;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
