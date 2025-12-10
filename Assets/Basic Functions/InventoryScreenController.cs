using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryScreenController : MonoBehaviour
{
    [Tooltip("Optional: assign your UIDocument. If null the script will try to find one in the scene.")]
    public UIDocument uiDocument;

    // Map item id -> slot VisualElement
    private Dictionary<int, VisualElement> idToSlot = new Dictionary<int, VisualElement>();

    private VisualElement root;
    private VisualElement inventoryRoot;

    // If you use ItemData ScriptableObjects, you can reference them here in the inspector
    public List<Item> itemsInOrder; // length 5, order matches slot_0..slot_4

    private bool inventoryVisible = false;

    void Start()
    {
        if (uiDocument == null)
            uiDocument = FindObjectOfType<UIDocument>();

        root = uiDocument.rootVisualElement;
        // try by name then by class (adjust if your element uses a different name)
        inventoryRoot = root.Q<VisualElement>("inventory-root") ?? root.Query<VisualElement>().Class("inventory-root").First();

        // Map each manual slot by name (slot_0..slot_4)
        for (int i = 0; i < itemsInOrder.Count; i++)
        {
            string slotName = "slot_" + i; // match the names you set in UI Builder
            var slot = inventoryRoot.Q<VisualElement>(slotName);
            if (slot == null)
            {
                Debug.LogWarning($"Slot '{slotName}' not found under inventory-root. Check names in UI Builder.");
                continue;
            }

            // find children
            var silhouette = slot.Q<VisualElement>("silhouette") ?? slot.Query<VisualElement>().Class("silhouette").First();
            var itemVE     = slot.Q<VisualElement>("item")       ?? slot.Query<VisualElement>().Class("item").First();

            // make sure sprites are assigned (you already did this in Builder, but we set them defensively)
            var item = itemsInOrder[i];
            if (item != null)
            {
                if (silhouette != null && item.icon != null)
                    silhouette.style.backgroundImage = new StyleBackground(item.silhouetteIcon.texture);
                if (itemVE != null && item.icon != null)
                    itemVE.style.backgroundImage = new StyleBackground(item.icon.texture);
            }

            // store mapping for runtime update when collected
            if (item != null)
                idToSlot[item.designatedSlot] = slot;
           
            // initialize state if the player already collected it
           if (item != null && Inventory.instance.slotOccupied[item.designatedSlot])
            {
                if (silhouette != null) silhouette.style.display = DisplayStyle.None;
                if (itemVE != null)      itemVE.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (silhouette != null) silhouette.style.display = DisplayStyle.Flex;
                if (itemVE != null)      itemVE.style.display = DisplayStyle.None;
            }
        }

        // Listen for items being collected
        if (Inventory.instance != null)
            Inventory.instance.OnItemCollected += HandleItemCollected;

        // Start hidden (optional) — hide inventory by default
        SetInventoryVisible(false);

        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnItemCollected += HandleItemCollected;
            Debug.Log("[UI] Subscribed to OnItemCollected");
        }
        else
        {
            Debug.LogWarning("[UI] Inventory.Instance is NULL at Start");
        }
    }

    void OnDestroy()
    {
        if (Inventory.instance != null)
            Inventory.instance.OnItemCollected -= HandleItemCollected;
    }

    void Update()
    {
        // Toggle inventory UI when pressing E
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetInventoryVisible(!inventoryVisible);
        }

        // optional: close with Escape
        if (inventoryVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            SetInventoryVisible(false);
        }

         
    }

    private void SetInventoryVisible(bool visible)
    {
        inventoryVisible = visible;

        // Option A: toggle the whole inventoryRoot display (keeps UIDocument active)
        if (inventoryRoot != null)
        {
            inventoryRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Option B (alt): toggle the entire UIDocument GameObject active/inactive
        // if (uiDocument != null && uiDocument.gameObject != null)
        //     uiDocument.gameObject.SetActive(visible);

        // optional: pause game or lock cursor when inventory is open
        // Time.timeScale = visible ? 0f : 1f;
        // Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        // Cursor.visible = visible;
    }

    private void HandleItemCollected(Item item)   // or ItemData item
    {
          Debug.Log("[UI] HandleItemCollected fired for item: " + item.name);
        // example using designatedSlot and slotOccupied
        int slotIndex = item.designatedSlot;
        Inventory.instance.slotOccupied[slotIndex] = true;

        if (!idToSlot.TryGetValue(slotIndex, out var slot)) return;
        var silhouette = slot.Q<VisualElement>("silhouette");
        var itemVE     = slot.Q<VisualElement>("item");

        if (silhouette != null) silhouette.style.display = DisplayStyle.None;
        if (itemVE != null)     itemVE.style.display = DisplayStyle.Flex;
    }

   
}