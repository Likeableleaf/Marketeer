using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI cashText;
    public GameObject[] InventoryIcons;
    public TextMeshProUGUI[] InventoryDisplay;
    //public int StrikeCounter = 0;  //used to test the strike counter
    public GameObject[] strikes;
    

    void Start()
    {
        GridManager.OnCashUpdated.AddListener(UpdateCashText);
        CharacterController.OnInventoryUpdated.AddListener(UpdateInventoryDisplay);
        Manager.OnStrikeUpdated.AddListener(UpdateStrikeDisplay);
    }

    //used to test the strike counter
    /*
    private void Update()
    {
        UpdateStrikeDisplay(StrikeCounter);
    }
    */

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

    private void UpdateStrikeDisplay(int count)
    {
        Debug.Log("Counter just went up!: " + count);

        if(count == 0)
        {
            strikes[0].SetActive(true);
            strikes[1].SetActive(true);
            strikes[2].SetActive(true);
        }

        if(count == 1 )
        {
           strikes[0].SetActive(false);
        }
        else
        if (count == 2 )
        {
            strikes[0].SetActive(false);
            strikes[1].SetActive(false);
        }
        else
        if (count >= 3 )
        {
            strikes[0].SetActive(false);
            strikes[1].SetActive(false);
            strikes[2].SetActive(false);
        }
        
        //TODO: implement the update to display for number of stikes
    }
}
