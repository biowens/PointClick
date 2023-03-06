using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class MoveCharacter : MonoBehaviour
{
    public BoolVariable playerMoving;

    public Vector3Variable moveDestination;
    private AIPath aipath;
    private AIDestinationSetter destinationsetter;

    void Update()
    {
        if (playerMoving.Value == true && aipath.reachedDestination) {
            playerMoving.SetValue(false);
        }
    }

    private void Awake() {
        aipath = GetComponent<AIPath>();
        Debug.Log(aipath.name);
    }

    public void moveCharacterToDestination()
    {
        //Debug.Log("Test");
        //Debug.Log("AIPath Destination" + aipath.destination);
        aipath.destination = moveDestination.Value;
        playerMoving.SetValue(true);
    }
}
