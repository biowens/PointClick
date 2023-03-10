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
    public List<ItemInfo> Items;

    [SerializeField]
    private GameEvent changedActiveItem;

    public void AddItem(Item item) 
    {
        ItemInfo temp = new ItemInfo();
        temp.item = item;
        temp.active = false;

        Items.Add(temp);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(Items.Find(x => x.item = item));
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

    public void disableAllActiveItems() 
    {
        for (int i=0; i<Items.Count; i++)
        {
            Items[i].active = false;
        }
        changedActiveItem.Raise();
    }
}
