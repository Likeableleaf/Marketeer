using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Manager : MovingGridObject {
    // Variable Cache
    public GameObject CurrentTarget;
    public ManagerState state;
    public int turntimer1 = -5;
    public int turntimer2 = -5;
    [SerializeField] Vector3 OfficeDoorExitPos; //= new Vector3(7.5f, 0.0f, 5.5f);
    [SerializeField] Vector3 OfficeDoorEnterPos; //= new Vector3(7.5f, 0.0f, 4.5f);
    [SerializeField] Vector3 OfficeChairPos; //= new Vector3(9.5f, 0.0f, 7.5f);
    [SerializeField] GameObject[] skins;

    public class StrikerUpdatedEvent : UnityEvent<int> { }
    public static StrikerUpdatedEvent OnStrikeUpdated = new StrikerUpdatedEvent();

    public class EndGameUpdatedEvent : UnityEvent<bool> { }
    public static EndGameUpdatedEvent OnEndGameUpdated = new EndGameUpdatedEvent();

    private int actionNum = 0;
    private int strikeCounter = 0;

    // Start is called before the first frame update
    new void Start() {
        base.Start();

        //Default state
        state = ManagerState.PatrolStore;
    }

    public override void Turn() {
        base.Turn();

        StartTurnMovement();
    }

     public void StartTurnMovement() {
        //If path is valid & still steps in the path left, go to next step
        if (path != null && path.Count > 0) {
            nextStep = path.Pop();
            startTime = Time.time;
            startPosition = transform.position;
            moving = true;
        }
        //If no path left, perform action
        if ((path == null || path.Count <= 0) && !moving){
            //Set this' position to the nextStep if it isnt the default
            if (nextStep != new Vector3(0, 0, 0)) {
                transform.position = nextStep;
            }

            //Path may be generated within the actions
            //State changes will only happen here
            switch (state) {
                case ManagerState.Inactive:
                    InactiveAction();
                    break;
                case ManagerState.EnteringOffice:
                    EnteringOfficeAction();
                    break;
                case ManagerState.DoingOfficeThings:
                    DoingOfficeThingsAction();
                    break;
                case ManagerState.ExitingOffice:
                    ExitingOfficeAction();
                    break;
                case ManagerState.ChasePlayer:
                    ChasePlayerAction();
                    break;
                case ManagerState.PatrolStore:
                    PatrolStoreAction();
                    break;
            }
        }
    }

    //getter
    public ManagerState getState()
    {
        return state;
    }

    // Actions
    
    // Do Nothing
    private void InactiveAction() {
        // Seriously... Nothing... ...?
    }

    // Goto Office
    private void EnteringOfficeAction() {
        // Change to angry skin if needed
        if (!skins[0].active) {
            skins[1].SetActive(false);
            skins[0].SetActive(true);
        }

        // If path finished 
        if (transform.position == OfficeDoorEnterPos) {
            state = ManagerState.DoingOfficeThings;
            return;
        }

        // Retrieve Office Door Location
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Idle movement in Office
    private void DoingOfficeThingsAction() {
        // Timer for switching actions
        if (turntimer2 == -5) {
            turntimer2 = UnityEngine.Random.Range(1, 5);
        } else if (turntimer2 <= 0f) {
            actionNum = 0;
            //actionNum = UnityEngine.Random.Range(0, 0);
        }

        // Manage timer for Duration of stay
        if (turntimer1 == -5) {
            turntimer1 = UnityEngine.Random.Range(5, 25);
        } else if (turntimer1 <= 0) {
            turntimer1 = -5;
            turntimer2 = -5;
            state = ManagerState.ExitingOffice;
            return;
        } else {
            turntimer1 -= 1;
            turntimer2 -= 1;
        }
        
        // Idle Actions to do
        /*switch(actionNum) {
            case 0:
                //ToDo
                break;
        }*/
        
        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Goto StoreFloor
    private void ExitingOfficeAction() {
        // If path finished 
        if (transform.position == OfficeDoorExitPos) {
            state = ManagerState.PatrolStore;
            return;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Chase Player
    private void ChasePlayerAction() {
        // Change to angry skin if needed
        if (!skins[1].active) {
            skins[0].SetActive(false);
            skins[1].SetActive(true);
        }

        if(transform.position == ClosestGridPos(gridManager.Player.transform.position))
        {
            //If not helping customer....
            if(!gridManager.Player.helpingCustomer)
            {
                strikeCounter += 1;

                if (strikeCounter >= 3)//reset strike counter
                {
                    //end game "you are fired!"
                    OnEndGameUpdated.Invoke(true);
                }
                else
                {
                    OnStrikeUpdated.Invoke(strikeCounter);
                }
            }
            //If helping customer
            else
            {
                //TODO
                //Maybe animation here for when already helping?
            }
            //Either way returns to entering office state
            state = ManagerState.EnteringOffice;
            turntimer1 = -5;
            return;
        }

        // Manage timer for Duration of Aggro
        if (turntimer1 == -5)
        {
            turntimer1 = 30;
        }
        else if (turntimer1 <= 0)
        {
            state = ManagerState.EnteringOffice;
            turntimer1 = -5;
            return;
        }
        else
        {
            turntimer1 -= 1;
        }

        //Get the first Step to Player
        GenerateFirstStepToPlayer();
    }

    // Wander and moniter the Store
    private void PatrolStoreAction() {
        Shelf usingShelf = gridManager.CheckForSurroundingShelf(transform.position);
        //If found an adjacent shelf and its empty
        if (usingShelf && !usingShelf.HasStock(1))
        {          
            state = ManagerState.ChasePlayer;
            turntimer1 = -5;
            return;
        }

        // Manage timer for Duration of Patrol
        if (turntimer1 == -5) {
            turntimer1 = UnityEngine.Random.Range(10, 30);
        } else if (turntimer1 <= 0) {
            state = ManagerState.EnteringOffice;
            turntimer1 = -5;
            return;
        } else {
            turntimer1 -= 1;
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);

        // Temp system for now
        //GenerateRandomPath();
    }


    //Target + Path Generation

    private void GenerateRandomPath() {
        Vector3 targetPosition = new Vector3(
            UnityEngine.Random.Range(-9, 9) + 0.5f,
            0,
            UnityEngine.Random.Range(-7, 4) + 0.5f
        );

        PathTile LastTileInPath = AStarPathFromTo(transform.position, targetPosition);
        if (LastTileInPath != null) {
            path = UnwrapPath(LastTileInPath);
            gridManager.RemoveDecidingFlag(transform.position);
        }
    }

    private bool GeneratePathFromTargets(List<Vector3> targets) {
        foreach (var target in targets) {
            //Try to generate a path to the tile
            PathTile potFinalTile = AStarPathFromTo(transform.position, target);
            //If the path works unwrap it and set it as the path
            //Then return true as we were successful
            if (potFinalTile != null && potFinalTile.GetPosition() != transform.position) {
                path = UnwrapPath(potFinalTile);
                gridManager.RemoveDecidingFlag(transform.position);
                return true;
            }
        }

        //Completely blocked, just stand still
        return false;
    }

    private List<Vector3> GeneratePotentialTargets() {
        var potTargets = new List<Vector3>();

        switch (state) {
            case ManagerState.Inactive:
                potTargets.AddRange(GenerateInactiveTargets());
                break;
            case ManagerState.EnteringOffice:
                potTargets.Add(GenerateOfficeEntranceTarget());
                break;
            case ManagerState.DoingOfficeThings:
                potTargets.Add(GenerateDoingOfficeThingsTarget());
                break;
            case ManagerState.ExitingOffice:
                potTargets.Add(GenerateOfficeExitTarget());
                break;
            case ManagerState.PatrolStore:
                potTargets.AddRange(GeneratePatrolStoreTargets());
                break;
        }

        //Add Adjacent tiles just so that the shopper isn't standing still if stuck

        //Calculate the eight adjacent positions
        Vector3 position = transform.position;
        List<Vector3> adjacentPositions = new List<Vector3>() {
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

    private List<Vector3> GenerateInactiveTargets() {
        //This should never get reached or called
        Debug.LogWarning("Unreachable Method GenerateInactiveTargets reached");
        return null;
    }

    // Section Bugged Methinks
    // Return Random Office Positions
    private Vector3 GenerateDoingOfficeThingsTarget() {
        Vector3 DoingOfficeThingsTarget = new Vector3(9.5f, 0.0f, 7.5f);;
        
        // Postions based on preset
        switch(actionNum) {
            case 0: 
            DoingOfficeThingsTarget = OfficeChairPos;
            break;
            // ToDo
        }
                
        return DoingOfficeThingsTarget;
    }

    // Return office enter door position
    private Vector3 GenerateOfficeEntranceTarget() {
        return OfficeDoorEnterPos;
    }

    // Return office exit door position
    private Vector3 GenerateOfficeExitTarget() {
        return OfficeDoorExitPos;
    }

    // Return Patrol Targets
    private List<Vector3> GeneratePatrolStoreTargets() {
        List<Vector3> PatrolStoreTargets = new();

        //Add a random shelf of each type to be patrolled
        foreach (GroceryType grocery in Enum.GetValues(typeof(GroceryType)))
        {
            int randomIndex = UnityEngine.Random.Range(0, gridManager.groceryDictionary[grocery].Count);
            PatrolStoreTargets.Add(gridManager.groceryDictionary[grocery][randomIndex].InFrontOfShelfPos);
        }

        return ShuffleList(PatrolStoreTargets);
    }

    public enum ManagerState {
        Inactive, //Doing Nothing
        EnteringOffice, // Enters office
        DoingOfficeThings, // Idle animations and movement in Office
        ExitingOffice, // Exits office
        ChasePlayer, // Chases after Player
        PatrolStore, // Walks round the store observing
    }
    
    //Strike Methods
    public void SetStrikerCount(int strikes)
    {
        strikeCounter = strikes;
        OnStrikeUpdated.Invoke(strikeCounter);
    }

    public void AddStrikerCount(int strikes)
    {
        strikeCounter += strikes;
        OnStrikeUpdated.Invoke(strikeCounter);
    }

    public void RemoveStrikerCount(int strikes)
    {
        strikeCounter -= strikes;
        OnStrikeUpdated.Invoke(strikeCounter);
    }

    public int getStrikerCount()
    {
        return strikeCounter;
    }
}
