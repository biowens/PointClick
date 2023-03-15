using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public Item item;
    public bool active;
}

[CreateAssetMenu]
public class ItemCollection : ScriptableObject
{
    [SerializeField]
    private GameEvent changedActiveItem;

    public List<ItemInfo> Items;
 
    // Adds item at the end of the list
    public void AddItem(Item item) 
    {
        ItemInfo temp = new ItemInfo();
        temp.item = item;
        temp.active = false;

        Items.Add(temp);
    }

    // Adds item after the index
    public void AddItem(Item item, int index) 
    {
        ItemInfo temp = new ItemInfo();
        temp.item = item;
        temp.active = false;
        
        // added +1 to index so item is added after the item, not before
        Items.Insert(index+1, temp);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(Items.Find(x => x.item = item));
    }

    public void RemoveItem(int index)
    {
        Items.Remove(Items[index]);
    }

    public void RemoveActiveItem()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].active)
                Items.RemoveAt(i);
        }
    }

    public void ReplaceItem(Item toReplace, Item replaceWith)
    {
        int index = Items.FindIndex(x => x.item = toReplace);
        if (index >= 0)
        {   
            ItemInfo temp = new ItemInfo();
            temp.item = replaceWith;
            temp.active = false;

            Items[index] = temp;
        } 
        else
        {
            Debug.Log("Item " + toReplace.name + " does not exist in this item collection");
        }
    }

    public void ReplaceItem(int toReplace, Item replaceWith)
    {
        Items[toReplace].item = replaceWith;
    }


    public void SetValue(List<ItemInfo> items)
    {
        Items.Clear();
        Items = items;
    }

    public void SetValue(ItemCollection items)
    {
        Items.Clear();
        Items = items.Items;
    }

    public void setActive(int index)
    {
        disableAllActiveItems();
        Items[index].active = true;
        changedActiveItem.Raise();
    }

    // Returns the index of the current active item. If no active item, returns -1.
    public int getActiveIndex()
    {
        for (int i=0; i<Items.Count; i++)
        {
            if (Items[i].active)
                return i;
        }
        return -1;
    }

    // Tries to combine two items in the inventory
    public void combineItems(int indexClicked, int indexActive)
    {
        string debugString = "";

        Item itemClicked = Items[indexClicked].item;
        Item itemActive = Items[indexActive].item;
        
        Debug.Log("Attempting to combine selected item " + itemClicked.name + " with active item " + itemActive.name);
        CombineItem result = itemClicked.getCombinedItem(itemActive);

        if (result != null)
        {
            // Insert item after clicked
            for (int i = 0; i < result.resultItem.Count; i++)
            {
                AddItem(result.resultItem[i], indexClicked);
                debugString += result.resultItem[i] + " ";
            }
            Debug.Log("Added result items: " + debugString);

            // If the active item is after the clicked item, adjust index based on number of added items
            if (indexActive > indexClicked)
            {
                indexActive += result.resultItem.Count;
            }

            // Destroy the active item if necessary
            if (result.destroyCombinedItem) 
            {
                Debug.Log("Removing active item: " + Items[indexActive].item.name);
                RemoveItem(indexActive);

                // If the clicked item is after the active item, adjust clicked index based on the removal of the active item
                if (indexClicked > indexActive)
                {
                    indexClicked -= 1;
                }
            }

            // Destroy the clicked item if necessary
            if (result.destroyThisItem)
            {
                Debug.Log("Removing clicked item: " + Items[indexClicked].item.name);
                RemoveItem(indexClicked);
            }
        }
        else
        {
            Debug.Log("Could not combine " + itemClicked.name + " with " + itemActive.name);
        }
        disableAllActiveItems();
    }

    public void disableAllActiveItems() 
    {
        for (int i=0; i<Items.Count; i++)
        {
            Items[i].active = false;
        }
        changedActiveItem.Raise();
    }
}
