using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class MoveCharacter : MonoBehaviour
{
    public Vector3Variable moveDestination;
    private AIPath aipath;
    private AIDestinationSetter destinationsetter;

    private void Awake() {
        aipath = GetComponent<AIPath>();
        Debug.Log(aipath.name);
    }

    public void moveCharacterToDestination()
    {
        //Debug.Log("Test");
        //Debug.Log("AIPath Destination" + aipath.destination);
        aipath.destination = moveDestination.Value;
    }
}
