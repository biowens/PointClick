using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class States
{
    public GameObject changeStateObject;
    public Item interactionItem;
    public bool destroyInteractionItem;
    public Item pickUpItem;
    public List<GameObject> enabledObjects;
}

public enum interactionType
{
    ChangeState,
    PickUp
}

public class Interactable : MonoBehaviour
{
    [Header("Game Events")]
    public GameEvent lockClick;
    public GameEvent unlockClick;
    
    public GameEvent movePlayer;

    [Header("Scriptable Object References")]
    public BoolVariable playerMoving;
    public Vector3Variable moveLocation;
    public ItemCollection inventory;

    [Header("GameObject")]
    public GameObject standPoint;

    [Header("Interaction Management")]

    public List<States> states;
    
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
        
        //Debug.Log("Start Pick Up");
        //PickUp();
        Debug.Log("Start StateChange");
        StateChange();

        Debug.Log("You Interacted!");

        // Unlock clicking now interaction is done
        unlockClick.Raise();
    }

    private void StateChange()
    {
        Item activeItem = null;
        bool allObjectsEnabled = true;

        // Check if there's an active item in inventory
        for (int i = 0; i < inventory.Items.Count; i++)
        {
            // If there is, save it
            if (inventory.Items[i].active)
                activeItem = inventory.Items[i].item;
        }

        Debug.Log("Active Item is " + activeItem);

        // Check if there's a state that is valid to change to
        // For each state
        for (int i = 0; i < states.Count; i++)
        {
            // If there's an active item, check to see if state has same item, or if no active item, check if state has no items listed
            if (states[i].interactionItem == activeItem)
            {
                // For each EnabledObject
                for (int j = 0; j < states[i].enabledObjects.Count; j++)
                {
                    // Verify that each object is enabled
                    if (!states[i].enabledObjects[j].activeSelf)
                        allObjectsEnabled = false;
                }

                Debug.Log("AllObjectsEnabled is " + allObjectsEnabled);

                if (allObjectsEnabled)
                {
                    // If destroy interaction object, remove object from inventory
                    if (states[i].destroyInteractionItem)
                    {
                        inventory.RemoveActiveItem();
                        Debug.Log("Removed active item from inventory");
                    }

                    // Disable current game object
                    this.gameObject.SetActive(false);
                    Debug.Log("Disabling current object " + this.gameObject.name);

                    // Pick up object, if needed
                    if (states[i].pickUpItem != null)
                    {
                        inventory.AddItem(states[i].pickUpItem);
                        Debug.Log("Picked Up Item");
                    }

                    // Enable the game object in that state, if there is one
                    if (states[i].changeStateObject != null)
                    {
                        states[i].changeStateObject.SetActive(true);
                        Debug.Log("Enabling new object " + states[i].changeStateObject.name);
                    }
                }
            }
        }
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
