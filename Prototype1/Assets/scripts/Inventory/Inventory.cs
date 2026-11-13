using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public FPController playerController;

    public GameObject inventoryPanel; // Assign your UI panel here

    public GameObject hotBarObject;
    public GameObject InventorySlotParent;

    public Image dragIcon;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotBarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot dragSlot = null;
    private bool isDragging = false;

    private void Awake()
    {
        inventorySlots.AddRange(InventorySlotParent.GetComponentsInChildren<Slot>());
        hotBarSlots.AddRange(hotBarObject.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotBarSlots);
    } 
    
    void Update()
    {
        // Always listen for the 'I' key to toggle inventory open/closed
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        // Only process dragging when the inventory panel is open
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            StartDrag();
            UpdateDragItemPosition();
            EndDrag();
        }
    }

   public void ToggleInventory()
{
    if (inventoryPanel == null) return;

    // Force the opposite state explicitly
    bool shouldBeActive = !inventoryPanel.activeSelf;
    inventoryPanel.SetActive(shouldBeActive);

    // Tell the FPController to update cursor visibility and lock state
    if (playerController != null)
    {
        playerController.ToggleCursor(shouldBeActive);
    }
}

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach(Slot slot in allSlots)
        {
            if(slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if(currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if(remaining <= 0)
                    return;
                }
            }
        }

        foreach(Slot slot in allSlots)
        {
            if(!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;
                if(remaining <= 0)
                return;
            }
        }

        if(remaining > 0)
        {
            Debug.Log("Inventory is full, could not add " + remaining + " of " + itemToAdd.itemName);
        }
    }

    private void StartDrag()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Slot hovered = GetHoveredSlot();

            if(hovered != null && hovered.HasItem())
            {
                dragSlot = hovered;
                isDragging = true;

                // Show drag item
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Mouse.current != null && (Mouse.current.leftButton.wasReleasedThisFrame || !Mouse.current.leftButton.isPressed) && isDragging)
        {
            Slot hovered = GetHoveredSlot();

            if(hovered != null)
            {
                HandleDrop(dragSlot, hovered);
            }

            if (dragIcon != null)
            {
                dragIcon.enabled = false;
            }
            
            dragSlot = null;
            isDragging = false;
        }
    }

    private Slot GetHoveredSlot()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            var pointerEventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerEventData.position = Mouse.current.position.ReadValue();

            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                Slot slot = result.gameObject.GetComponentInParent<Slot>();
                if (slot != null && allSlots.Contains(slot))
                {
                    return slot;
                }
            }
        }

        return null;
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if(from == to) return;
    
        // Stacking
        if(to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if(space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if(from.GetAmount() <= 0)
                    from.ClearSlot();

                return;
            }
        } 

        // Different item
        if(to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        // Empty slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging && Mouse.current != null && dragIcon != null)
        {
            RectTransform canvasRect = dragIcon.canvas.GetComponent<RectTransform>();
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                Mouse.current.position.ReadValue(), 
                dragIcon.canvas.worldCamera, 
                out localPoint))
            {
                dragIcon.rectTransform.anchoredPosition = localPoint;
            }
        }
    }
}