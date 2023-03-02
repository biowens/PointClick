using UnityEngine;

[CreateAssetMenu]
public class GameObjectVariable : ScriptableObject
{
    public GameObject Value;

    public void SetValue(GameObject value)
    {
        Value = value;
    }

    public void SetValue(GameObjectVariable value)
    {
        Value = value.Value;
    }
}
