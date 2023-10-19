using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shopper : MovingGridObject
{
    // Variable Cache
    public GameObject CurrentTarget;

    public static int minShoppingListSize = 1;
    public static int maxShoppingListSize = 6;

    //Specifc Grid Locations
    [Header("State Positions")]
    public Vector3[] StartingPositions;
    public Vector3[] FrontOfDoorPositions;
    public Vector3[] RegisterPositions;
    public Vector3[] BackOfDoorPositions;
    public Vector3[] OffscreenPositions;
    public Vector3[] RegisterLinePositions;

    //Moved the list of StoreShelves and Register to GridManager
    public List<GroceryType> ShoppingList = new();
    private int ShoppingListSize;
    public ShopperState state;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gridManager.AddToInactiveShoppers(this);
    }

    public override void Turn()
    {
        base.Turn();

        StartTurnMovement();
    }

    public void BecomeActive()
    {
        // Decide How many Items to Shop for
        ShoppingListSize = Random.Range(minShoppingListSize, maxShoppingListSize + 1);
        // Decide the Items to Shop for
        for (int i = 0; i < ShoppingListSize; i++)
        {
            //Adds a random grocery to the list (can be duplicates)
            ShoppingList.Add((GroceryType)Random.Range(0, System.Enum.GetValues(typeof(GroceryType)).Length));
        }

        state = ShopperState.Entering;
    }

    public void StartTurnMovement()
    {
        //If path is valid & still steps in the path left, go to next step
        if (path != null && path.Count > 0)
        {
            nextStep = path.Pop();
            startTime = Time.time;
            startPosition = transform.position;
            moving = true;
        }
        //If no path left, perform action
        if ((path == null || path.Count <= 0) && !moving)
        {
            //Set this' position to the nextStep if it isnt the default
            if (nextStep != new Vector3(0, 0, 0))
            {
                transform.position = nextStep;
            }

            //Path may be generated within the actions
            //State changes will only happen here
            switch (state)
            {
                case ShopperState.Inactive:
                    InactiveAction();
                    break;
                case ShopperState.Entering:
                    EnteringAction();
                    break;
                case ShopperState.Shopping:
                    ShoppingAction();
                    break;
                case ShopperState.CheckingOutLine:
                    CheckingOutLineAction();
                    break;
                case ShopperState.CheckingOut:
                    CheckingOutAction();
                    break;
                case ShopperState.Exiting:
                    ExitingAction();
                    break;
                case ShopperState.WalkingAway:
                    WalkingAwayAction();
                    break;
            }
        }
    }

    //ShopperState Actions

    // Do nothing
    private void InactiveAction()
    {
        //Literally just do nothing
    }

    private void EnteringAction()
    {
        if (GetEnteringTargets().Contains(transform.position))
        {
            //Open Door
            //Then
            state = ShopperState.Shopping;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void ShoppingAction()
    {
        Shelf usingShelf = gridManager.CheckForSurroundingShelf(transform.position);
        //If found an adjacent shelf and its of a needed item and it has stock
        if (GetShoppingTargets().Contains(transform.position) && usingShelf && usingShelf.HasStock(1))
        {
            //Tell the shelf to reduce its stock
            usingShelf.RemoveItem(1);
            //Remove that item from the ShoppingList
            ShoppingList.Remove(usingShelf.groceryType);

            //If the ShoppingList is now empty go check out
            if (ShoppingList.Count <= 0)
            {
                state = ShopperState.CheckingOutLine;
            }
            return;
        }

        //For now, just create path

        //For fun, ig
        //Likely to fail lol
        //GenerateRandomPath();

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void CheckingOutLineAction() {
        if (GetCheckOutLineTargets().Contains(transform.position))
        {
            // possibly need a if first spot is full and so on so it works like a line....
            state = ShopperState.CheckingOut;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void CheckingOutAction()
    {
        if (GetCheckingOutTargets().Contains(transform.position))
        {
            //Use Register
            /*for (int i = 0; i < ShoppingListSize; i++) {
                wait _ turns;
            } */ 
            //Then
            state = ShopperState.Exiting;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void ExitingAction()
    {
        if (GetExitingTargets().Contains(transform.position))
        {
            //Open Door
            //Then
            state = ShopperState.WalkingAway;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void WalkingAwayAction()
    {
        if (GetWalkingAwayTargets().Contains(transform.position))
        {
            //Become Inactive and remove from active list
            gridManager.RemoveFromActiveShoppers(this);
            gridManager.AddToInactiveShoppers(this);
            gridManager.RemoveDecidingFlag(transform.position);
            transform.position = StartingPositions[Random.Range(0, StartingPositions.Length)];
            nextStep = transform.position;
            startPosition = transform.position;
            state = ShopperState.Inactive;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    //Path Generation

    private void GenerateRandomPath() 
    {
        Vector3 targetPosition = new Vector3(
            UnityEngine.Random.Range(-9, 9) + 0.5f,
            0,
            UnityEngine.Random.Range(-9, 9) + 0.5f
        );

        //targetPosition = new Vector3(4.5f, 0, 4.5f);
        PathTile LastTileInPath = AStarPathFromTo(transform.position, targetPosition);
        if (LastTileInPath != null)
        {
            path = UnwrapPath(LastTileInPath);
            gridManager.RemoveDecidingFlag(transform.position);
        }
    }

    private bool GeneratePathFromTargets(List<Vector3> targets)
    {
        foreach (var target in targets)
        {
            //Try to generate a path to the tile
            PathTile potFinalTile = AStarPathFromTo(transform.position, target);
            //If the path works unwrap it and set it as the path
            //Then return true as we were successful
            if (potFinalTile != null && potFinalTile.GetPosition() != transform.position)
            {
                path = UnwrapPath(potFinalTile);
                gridManager.RemoveDecidingFlag(transform.position);
                return true;
            }
        }

        //Completely blocked, just stand still
        return false;
    }

    private List<Vector3> GeneratePotentialTargets()
    {
        var potTargets = new List<Vector3>();

        switch (state)
        {
            case ShopperState.Inactive:
                potTargets.AddRange(GetInactiveTargets());
                break;
            case ShopperState.Entering:
                potTargets.AddRange(GetEnteringTargets());
                break;
            case ShopperState.Shopping:
                potTargets.AddRange(GetShoppingTargets());
                //Add adjacent Tiles too
                potTargets.AddRange(GenerateAdjacentTargets());
                break;
            case ShopperState.CheckingOutLine:
                potTargets.AddRange(GetCheckOutLineTargets());
                break;
            case ShopperState.CheckingOut:
                potTargets.AddRange(GetCheckingOutTargets());
                //Add adjacent Tiles too
                potTargets.AddRange(GenerateAdjacentTargets());
                break;
            case ShopperState.Exiting:
                potTargets.AddRange(GetExitingTargets());
                //Add adjacent Tiles too
                potTargets.AddRange(GenerateAdjacentTargets());
                break;
            case ShopperState.WalkingAway:
                potTargets.AddRange(GetWalkingAwayTargets());
                break;
        }
        //Return the List
        return potTargets;
    }

    private List<Vector3> GenerateAdjacentTargets()
    {
        Vector3 position = transform.position;
        List<Vector3> adjacentPositions = new List<Vector3>()
        {
            position + new Vector3(1, 0, 1),
            position + new Vector3(1, 0, 0),
            position + new Vector3(1, 0, -1),
            position + new Vector3(0, 0, 1),
            position + new Vector3(0, 0, -1),
            position + new Vector3(-1, 0, 1),
            position + new Vector3(-1, 0, 0),
            position + new Vector3(-1, 0, -1)
        };
        return ShuffleList(adjacentPositions);
    }

    private List<Vector3> GetInactiveTargets()
    {
        //This should never get reached or called
        Debug.LogWarning("Unreachable Method GenerateInactiveTargets reached");
        return null;
    }

    // Return front door positions
    private List<Vector3> GetEnteringTargets()
    {
        return FrontOfDoorPositions.ToList();
    }

    //Return front of shelves positions
    private List<Vector3> GetShoppingTargets()
    {
        List<Vector3> ShoppingTargets = new();
        //Shopping targets are the positions in front of needed materials

        //Randomize the shopping List order
        ShoppingList = ShuffleList(ShoppingList);

        //Add the accessible spot of each shelf
        foreach (var grocery in ShoppingList)
        {
            foreach (var shelf in gridManager.groceryDictionary[grocery])
            {
                ShoppingTargets.Add(shelf.InFrontOfShelfPos);
            }
        }

        return ShuffleList(ShoppingTargets);
    }

    private List<Vector3> GetCheckOutLineTargets() {
        return RegisterLinePositions.ToList();
    }
    
    // Return positions in front of the registers 
    private List<Vector3> GetCheckingOutTargets()
    {
        return RegisterPositions.ToList();
    }

    // Return exit door position
    private List<Vector3> GetExitingTargets()
    {
        return BackOfDoorPositions.ToList();
    }

    // Return offscreen positions
    private List<Vector3> GetWalkingAwayTargets()
    {
        return OffscreenPositions.ToList();
    }
}

public enum ShopperState
{
    Inactive, //Doing Nothing
    Entering, //Walking towards and entering the building
    Shopping, //Grabbing different items from list
    CheckingOutLine, //Heading to registers
    CheckingOut, //Using the register
    Exiting, //Walking to and exiting the building
    WalkingAway //Walking out of view
}