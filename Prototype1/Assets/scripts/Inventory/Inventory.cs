using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{

   public ItemSO policeReportItem;
   public ItemSO letterItem;

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
       if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                AddItem(letterItem, 1);
            } 
            else if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                AddItem(policeReportItem, 1);
            }
        }

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();
    }

   public void ToggleInventory()
   {
     // Toggle the panel visibility
     bool isActive = !inventoryPanel.activeSelf;
     inventoryPanel.SetActive(isActive);

     // This is where you put the line!
     playerController.ToggleCursor(isActive);
     if (playerController != null)
     {
       playerController.ToggleCursor(isActive);
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

    if(remaining >0)
    {
        Debug.Log("Inventory is full, could not add" + remaining + "of" + itemToAdd.itemName);
    }
   }

   private void StartDrag()
   {
    if(Input.GetMouseButtonDown(0))
    {
        Slot hovered = GetHoveredSlot();

        if(hovered != null && hovered.HasItem())
        {
            dragSlot = hovered;
            isDragging = true;

            //Show drag item
            dragIcon.sprite = hovered.GetItem().icon;
            dragIcon.color = new Color(1, 1, 0.5f);
            dragIcon.enabled = true;
        }
    }
   }

   private void EndDrag()
   {
    if(Input.GetMouseButtonUp(0) && isDragging)
    {
      Slot hovered = GetHoveredSlot();

      if(hovered != null)
      {
        HandleDrop(dragSlot, hovered);

        dragIcon.enabled = false;
        
        dragSlot = null;
        isDragging = false;
      }
    }
   }

   private Slot GetHoveredSlot()
   {
     foreach(Slot s in allSlots)
     {
        if(s.hovering)
            return s;
     }

     return null;
   }

   private void HandleDrop(Slot from, Slot to)
   {
     if(from == to) return;
    
    //Stacking
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

     //Different item
     if(to.HasItem())
     {
        ItemSO tempItem = to.GetItem();
        int tempAmount = to.GetAmount();

        to.SetItem(from.GetItem(), from.GetAmount());
        from.SetItem(tempItem, tempAmount);
        return;
     }

     //Empty slot
     to.SetItem(from.GetItem(), from.GetAmount());
     from.ClearSlot();
   }

   private void UpdateDragItemPosition()
   {
    if(isDragging)
    {
        dragIcon.transform.position = Input.mousePosition;
    }
   }

   
}
