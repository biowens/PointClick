using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

[System.Serializable]
public class States
{
    public GameObject changeStateObject;
    public Item interactionItem;
    public bool destroyInteractionItem;
    public TextAsset interactionDialogueJSON = null;
	public TextAsset newLookDialogueJSON = null;
    public List<Item> pickUpItem;
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
    [SerializeField]
    private GameObject currentObject;
    [SerializeField]
	private TextAsset lookDialogueJSON = null;
    private Story lookStory;

    public List<States> states;
    
    public void Interact()
    {
        StartCoroutine(InteractCoroutine());
    }

    // I did this because I dont know how to wait :raegret:
    private IEnumerator InteractCoroutine() 
    {
        States changeState = null;

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
        
        changeState = GetValidState();

        if (changeState != null)
        {
            if (changeState.interactionDialogueJSON != null)
            {
                // Play any interaction dialogue
                UIDialogueManager.GetInstance().Dialogue(changeState.interactionDialogueJSON);
                
                // Wait for dialogue to end
                yield return new WaitUntil(() => !UIDialogueManager.GetInstance().dialogueIsPlaying && !UIDialogueManager.GetInstance().choiceIsPlaying);
            }

            // Change state
            Debug.Log("CHANGING STATE NOW");
            ChangeState(changeState);
        }

        // After trying item, it should not be active anymore
        inventory.disableAllActiveItems();

        /*
        Debug.Log("Start StateChange");
        StateChange();
        Debug.Log("You Interacted!");
        */

        // Unlock clicking now interaction is done
        unlockClick.Raise();
    }

    private Item GetActiveItem()
    {
        Item activeItem = null;

        // Check if there's an active item in inventory
        if (inventory.getActiveIndex() != -1)
            activeItem = inventory.Items[inventory.getActiveIndex()].item;
        Debug.Log("Active Item is " + activeItem);
        
        return activeItem;
    }

    private States GetValidState()
    {
        Item activeItem = null;
        bool allObjectsEnabled;

        activeItem = GetActiveItem();

        for (int i =0; i < states.Count; i++)
        {
            States checkState = states[i];

            if (checkState.interactionItem == activeItem)
            {
                // Initialize allObjectsEnabled
                allObjectsEnabled = true;

                // Verify that each object is enabled
                for (int j = 0; j < checkState.enabledObjects.Count; j++)
                {
                    if (!checkState.enabledObjects[j].activeSelf)
                        allObjectsEnabled = false;
                }

                if (allObjectsEnabled)
                {
                    //Debug.Log("VALID STATE FOUND - Active Item: " + activeItem.itemName + " - Changed Item will be: " + checkState.changeStateObject.name + " - State Index: " + i);
                    return checkState;
                }
            }
        }

        return null;
    }

    private void ChangeState(States state) 
    {
        // If destroy interaction object, remove object from inventory
        if (state.destroyInteractionItem)
        {
            inventory.RemoveActiveItem();
            Debug.Log("Removed active item from inventory");
        }

        // Disable current game object
        currentObject.SetActive(false);
        Debug.Log("Disabling current object " + currentObject.name);

        // Pick up object, if needed
        if (state.pickUpItem != null)
        {
            for (int j = 0; j < state.pickUpItem.Count; j++)
            {
                inventory.AddItem(state.pickUpItem[j]);
                Debug.Log("Picked Up Item " + state.pickUpItem[j].name);
            }
        }

        // Enable the game object in that state, if there is one
        if (state.changeStateObject != null)
        {
            currentObject = state.changeStateObject;
            Debug.Log("Set currentObject to " + state.changeStateObject.name);  

            state.changeStateObject.SetActive(true);

            // Set currentObject in the changed object too
            if (state.changeStateObject.GetComponent<Interactable>() != null) {
                    state.changeStateObject.GetComponent<Interactable>().setCurrentObject(currentObject);
                    Debug.Log("Set currentObject in " + state.changeStateObject.name + " to " + currentObject.name);
            }
            else {
                    state.changeStateObject.GetComponentInParent<Interactable>().setCurrentObject(currentObject);
                    Debug.Log("Set currentObject in " + state.changeStateObject + "'s parent to " + currentObject.name);
            }
                
            Debug.Log("Enabling new object " + state.changeStateObject.name);
        }

        // Replace look dialogue, if needed
        if (state.newLookDialogueJSON != null)
        {
            // If the changeStateObject is a child of this, then change this look dialogue
            if (state.changeStateObject.transform.IsChildOf(this.gameObject.transform)) 
            {
                lookDialogueJSON = state.newLookDialogueJSON;
            }
            // If the changeStateObject is not a child of this, then change that object's look dialogue
            // I know this is jank but whatever
            else
            {
                state.changeStateObject.transform.parent.GetComponent<Interactable>().lookDialogueJSON = state.newLookDialogueJSON;
            }
        }
    }

    private void StateChange()
    {
        Item activeItem = null;
        bool allObjectsEnabled = true;
        bool stateChanged = false;

        activeItem = GetActiveItem();

        for (int i = 0; i < states.Count && !stateChanged; i++)
        {
            // If there's an active item, check to see if state has same item, or if no active item, check if state has no items listed
            if (states[i].interactionItem == activeItem)
            {
                // Initialize allObjectsEnabled
                allObjectsEnabled = true;

                // Verify that each object is enabled
                for (int j = 0; j < states[i].enabledObjects.Count; j++)
                {
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
                    //this.gameObject.SetActive(false);
                    currentObject.SetActive(false);
                    Debug.Log("Disabling current object " + currentObject.name);

                    // Pick up object, if needed
                    if (states[i].pickUpItem != null)
                    {
                        for (int j = 0; j < states[i].pickUpItem.Count; j++)
                        {
                            inventory.AddItem(states[i].pickUpItem[j]);
                            Debug.Log("Picked Up Item " + states[i].pickUpItem[j].name);
                        }
                    }

                    // Replace look dialogue, if needed
                    if (states[i].newLookDialogueJSON != null)
                    {
                        lookDialogueJSON = states[i].newLookDialogueJSON;
                    }

                    // Enable the game object in that state, if there is one
                    if (states[i].changeStateObject != null)
                    {
                        currentObject = states[i].changeStateObject;
                        Debug.Log("Set currentObject to " + states[i].changeStateObject.name);  

                        states[i].changeStateObject.SetActive(true);

                        // Set currentObject in the changed object too
                        if (states[i].changeStateObject.GetComponent<Interactable>() != null) {
                             states[i].changeStateObject.GetComponent<Interactable>().setCurrentObject(currentObject);
                             Debug.Log("Set currentObject in " + states[i].changeStateObject.name + " to " + currentObject.name);
                        }
                        else {
                             states[i].changeStateObject.GetComponentInParent<Interactable>().setCurrentObject(currentObject);
                             Debug.Log("Set currentObject in " + states[i].changeStateObject + "'s parent to " + currentObject.name);
                        }
                           
                        Debug.Log("Enabling new object " + states[i].changeStateObject.name);
                    }

                    stateChanged = true;
                }
            }
        }
        
        // After trying item, it should not be active anymore
        inventory.disableAllActiveItems();
    }

    public void Look()
    {
        UIDialogueManager.GetInstance().Dialogue(lookDialogueJSON);
        Debug.Log("You Looked!");
        
    }

    public void setCurrentObject(GameObject newCurrentObject)
    {
        currentObject = newCurrentObject;
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
