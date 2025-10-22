using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public int targetSlotIndex; // the slot that the item belongs to

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
