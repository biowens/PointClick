using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnerRecord
{
    public GameObject prefab;
    public float weight;
}

public class Interactable : MonoBehaviour
{
    public GameEvent lockClick;
    public GameEvent unlockClick;
    
    public BoolVariable playerMoving;

    public GameEvent movePlayer;
    public Vector3Variable moveLocation;

    public GameObject standPoint;

    public List<SpawnerRecord> states;

    private IEnumerable coroutine;
    
    public void Interact()
    {
        StartCoroutine(InteractCoroutine());

        /*
        // Lock clicking until interaction is done
        lockClick.Raise();

        MovePlayer();

        Debug.Log("You Interacted!");

        // Unlock clicking now interaction is done
        unlockClick.Raise();
        */
    }

    // I did this because I dont know how to wait :raegret:
    private IEnumerator InteractCoroutine() 
    {
        // Lock clicking until interaction is done
        lockClick.Raise();

        // if there is a move location, move player to location
        if (moveLocation != null)
        {
            moveLocation.SetValue(standPoint.transform.position);
            movePlayer.Raise();

            // Wait until player is done moving
            Debug.Log("WAITING");
            yield return new WaitUntil(() => !playerMoving.Value);
            Debug.Log("NOT WAITING");
        }

        Debug.Log("You Interacted!");

        // Unlock clicking now interaction is done
        unlockClick.Raise();
    }

    public void Look()
    {
        Debug.Log("You Looked!");
    }


    // ******************************************
    // OLD FUNCTIONS TO REFERENCE LATER IF NEEDED
    // ******************************************
    void MovePlayer()
    {
        // if there is a move location, move player to location
        if (moveLocation != null)
        {
            moveLocation.SetValue(standPoint.transform.position);
            movePlayer.Raise();

            // Wait until player is done moving
            Debug.Log("Waiting coroutine start");
            StartCoroutine(WaitForDestinationReach());
            Debug.Log("Waiting coroutine end");
        }
    }
  
    private IEnumerator WaitForDestinationReach()
    {
        Debug.Log("WAITING");
        yield return new WaitUntil(() => !playerMoving.Value);
        Debug.Log("NOT WAITING");
    }

}
