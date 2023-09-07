using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ShopperList : MonoBehaviour
{
    // Variable Cache
    public GameObject CurrentTarget;

    private static GameObject[] StoreShelves;
    private List<GameObject> ShoppingList = new List<GameObject>();
    private int ShoppingListSize;
    private static GameObject Register;

    // Start is called before the first frame update
    void Awake() {

        // Decide How many Items to Shop for
        ShoppingListSize = Random.Range(1,6);

        // Decide the Items to Shop for
        for(int i = 0; i < ShoppingListSize; i++) {
            ShoppingList.Add(StoreShelves[Random.Range(1,6)]);
        }
    }

    // Update is called once per frame
    void Update() {
        
        // Decide if finished 
        if (ShoppingList.Count > 0) {
            // Go to First item in List
            CurrentTarget = ShoppingList[0];

            // Delete Item from List
            ShoppingList.Remove(CurrentTarget);
        } else {
            // Go to Register
            CurrentTarget = Register;  
        }

        // Move to CurrentTarget

    }
}
