using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovingGridObject : GridObject
{
    public Vector3 targetPosition;
    public Stack<Vector3> path;

    private float moveTime = 1.0f; //Number of turns to move a spot
    private Vector3 nextStep;
    private bool moving = false;
    private float startTime;
    private Vector3 startPosition;


    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        //Set the moveTime to the time a Turn is
        moveTime = gridManager.turnInterval;
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();

        if (moving)
        {
            float timeSinceStarted = Time.time - startTime;
            float percentageComplete = timeSinceStarted / moveTime;

            transform.position = Vector3.Lerp(startPosition, nextStep, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                moving = false;
                transform.position = nextStep;
            }
            Vector3 direction = nextStep - startPosition;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, percentageComplete);
            }
        }
    }

    //Is called once per turn by the GridManager
    public override void Turn()
    {
        //Debug.Log(this.name + " Turn Started");
        base.Turn();

        StartTurnMovement();
        //Debug.Log(this.name + " Turn Ended");
    }

    public void StartTurnMovement()
    {
        if (path != null && path.Count > 0)
        {
            nextStep = path.Pop();
            startTime = Time.time;
            startPosition = transform.position;
            moving = true;
        }
        else
        {
            if(nextStep != new Vector3(0,0,0))
            {
                transform.position = nextStep;
            }
            targetPosition = new Vector3(
                Random.Range(-9, 9) + 0.5f,
                0,
                Random.Range(-9, 9) + 0.5f
            );

            //targetPosition = new Vector3(4.5f, 0, 4.5f);
            path = AStarPathFromTo(transform.position, targetPosition);
            if(path != null)
            {
                gridManager.RemoveDecidingFlag(transform.position);
            }
        }
    }
}
