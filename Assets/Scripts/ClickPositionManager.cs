using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPositionManager : MonoBehaviour {

    public GameEvent movePlayer;
    public Vector3Variable movePlayerLocation;

    void Update() {
        if (Input.GetMouseButton(0)) {
            Vector3 clickPosition = -Vector3.one;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                clickPosition = hit.point;

                if (hit.collider.gameObject.tag == "Walkable")
                {
                    movePlayerLocation.SetValue(clickPosition);
                    movePlayer.Raise();
                }
            }

            Debug.Log("Position: " + clickPosition + " Collider: " + hit.collider.gameObject.name);            
        }       
    }
}
