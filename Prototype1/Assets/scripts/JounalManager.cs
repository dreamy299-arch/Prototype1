using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    [Header("UI References")]
    public GameObject journalUIPanel;
    public TextMeshProUGUI journalTextView;

    [Header("Inventory Grid Settings")]
    public Transform inventoryContainer;
    public List<GameObject> inventorySlots;

    [Header("Inventory Data")]
    public List<string> collectedJournals = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (journalUIPanel != null)
            journalUIPanel.SetActive(false);
            
        if (inventoryContainer != null)
            inventoryContainer.gameObject.SetActive(false);
    }

    public void CollectAndOpenJournal(string journalText)
    {
        if (!collectedJournals.Contains(journalText))
        {
            collectedJournals.Add(journalText);
            UpdateInventoryUI();
        }

        OpenJournal(journalText);
    }

    public void OpenJournal(string journalText)
    {
        if (journalTextView != null)
            journalTextView.text = journalText;

        if (journalUIPanel != null)
            journalUIPanel.SetActive(true);
    }

    public void CloseJournal()
{
    if (journalUIPanel != null)
        journalUIPanel.SetActive(false);

    if (inventoryContainer != null)
        inventoryContainer.gameObject.SetActive(false);

    // Find the player controller and reset its inventory flag
    FPController player = FindFirstObjectByType<FPController>();
    if (player != null)
    {
        player.isInventoryOpen = false;
    }

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}   

    public void ToggleInventory()
{
    if (inventoryContainer != null)
    {
        bool isActive = inventoryContainer.gameObject.activeSelf;
        inventoryContainer.gameObject.SetActive(!isActive);

        if (!isActive)
        {
            UpdateInventoryUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

    public void UpdateInventoryUI()
    {
        if (inventorySlots.Count == 0 && inventoryContainer != null)
        {
            inventorySlots.Clear();
            foreach (Transform child in inventoryContainer)
            {
                inventorySlots.Add(child.gameObject);
            }
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject slot = inventorySlots[i];
            TextMeshProUGUI slotText = slot.GetComponentInChildren<TextMeshProUGUI>();
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton == null) slotButton = slot.GetComponentInChildren<Button>();

            if (i < collectedJournals.Count)
            {
                string entry = collectedJournals[i];
                slot.SetActive(true);

                if (slotText != null)
                    slotText.text = entry.Length > 15 ? entry.Substring(0, 15) + "..." : entry;

                if (slotButton != null)
                {
                    slotButton.onClick.RemoveAllListeners();
                    string cachedEntry = entry;
                    slotButton.onClick.AddListener(() => {
                        OpenJournal(cachedEntry);
                    });
                }
            }
            else
            {
                if (slotText != null) slotText.text = "";
                if (slotButton != null) slotButton.onClick.RemoveAllListeners();
            }
        }
    }
}