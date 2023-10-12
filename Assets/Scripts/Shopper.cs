using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shopper : MovingGridObject
{
    // Variable Cache
    public GameObject CurrentTarget;

    public static int minShoppingListSize = 1;
    public static int maxShoppingListSize = 6;

    //Moved the list of StoreShelves and Register to GridManager
    private List<GroceryType> ShoppingList = new();
    private int ShoppingListSize;
    private ShopperState state;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //Default state for now
        state = ShopperState.Shopping;

        // Decide How many Items to Shop for
        ShoppingListSize = Random.Range(minShoppingListSize, maxShoppingListSize+1);

        // Decide the Items to Shop for
        for (int i = 0; i < ShoppingListSize; i++)
        {
            //Adds a random grocery to the list (can be duplicates)
            ShoppingList.Add((GroceryType)Random.Range(0, System.Enum.GetValues(typeof(GroceryType)).Length));
        }
    }

    public override void Turn()
    {
        base.Turn();

        StartTurnMovement();
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
        if (path == null || path.Count <= 0)
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

    }

    private void EnteringAction()
    {
        //TODO

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void ShoppingAction()
    {
        //TODO

        //For now, just create path

        //For fun, ig
        //Likely to fail lol
        //GenerateRandomPath();

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void CheckingOutAction()
    {
        //TODO

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void ExitingAction()
    {
        //TODO

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void WalkingAwayAction()
    {
        //TODO

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    //Path Generation

    private void GenerateRandomPath() 
    {
        targetPosition = new Vector3(
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
                potTargets.AddRange(GenerateInactiveTargets());
                break;
            case ShopperState.Entering:
                potTargets.AddRange(GenerateEnteringTargets());
                break;
            case ShopperState.Shopping:
                potTargets.AddRange(GenerateShoppingTargets());
                break;
            case ShopperState.CheckingOut:
                potTargets.AddRange(GenerateCheckingOutTargets());
                break;
            case ShopperState.Exiting:
                potTargets.AddRange(GenerateExitingTargets());
                break;
            case ShopperState.WalkingAway:
                potTargets.AddRange(GenerateWalkingAwayTargets());
                break;
        }

        //Add Adjacent tiles just so that the shopper isn't standing still if stuck

        //Calculate the eight adjacent positions
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

        //Shuffle the adjacent positions and add them to potTargets
        potTargets.AddRange(ShuffleList(adjacentPositions));

        //Return the List
        return potTargets;
    }

    private List<Vector3> GenerateInactiveTargets()
    {
        //This should never get reached or called
        Debug.LogWarning("Unreachable Method GenerateInactiveTargets reached");
        return null;
    }

    // Return front door positions
    private List<Vector3> GenerateEnteringTargets()
    {
        List<Vector3> EnteringTargets = new();
       
        EnteringTargets.Add(new Vector3(-0.5f, 0.0f, -7.5f));

        return EnteringTargets;
    }

    private List<Vector3> GenerateShoppingTargets()
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

        return ShoppingTargets;
    }

    // Return positions in front of the registers 
    private List<Vector3> GenerateCheckingOutTargets()
    {
        List<Vector3> CheckingOutTargets = new();

        CheckingOutTargets.Add(new Vector3(3.5f, 0f, -4.5f));
        CheckingOutTargets.Add(new Vector3(5.5f, 0f, -3.5f));

        return CheckingOutTargets;
    }

    // Return exit door position
    private List<Vector3> GenerateExitingTargets()
    {
        List<Vector3> ExitingTargets = new();
        
        ExitingTargets.Add(new Vector3(0.5f, 0.0f, -7.5f));

        return ExitingTargets;
    }

    // Return offscreen positions
    private List<Vector3> GenerateWalkingAwayTargets()
    {
        List<Vector3> WalkingAwayTargets = new();
        
        WalkingAwayTargets.Add(new Vector3(2.5f, 0.0f, -10.5f));

        return WalkingAwayTargets;
    }
}

public enum ShopperState
{
    Inactive, //Doing Nothing
    Entering, //Walking towards and entering the building
    Shopping, //Grabbing different items from list
    CheckingOut, //Heading to and using the register
    Exiting, //Walking to and exiting the building
    WalkingAway //Walking out of view
}