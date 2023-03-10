using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    void Update() {
        // If right/left click is detected, click lock is not active, and pointer is not over UI object
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !lockClick && !EventSystem.current.IsPointerOverGameObject()) {
            // Initializes clickPosition to identify false clicks
            Vector3 clickPosition = -Vector3.one;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                // Save location of raycast hit
                clickPosition = hit.point;
                clickLocation.SetValue(clickPosition);

                // Save object that was interacted with
                clickObject.SetValue(hit.collider.gameObject);

                // Save mouse input
                if (Input.GetMouseButtonDown(0))
                    mouseInput.SetValue(0);
                else
                    mouseInput.SetValue(1);

                // Send out valid click event
                validClick.Raise();
            }

            Debug.Log("Position: " + clickPosition + " Collider: " + hit.collider.gameObject.name);            
        }       
    }

    public void SetLockClick(bool value) 
    {
        lockClick = value;
    }
}
