using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Manager : MovingGridObject {
    // Variable Cache
    public GameObject CurrentTarget;
    
    private ManagerState state;

    // Start is called before the first frame update
    new void Start() {
        base.Start();
        
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
        if (path == null || path.Count <= 0) {
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
        
    }

    // Goto Office
    private void EnteringOfficeAction() {

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Idle movement in Office
    private void DoingOfficeThingsAction() {
        
        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Goto StoreFloor
    private void ExitingOfficeAction() {
        
        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Chase Player
    private void ChasePlayerAction() {
        
        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }

    // Wander and moniter the Store
    private void PatrolStoreAction() {

        //Generate list of potential targets
        List<Vector3> potentialTargets = GeneratePotentialTargets();

        //Try to form a path in the potential targets, doing the first one that results in a valid path
        GeneratePathFromTargets(potentialTargets);
    }


    //Target Generation

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
                potTargets.AddRange(GenerateOfficeEntranceTarget());
                break;
            case ManagerState.DoingOfficeThings:
                potTargets.AddRange(GenerateDoingOfficeThingsTargets());
                break;
            case ManagerState.ExitingOffice:
                potTargets.AddRange(GenerateOfficeEntranceTarget());
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

    // Return Random Office Positions
    private List<Vector3> GenerateDoingOfficeThingsTargets() {
        List<Vector3> DoingOfficeThingsTargets = new();
        
        DoingOfficeThingsTargets.Add(new Vector3(0.5f, 0.0f, -7.5f));
        
        return DoingOfficeThingsTargets;
    }

    // Return office exit door position
    private List<Vector3> GenerateOfficeEntranceTarget() {
        List<Vector3> ExitingOfficeTargets = new();
        
        ExitingOfficeTargets.Add(new Vector3(0.5f, 0.0f, -7.5f));

        return ExitingOfficeTargets;
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
