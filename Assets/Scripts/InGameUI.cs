using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI cashText;
    public GameObject[] InventoryIcons;
    public TextMeshProUGUI[] InventoryDisplay;

    void Start()
    {
        GridManager.OnCashUpdated.AddListener(UpdateCashText);
        CharacterController.OnInventoryUpdated.AddListener(UpdateInventoryDisplay);
    }

    //Update the Cash Amount
    private void UpdateCashText(float amount)
    {
        cashText.text = $"Cash: ${amount:F2}";
    }

    private void UpdateInventoryDisplay(int[] PlayerInventory)
    {
        for (int i = 0; i < PlayerInventory.Length; i++)
        {
            InventoryDisplay[i].text = PlayerInventory[i].ToString();
        }
    }
}
