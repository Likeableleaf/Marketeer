using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    //Grid System Variables
    public Dictionary<Vector3, Dictionary<int, GameObject>> GameGrid = new();
    public float turnInterval = 1f;
    private Queue<GridObject> turnQueue = new();
    private float timeSinceLastTurn = 0.0f;

    //Grocery System Variables
    public Dictionary<GroceryType, List<Shelf>> groceryDictionary = new();
    public GameObject[] Registers;

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
        AddObjectAt(position, turnNumber, obj.gameObject);
        AddObjectAt(position, -5, obj.gameObject);
        turnQueue.Enqueue(obj);
    }

    public void AddObjectAt(Vector3 position, int turnNumber, GameObject obj)
    {
        if (!GameGrid.ContainsKey(position))
        {
            GameGrid[position] = new Dictionary<int, GameObject>();
        }
        GameGrid[position][turnNumber] = obj;
    }

    public void RemoveDecidingFlag(Vector3 position)
    {
        if (GameGrid.ContainsKey(position))
        {
            GameGrid[position].Remove(-5);
        }
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