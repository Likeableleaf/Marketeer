using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterInterface : MonoBehaviour
{
    // Variable Cache
    [SerializeField] private CharacterController Player;
    [SerializeField] private GameObject[] InventoryIcons;
    [SerializeField] private TextMeshProUGUI[] InventoryDisplay;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        DisplayInventory();
    }

    private void DisplayInventory() {
        //TODO display icons on left of screen with coresponding number from player based on grocery type and player inventory
        for (int i = 0; i < Player.Inventory.Length; i++) {
            //Display Icon(s) of inventory
            //if (Player.Inventory[i] != 0) {
                InventoryDisplay[i].text = Player.Inventory[i].ToString();
            //} 
        }
    }
}
