using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public FloatVariable mouseInput;

    public GameObjectVariable clickObject;
    public Vector3Variable clickLocation;

    public GameEvent movePlayerEvent;
    public Vector3Variable movePlayerLocation;

    // Update is called once per frame
    public void ManageInteract()
    {
        // If left click, interactions will be move and use
        if (mouseInput.Value == 0)
        {
            switch (clickObject.Value.tag)
            {
                case "Walkable":
                    MovePlayer();
                    break;
                case "Interactable":
                    Interact();
                    break;
                default:
                    // code block
                    break;
            }
        }
        // If right click, interactions will be look
        else if (mouseInput.Value == 1)
        {
            if (clickObject.Value.tag == "Interactable")
                Look();
        }
        

    }

    void MovePlayer()
    {
        movePlayerLocation.SetValue(clickLocation.Value);
        movePlayerEvent.Raise();
    }

    void Interact() 
    {
        if (clickObject.Value.GetComponent<Interactable>() != null)
            clickObject.Value.GetComponent<Interactable>().Interact();
        else
            clickObject.Value.GetComponentInParent<Interactable>().Interact();
    }

    void Look()
    {
        if (clickObject.Value.GetComponent<Interactable>() != null)
            clickObject.Value.GetComponent<Interactable>().Look();
        else
            clickObject.Value.GetComponentInParent<Interactable>().Look();
    }
}
