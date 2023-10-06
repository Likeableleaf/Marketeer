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

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Decide How many Items to Shop for
        ShoppingListSize = Random.Range(minShoppingListSize, maxShoppingListSize+1);

        // Decide the Items to Shop for
        for (int i = 0; i < ShoppingListSize; i++)
        {
            //Adds a random grocery to the list (can be duplicates)
            ShoppingList.Add((GroceryType)Random.Range(0, System.Enum.GetValues(typeof(GroceryType)).Length));
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        //Leaving this here just to say
        //DONT USE UPDATE FOR GRIDOBJECTS
        //Only use Turn

    }

    public override void Turn()
    {
        base.Turn();

        StartTurnMovement();
        /*
        // Decide if finished 
        if (ShoppingList.Count > 0)
        {
            // Go to First item in List
            CurrentTarget = ShoppingList[0];

            // Delete Item from List
            ShoppingList.Remove(CurrentTarget);
        }
        else
        {
            // Go to Register
            CurrentTarget = Register;
        }
        */
        // Move to CurrentTarget
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
        //Otherwise, generate next path
        else
        {
            //Set this' position to the nextStep if it isnt the default
            if (nextStep != new Vector3(0, 0, 0))
            {
                transform.position = nextStep;
            }

            //Now, check if character needs to perform an action here
            //First, check if at just outside front door (assuming trying to enter the building)
            if (false)
            {
                //TODO
            }

            //Second, check if in front of a shelf of a needed product
            else if (false) 
            {
                //TODO
            }

            //Third, check if in front of a register
            else if (false)
            {
                //TODO
            }

            //Fourth, check if in at just inside front door (assuming trying to exit the building)
            else if (false)
            {
                //TODO
            }

            //Fifth, check if outside of level (assuming finished shopper experience)
            else if (false)
            {
                //TODO
            }

            //Otherwise, do nothing special and 

            //For fun, ig
            //Likely to fail lol
            //GenerateRandomPath();

            //Generate list of potential targets
            List<Vector3> potentialTargets = GeneratePotentialTargets();

            //Try to form a path in the potential targets, doing the first one that results in a valid path
            GeneratePathFromTargets(potentialTargets);
        }
    }

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

    private List<Vector3> GeneratePotentialTargets()
    {
        var potTargets = new List<Vector3>();

        //First, check if need to move into the store
        if (false)
        {
            //TODO
        }

        //Second, check if need to grab groceries
        else if (ShoppingList.Count > 0)
        {
            //Randomize the shopping List order
            int n = ShoppingList.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                GroceryType value = ShoppingList[k];
                ShoppingList[k] = ShoppingList[n];
                ShoppingList[n] = value;
            }
            //Add the accessible spot of each shelf
            foreach(var grocery in ShoppingList)
            {
                foreach(var shelf in gridManager.groceryDictionary[grocery])
                {
                    potTargets.Add(shelf.InFrontOfShelfPos);
                }
            }
        }

        //Third, check if need to go checkout
        else if (false)
        {
            //TODO
        }

        //Fourth, check if need to exit the store
        else
        {
            //TODO
        }

        //Fifth, add the adjacent tiles just so that the shopper isn't standing sitll if stuck

        //Calculate the eight adjacent positions
        Vector3 position = transform.position;
        potTargets.Add(position + new Vector3(1, 0, 1));
        potTargets.Add(position + new Vector3(1, 0, 0));
        potTargets.Add(position + new Vector3(1, 0, -1));
        potTargets.Add(position + new Vector3(0, 0, 1));
        potTargets.Add(position + new Vector3(0, 0, -1));
        potTargets.Add(position + new Vector3(-1, 0, 1));
        potTargets.Add(position + new Vector3(-1, 0, 0));
        potTargets.Add(position + new Vector3(-1, 0, -1));

        //Return the List
        return potTargets;
    }

    private bool GeneratePathFromTargets(List<Vector3> targets)
    {
        foreach(var target in targets)
        {
            //Try to generate a path to the tile
            PathTile potFinalTile = AStarPathFromTo(transform.position, target);
            //If the path works unwrap it and set it as the path
            //Then return true as we were successful
            if (potFinalTile != null)
            {
                path = UnwrapPath(potFinalTile);
                gridManager.RemoveDecidingFlag(transform.position);
                return true;
            }
        }

        //Completely blocked, just stand still
        return false;
    }
}
