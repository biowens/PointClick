using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum ClickType 
{
    Select,
    Look
}

public class ClickPositionManager : MonoBehaviour {

    public bool lockClick;

    public GameEvent validClick;

    public FloatVariable mouseInput;
    public Vector3Variable clickLocation;
    public GameObjectVariable clickObject;

    void Awake() 
    {
        // Init click lock to off
        lockClick = false;
    }

    public void SelectClick(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            IdentifyValidWorldSelect(ClickType.Select);
        }
    }

    public void LookClick(InputAction.CallbackContext context)
    {
        if (context.performed) {
            IdentifyValidWorldSelect(ClickType.Look);
        }
    }

    public void IdentifyValidWorldSelect(ClickType clickType) 
    {
        if (!lockClick && !EventSystem.current.IsPointerOverGameObject()) 
        {
            clickLocation.SetValue(-Vector3.one);
            clickObject.Value = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                // Save location of raycast hit
                clickLocation.SetValue(hit.point);

                // Save object that was interacted with
                clickObject.SetValue(hit.collider.gameObject);
                
                // Save mouse input
                if (clickType == ClickType.Select)
                    mouseInput.SetValue(0);
                else if (clickType == ClickType.Look)
                    mouseInput.SetValue(1);

                // Send out valid click event
                validClick.Raise();
            }

            Debug.Log("Position: " + clickLocation.Value + " Collider: " + clickObject.Value);  
        }
    }

    public void SetLockClick(bool value) 
    {
        lockClick = value;
    }
}
