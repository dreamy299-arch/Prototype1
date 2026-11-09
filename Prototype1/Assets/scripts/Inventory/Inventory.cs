using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class Inventory : MonoBehaviour
{

   public ItemSO policeReportItem;
   public ItemSO letterItem;

   
   public GameObject hotBarObject;
   public GameObject InventorySlotParent;

   private List<Slot> inventorySlots = new List<Slot>();
   private List<Slot> hotBarSlots = new List<Slot>();
   private List<Slot> allSlots = new List<Slot>();

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
}
