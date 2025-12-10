using UnityEngine;
using UnityEngine.UIElements;

public class InventoryToggle : MonoBehaviour
{
    public enum ToggleMode { ToggleElement, ToggleDocument }
    [Tooltip("If null, script will try to find a UIDocument in the scene.")]
    public UIDocument uiDocument;
    [Tooltip("Name of the VisualElement that contains the inventory UI inside the UIDocument (use the element Name from UI Builder).")]
    public string inventoryElementName = "inventory-root";
    [Tooltip("Which method to use to show/hide inventory. Use ToggleDocument if style/display isn't cooperating.")]
    public ToggleMode toggleMode = ToggleMode.ToggleElement;

    private VisualElement root;
    private VisualElement inventoryRoot;
    private bool inventoryVisible = false;

    void Awake()
    {
        if (uiDocument == null)
        {
            uiDocument = FindObjectOfType<UIDocument>();
            Debug.Log("[InventoryToggle] Auto-found UIDocument: " + (uiDocument != null));
        }

        if (uiDocument == null)
        {
            Debug.LogError("[InventoryToggle] No UIDocument found. Assign one in the inspector.");
            enabled = false;
            return;
        }

        root = uiDocument.rootVisualElement;
        inventoryRoot = root.Q<VisualElement>(inventoryElementName);

        if (inventoryRoot == null)
        {
            // Try class fallback
            inventoryRoot = root.Query<VisualElement>().Class(inventoryElementName).First();
            if (inventoryRoot != null)
                Debug.Log("[InventoryToggle] Found inventoryRoot by class name as fallback.");
        }

        if (inventoryRoot == null)
        {
            Debug.LogError($"[InventoryToggle] Could not find element named '{inventoryElementName}' in UIDocument. Make sure the element Name in UI Builder matches exactly.");
            // still keep script enabled in case ToggleDocument will be used
        }
        else
        {
            // initialize visibility based on current style
            inventoryVisible = inventoryRoot.style.display == DisplayStyle.Flex;
            Debug.Log($"[InventoryToggle] Found inventoryRoot. initial display = {inventoryRoot.style.display}. inventoryVisible = {inventoryVisible}");
        }
    }

    void Update()
    {
        // Quick debug: show that Update runs
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[InventoryToggle] E pressed - toggling inventory");
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (toggleMode == ToggleMode.ToggleElement)
        {
            if (inventoryRoot == null)
            {
                Debug.LogWarning("[InventoryToggle] inventoryRoot is null, cannot toggle element. Switching to ToggleDocument mode.");
                ToggleModeDocument();
                return;
            }

            inventoryVisible = !inventoryVisible;
            inventoryRoot.style.display = inventoryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            Debug.Log($"[InventoryToggle] inventoryRoot.style.display set to {(inventoryVisible ? "Flex" : "None")}");
        }
        else // ToggleDocument
        {
            ToggleModeDocument();
        }
    }

    private void ToggleModeDocument()
    {
        if (uiDocument == null)
        {
            Debug.LogError("[InventoryToggle] No UIDocument to toggle.");
            return;
        }

        inventoryVisible = !inventoryVisible;
        uiDocument.gameObject.SetActive(inventoryVisible);
        Debug.Log($"[InventoryToggle] UIDocument GameObject.SetActive({inventoryVisible})");
    }
}