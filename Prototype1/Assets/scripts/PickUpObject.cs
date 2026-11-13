using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [Header("Item Data")]
    public ItemSO itemData; // Drag your Letter, Police Report, or any of the 9 items here!
    public int amount = 1;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when picking up an item into the inventory
    public void PickUp(Transform holdPoint = null)
    {
        // 1. Send item to your inventory system
        Inventory inventory = FindFirstObjectByType<Inventory>();
        if (inventory != null && itemData != null)
        {
            inventory.AddItem(itemData, amount);
        }

        // 2. Destroy the world object
        Destroy(gameObject);
    }

    // Optional physics methods if you still use them for other non-inventory objects
    public void PickUpPhysics(Transform holdPoint)
    {
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        if (rb != null) rb.useGravity = true;
        transform.SetParent(null);
    }

    public void MoveToHoldPoint(Vector3 targetPosition)
    {
        if (rb != null) rb.MovePosition(targetPosition);
    }

    public void Throw(Vector3 impulse)
    {
        transform.SetParent(null);
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(impulse, ForceMode.Impulse);
        }
    }
}