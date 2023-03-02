using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameEvent movePlayer;
    public Vector3Variable moveLocation;

    public GameObject standPoint;

    public void Interact()
    {
        moveLocation.SetValue(standPoint.transform.position);
        movePlayer.Raise();

        Debug.Log("You Interacted!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
