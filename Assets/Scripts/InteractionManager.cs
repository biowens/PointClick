using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public GameObjectVariable clickObject;
    public Vector3Variable clickLocation;

    public GameEvent movePlayerEvent;

    // Update is called once per frame
    public void interactionManager()
    {
        switch(clickObject.Value.tag) 
        {
        case "Walkable":
            movePlayer();
            break;
        case "Interactable":
            interact();
            break;
        default:
            // code block
            break;
        }

    }

    void movePlayer()
    {
        movePlayerEvent.Raise();
    }

    void interact() 
    {
        clickObject.Value.GetComponent<Interactable>().Interact();
    }
}
