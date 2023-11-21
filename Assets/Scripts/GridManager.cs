using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    //Event for when Cash changes
    [System.Serializable]
    public class CashUpdatedEvent : UnityEvent<float> { }
    public static CashUpdatedEvent OnCashUpdated = new CashUpdatedEvent();
    
    //Score Variables
    private static float Cash;

    //Dimension
    public static int dimension = 2;

    //Grid System Variables
    public Dictionary<Vector3, Dictionary<int, GridObject>> GameGrid = new();
    public float turnInterval = 1f;
    public int SimultaneousActiveShoppers = 4;
    public Camera MainCamera;
    public Camera PlayerCamera;
    public CharacterController Player;
    private Queue<GridObject> turnQueue = new();
    private List<Shopper> ActiveShoppers = new();
    private Queue<Shopper> InactiveShoppers = new();
    private float timeSinceLastTurn = 0.0f;

    //Grocery System Variables
    public Dictionary<GroceryType, List<Shelf>> groceryDictionary = new();
    public GameObject[] Registers;

    //Other Variables
    [SerializeField] private GameObject ceiling;

    private void Awake()
    {
        //Create the empty lists for the dictionary
        foreach (GroceryType type in System.Enum.GetValues(typeof(GroceryType)))
        {
            groceryDictionary[type] = new List<Shelf>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MainCamera.enabled = true;
        PlayerCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the timer
        timeSinceLastTurn += Time.deltaTime;

        // Check if the interval has passed
        if (timeSinceLastTurn >= turnInterval)
        {
            // Call AdvanceTurns and reset the timer
            AdvanceTurns();
            timeSinceLastTurn -= turnInterval;
        }
    }

    public void AdvanceTurns()
    {
        //PrintGameGrid();
        //First, Update all of the Grid Positions and their times
        //Remove Empty Grid Positions from the Dictionary
        List<Vector3> positionsToRemove = new List<Vector3>();

        foreach (var position in GameGrid.Keys)
        {
            var turnKeys = new List<int>(GameGrid[position].Keys);
            foreach (var turn in turnKeys)
            {
                if (turn > 0)
                {
                    GameGrid[position][turn - 1] = GameGrid[position][turn];
                }
                if (turn != -10 && turn != -5)
                {
                    // Remove old key
                    GameGrid[position].Remove(turn);
                }
            }
            if (GameGrid[position].Count == 0)
            {
                positionsToRemove.Add(position);
            }
        }
        foreach (var positionToRemove in positionsToRemove)
        {
            GameGrid.Remove(positionToRemove);
        }
        //Then, if there are not enough active Shoppers, activate an inactive one
        if (SimultaneousActiveShoppers > ActiveShoppers.Count)
        {
            Shopper activatedShopper = InactiveShoppers.Dequeue();
            ActiveShoppers.Add(activatedShopper);
            activatedShopper.BecomeActive();
        }

        //To make sure Grid Dictionary isn't being modified while read, do each object's turn one at a time
        foreach (var obj in turnQueue)
        {
            obj.Turn();
            //PrintGameGrid();
        }
    }

    public void AddGridObjectToGrid(Vector3 position, int turnNumber, GridObject obj)
    {
        // Add the object to the GameGrid dictionary using its position as the key
        AddObjectAt(position, turnNumber, obj);
        AddObjectAt(position, -5, obj);
        turnQueue.Enqueue(obj);
    }

    public void AddObjectAt(Vector3 position, int turnNumber, GridObject obj)
    {
        if (!GameGrid.ContainsKey(position))
        {
            GameGrid[position] = new Dictionary<int, GridObject>();
        }
        GameGrid[position][turnNumber] = obj;
    }

    public void AddToInactiveShoppers(Shopper shopper)
    {
        InactiveShoppers.Enqueue(shopper);
    }

    public void RemoveFromActiveShoppers(Shopper shopper)
    {
        ActiveShoppers.Remove(shopper);
    }

    public void RemoveDecidingFlag(Vector3 position)
    {
        if (GameGrid.ContainsKey(position))
        {
            GameGrid[position].Remove(-5);
        }
    }

    public Shelf CheckForSurroundingShelf(Vector3 position)
    {
        foreach(var shelfList in groceryDictionary.Values)
        {
            foreach (var shelf in shelfList)
            {
                if(shelf.InFrontOfShelfPos == position)
                {
                    return shelf;
                }
            }
        }
        return null;
    }

    private void PrintGameGrid()
    {
        string logMessage = "";
        foreach (var kvp1 in GameGrid)
        {
            logMessage += $"Position: {kvp1.Key}\n";
            foreach (var kvp2 in kvp1.Value)
            {
                logMessage += $"    Key: {kvp2.Key}, Value: {(kvp2.Value != null ? kvp2.Value.name : "null")}\n";
            }
        }

        Debug.Log(logMessage);
    }

    public void ShiftDimension(int newDimension)
    {
        switch (newDimension)
        {
            case 2:
                MainCamera.enabled = true;
                PlayerCamera.enabled = false;
                ceiling.SetActive(false);
                break;
            case 3:
                MainCamera.enabled = false;
                PlayerCamera.enabled = true;
                ceiling.SetActive(true);
                break;
        }
        dimension = newDimension;
    }

    //Cash Methods
    //Set the amount of Cash directly
    public static void SetCash(float amount)
    {
        Cash = amount;
        OnCashUpdated.Invoke(Cash);
    }
    //Add memory to Cash
    public static void AddCash(float amount)
    {
        Cash += amount;
        OnCashUpdated.Invoke(Cash);
    }
    //Remove an amount of Cash
    public static void RemoveCash(float amount)
    {
        Cash -= amount;
        OnCashUpdated.Invoke(Cash);
    }
    //Check if we have the given amount of cash
    public static bool IsCashAvailable(float amount)
    {
        if (Cash >= amount)
        {
            return true;
        }
        return false;
    }
    //Get the amount of Cash
    public static float GetCash(float amount)
    {
        return Cash;
    }
}

public enum GroceryType
{
    Beans,
    Bread,
    Eggs,
    Meat,
    Milk,
    Veggies
}