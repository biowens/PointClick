using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombineItem
{
    public Item combineItem;
    public List<Item> resultItem;
    public bool destroyCombinedItem;
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public List<CombineItem> combineItems;

    // Returns the result item if it can be combined
    // Returns null if it cannot be combined
    /*
    public Item getCombinedItem(Item toBeCombined)
    {
        for (int i=0; i < combineItems.Count; i++)
        {
            if (combineItems[i].combineItem == toBeCombined)
                return combineItems[i].resultItem;
        }
        return null;
    }
    */

    public CombineItem getCombinedItem(Item toBeCombined)
    {
        Debug.Log("Trying to combine " + this.name + " and " + toBeCombined.name);

        for (int i = 0; i < combineItems.Count; i++)
        {
            if (combineItems[i].combineItem == toBeCombined)
            {
                Debug.Log("Combine Possible");
                return combineItems[i];
            }
        }
        Debug.Log("Combine Impossible");
        return null;
    }
}
