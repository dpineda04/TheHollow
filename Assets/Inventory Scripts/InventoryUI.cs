using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;
    public GameObject slotPrefab;         // prefab with InventorySlot component
    public Transform slotParent;          // GridLayoutGroup or similar parent

    private List<InventorySlot> uiSlots = new List<InventorySlot>();

    
    // Start is called before the first frame update
    void Start()
    {
         // create UI slots to match inventory.slotCount
        BuildSlots();
        // subscribe to inventory changes
        inventory.onInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.onInventoryChanged -= RefreshUI;
    }


    private void BuildSlots()
    {
        // clear existing
        foreach (Transform t in slotParent) Destroy(t.gameObject);
        uiSlots.Clear();

        for (int i = 0; i < inventory.slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var slot = go.GetComponent<InventorySlot>();
            if (slot == null)
            {
                Debug.LogError("slotPrefab must have InventorySlot component.");
                continue;
            }
            slot.Initialize(i, this);
            uiSlots.Add(slot);
        }
    }
    
    public void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].Bind(inventory.GetItem(i));
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
