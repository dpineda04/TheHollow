using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
   [Header("References")]
    public Inventory inventory;
    public GameObject slotPrefab;         // prefab with InventorySlot component
    public Transform slotParent;          // GridLayoutGroup or similar parent

    [Header("Events")]
    // Use this in the Inspector to attach other systems (e.g. MonsterSpawner.SpawnMonster)
 

    private List<InventorySlot> uiSlots = new List<InventorySlot>();

    void Awake()
    {
        // Build slots early if inventory assigned (helps ensure UI is ready before other Start methods)
        if (inventory != null)
            BuildSlots();
    }

    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("InventoryUI: Inventory reference is missing on " + gameObject.name);
            return;
        }

        // Subscribe to inventory change events (guard against null / missing event implementation)
        // Note: your Inventory class should expose an Action/event called 'onInventoryChanged' and a way to notify when full.
        inventory.onInventoryChanged += RefreshUI;

        // If your Inventory exposes an event like 'OnInventoryFull' or 'onInventoryFull', have it call HandleInventoryFull()
        // e.g.: inventory.OnInventoryFull += HandleInventoryFull;
        // We don't subscribe here to a name that might not exist; call HandleInventoryFull from the Inventory when it's full.

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.onInventoryChanged -= RefreshUI;
    }

    private void BuildSlots()
    {
        if (inventory == null)
        {
            Debug.LogWarning("InventoryUI.BuildSlots: inventory is null.");
            return;
        }

        // clear existing
        if (slotParent != null)
        {
            foreach (Transform t in slotParent) Destroy(t.gameObject);
        }

        uiSlots.Clear();

        for (int i = 0; i < inventory.slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var slot = go.GetComponent<InventorySlot>();
            if (slot == null)
            {
                Debug.LogError("slotPrefab must have InventorySlot component.");
                Destroy(go);
                continue;
            }
            slot.Initialize(i, this);
            uiSlots.Add(slot);
        }
    }

    public void RefreshUI()
    {
        if (inventory == null) return;

        // If the inventory size changed, rebuild UI slots
        if (uiSlots.Count != inventory.slotCount)
        {
            BuildSlots();
        }

        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].Bind(inventory.GetItem(i));
        }
    }

    /// <summary>
    /// Call this when the inventory becomes full.
    /// You can either:
    ///  - have your Inventory script invoke InventoryUI.HandleInventoryFull() directly when it detects full,
    ///  - or subscribe InventoryUI.HandleInventoryFull to your Inventory's OnInventoryFull event:
    ///        inventory.OnInventoryFull += inventoryUI.HandleInventoryFull;
    /// </summary>
    public void HandleInventoryFull()
    {
      
    }
}
