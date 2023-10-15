using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Manager : MovingGridObject {
    // Variable Cache
    public GameObject CurrentTarget;
    [SerializeField] private ManagerState state;
    [SerializeField] private int turntimer1 = -5;
    [SerializeField] private int turntimer2 = -5;
    [SerializeField] Vector3 OfficeDoorExitPos; //= new Vector3(7.5f, 0.0f, 5.5f);
    [SerializeField] Vector3 OfficeDoorEnterPos; //= new Vector3(7.5f, 0.0f, 4.5f);
    [SerializeField] Vector3 OfficeChairPos; //= new Vector3(9.5f, 0.0f, 7.5f);

    private int actionNum = 0;

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

    // Actions
    
    // Do Nothing
    private void InactiveAction() {
        // Seriously... Nothing... ...?
    }

    // Goto Office
    private void EnteringOfficeAction() {
        // "Observe" for empty shelves
        //ToDO 

        // If finds emplty shelf ChasePlayer
        //ToDo
        /*if(-_-) {
            state = ManagerState.ChasePlayer;
        }
        */

        // If path finished 
        if (transform.position == OfficeDoorEnterPos) {
            state = ManagerState.DoingOfficeThings;
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
        }

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Chase Player
    private void ChasePlayerAction() {
        // As long as AggroTimer is active?
        // Todo

        // When AgroTimer Done
        // state = ManagerState.PatrolStore; or state = ManagerState.EnteringOffice;?
        
        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Wander and moniter the Store
    private void PatrolStoreAction() {
        // "Observe" for empty shelves
        //ToDO 

        // If finds emplty shelf ChasePlayer
        //ToDo
        /*if(-_-) {
            state = ManagerState.ChasePlayer;
        }
        */

        // Manage timer for Duration of Patrol
        if (turntimer1 == -5) {
            turntimer1 = UnityEngine.Random.Range(10, 30);
        } else if (turntimer1 <= 0) {
            state = ManagerState.EnteringOffice;
            turntimer1 = -5;
        } else {
            turntimer1 -= 1;
        }

        //Generate list of potential targets
        //List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        //GeneratePathFromTargets(potentialTargets);

        // Temp system for now
        GenerateRandomPath();
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

    /*private bool GeneratePathFromTarget(Vector3 target) {
        //Try to generate a path to the tile
        PathTile potFinalTile = AStarPathFromTo(transform.position, target);
            
        //If the path works unwrap it and set it as the path
        if (potFinalTile != null && potFinalTile.GetPosition() != transform.position) {
            path = UnwrapPath(potFinalTile);
            gridManager.RemoveDecidingFlag(transform.position);
                
            //Then return true as we were successful
            return true;
        }

        //Completely blocked, just stand still
        return false;
    }*/


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
            case ManagerState.ChasePlayer:
                potTargets.AddRange(GenerateChasePlayerTargets());
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

    // Return Player Position
    private List<Vector3> GenerateChasePlayerTargets() {
        List<Vector3> PlayerTargets = new();
        
        // PlayerTargets.Add();

        return PlayerTargets;
    }

    // Return Patrol Targets
    private List<Vector3> GeneratePatrolStoreTargets() {
        List<Vector3> PatrolStoreTargets = new();
        
        // PatrolStoreTargets.Add(new Vector3(0.5f, 0.0f, -0.5f));

        return PatrolStoreTargets;
    }

    public enum ManagerState {
        Inactive, //Doing Nothing
        EnteringOffice, // Enters office
        DoingOfficeThings, // Idle animations and movement in Office
        ExitingOffice, // Exits office
        ChasePlayer, // Chases after Player
        PatrolStore, // Walks round the store observing
    }
}
