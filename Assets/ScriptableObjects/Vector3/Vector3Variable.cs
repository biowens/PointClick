using UnityEngine;

[CreateAssetMenu]
public class Vector3Variable : ScriptableObject
{
    public Vector3 Value;

    public void SetValue(Vector3 value)
    {
        Value = value;
    }

    public void SetValue(Vector3Variable value)
    {
        Value = value.Value;
    }
}
