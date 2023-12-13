using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shopper : MovingGridObject
{
    // Variable Cache
    public Manager Manager;
    public SpriteRenderer HelpItemRendererSide;
    public SpriteRenderer HelpItemRendererTop;
    public static int minShoppingListSize = 1;
    public static int maxShoppingListSize = 6;
    public float askHelpChance = 0.2f;
    public float tipAmount = 3.0f;
    private CapsuleCollider capCollider;
    private int turntimer1 = -5;

    //Specifc Grid Locations
    [Header("State Positions")]
    public Vector3[] StartingPositions;
    public Vector3[] FrontOfDoorPositions;
    public Vector3[] NeedsHelpPositions;
    public Vector3[] RegisterPositions;
    public Vector3[] BackOfDoorPositions;
    public Vector3[] OffscreenPositions;
    public Vector3[] RegisterLinePositions;

    //Shopping variables
    public List<GroceryType> ShoppingList = new();
    private int ShoppingListSize;
    public ShopperState state;
    public Sprite[] HelpItemArray;

    // Model Storage
    [SerializeField] private GameObject[] characterModels;
    private GameObject avatar;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gridManager.AddToInactiveShoppers(this);
        capCollider = gameObject.GetComponent<CapsuleCollider>();
        HelpItemRendererSide.enabled = false;
        HelpItemRendererTop.enabled = false;
    }

    public override void Turn()
    {
        base.Turn();

        StartTurnMovement();
    }

    public void BecomeActive()
    {
        if (avatar != null) {
            Destroy(avatar);
        }
        // Choose model
        avatar = Instantiate(characterModels[Random.Range(1, 13)], transform.position, transform.rotation, transform); 

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
                case ShopperState.NeedsHelp:
                    NeedsHelpAction();
                    break;
                case ShopperState.GettingHelp:
                    GettingHelpAction();
                    break;
                case ShopperState.Frustrated:
                    FrustratedAction();
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
    #region Actions
    // Do nothing
    private void InactiveAction()
    {
        //Literally just do nothing
    }

    private void EnteringAction()
    {
        if (GetEnteringTargets().Contains(transform.position))
        {
            //Chance of immediately asking for help
            if (Random.value <= askHelpChance)
            {
                state = ShopperState.NeedsHelp;
            }
            else
            {
                state = ShopperState.Shopping;
            }
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
        if (GetShoppingTargets().Contains(transform.position) && usingShelf)
        {
            //If the shelf has stock
            if (usingShelf.HasStock(1))
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
            //Otherwise, take a chance of asking for help
            else if (Random.value <= askHelpChance)
            {
                state = ShopperState.NeedsHelp;
                return;
            }
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void NeedsHelpAction()
    {
        //If 1st Person
        if(GridManager.dimension == 3)
        {
            //Ensure the proper bubble is shown
            HelpItemRendererSide.sprite = HelpItemArray[(int)ShoppingList[0]];
            //Ensure the needed Item bubble is shown
            HelpItemRendererSide.enabled = true;
            HelpItemRendererTop.enabled = false;
        }
        //If Top-Down
        else
        {
            //Ensure the proper bubble is shown
            HelpItemRendererTop.sprite = HelpItemArray[(int)ShoppingList[0]];
            //Ensure the needed Item bubble is shown
            HelpItemRendererTop.enabled = true;
            HelpItemRendererSide.enabled = false;
        }
        //If player touching the shopper and is not already helping another customer
        if (transform.position == ClosestGridPos(gridManager.Player.transform.position) && !gridManager.Player.helpingCustomer)
        {
            //Set the player's helping customer to true
            gridManager.Player.helpingCustomer = true;
            turntimer1 = -5;
            //Set the game state to GettinHelp
            state = ShopperState.GettingHelp;
            return;
        }

        // Manage timer for Duration of guiding (before getting frustrated)
        if (turntimer1 == -5)
        {
            turntimer1 = 180;
        }
        else if (turntimer1 <= 0)
        {
            turntimer1 = -5;
            state = ShopperState.Frustrated;
            return;
        }
        else
        {
            turntimer1 -= 1;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //If already at a potential target, just stand still
        if(potentialTargets.Contains(transform.position))
        {
            return;
        }

        //Otherwise, try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    private void GettingHelpAction()
    {
        //Ensure collision is disabled when following
        capCollider.enabled = false;
        Shelf usingShelf = gridManager.CheckForSurroundingShelf(transform.position);
        //If taken to a shelf and it contains the requested item (first in list)
        if (usingShelf && ShoppingList[0] == usingShelf.groceryType && usingShelf.HasStock(1))
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
            //Otherwise, return to shopping normally
            else
            {
                state = ShopperState.Shopping;
            }
            //Re-enable collision
            capCollider.enabled = true;
            //Remove the help bubble
            HelpItemRendererSide.enabled = false;
            HelpItemRendererTop.enabled = false;
            //Give the Player a tip (Add to Score)
            GridManager.AddCash(tipAmount);
            //Let the player help others
            gridManager.Player.helpingCustomer = false;
            return;
        }

        // Manage timer for Duration of guiding (before getting frustrated)
        if (turntimer1 == -5)
        {
            turntimer1 = 120;
        }
        else if (turntimer1 <= 0)
        {
            turntimer1 = -5;
            //Re-enable collision
            capCollider.enabled = true;
            //Let the player help others
            gridManager.Player.helpingCustomer = false;
            state = ShopperState.Frustrated;
            return;
        }
        else
        {
            turntimer1 -= 1;
        }

        //Otherwise, follow the player
        //Get the first Step to Player
        GenerateFirstStepToPlayer();
    }

    private void FrustratedAction()
    {
        //If 1st Person
        if (GridManager.dimension == 3)
        {
            //Ensure the proper bubble is shown
            HelpItemRendererSide.sprite = HelpItemArray[6];
            //Ensure the needed Item bubble is shown
            HelpItemRendererSide.enabled = true;
            HelpItemRendererTop.enabled = false;
        }
        //If Top-Down
        else
        {
            //Ensure the proper bubble is shown
            HelpItemRendererTop.sprite = HelpItemArray[6];
            //Ensure the needed Item bubble is shown
            HelpItemRendererTop.enabled = true;
            HelpItemRendererSide.enabled = false;
        }
        // Stands still frustrated for a certain amount of time
        if (turntimer1 == -5)
        {
            turntimer1 = 20;
        }
        else if (turntimer1 <= 0)
        {
            if(Manager.state != Manager.ManagerState.ChasePlayer)
            {
                Manager.state = Manager.ManagerState.ChasePlayer;
                Manager.turntimer1 = -5;
            }

            turntimer1 = -5;
            state = ShopperState.CheckingOut;
            //Remove the help bubble
            HelpItemRendererSide.enabled = false;
            HelpItemRendererTop.enabled = false;
            return;
        }
        else
        {
            turntimer1 -= 1;
        }
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

    #endregion
    
    //Path Generation
    #region Path and Target Generation

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
            case ShopperState.NeedsHelp:
                potTargets.AddRange(GetNeedsHelpTargets());
                break;
            case ShopperState.GettingHelp:
                //Should never reach this
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

    private List<Vector3> GetNeedsHelpTargets()
    {
        return NeedsHelpPositions.ToList();
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

    #endregion
}

public enum ShopperState
{
    Inactive, //Doing Nothing
    Entering, //Walking towards and entering the building
    Shopping, //Grabbing different items from list
    NeedsHelp, //Needs help from the player
    GettingHelp, //Getting help from the player
    Frustrated, //Failed to get help in time
    CheckingOutLine, //Heading to registers
    CheckingOut, //Using the register
    Exiting, //Walking to and exiting the building
    WalkingAway //Walking out of view
}