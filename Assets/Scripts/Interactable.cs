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
    public GameEvent movePlayer;
    public Vector3Variable moveLocation;

    public GameObject standPoint;

    public List<SpawnerRecord> states;
    
    public void Interact()
    {
        moveLocation.SetValue(standPoint.transform.position);
        movePlayer.Raise();

        Debug.Log("You Interacted!");
    }

    public void Look()
    {
        Debug.Log("You Looked!");
    }

}
