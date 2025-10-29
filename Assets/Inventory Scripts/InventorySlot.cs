using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public Button removeButton;
    public int slotIndex { get; private set; }
    private InventoryUI parentUI;

    // call once from InventoryUI when the slot prefab is instantiated
    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        parentUI = ui;
        if (removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
           
        }
    }

    // update visuals
    public void Bind(Item item)
    {
        if (item != null)
        {
            if (iconImage) iconImage.sprite = item.icon;
            gameObject.SetActive(true);
        }
        else
        {
            if (iconImage) iconImage.sprite = null;
            // we keep the slot visible (empty) so layout doesn't break
        }
    }
}