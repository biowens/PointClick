using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public ItemCollection inventory;

    public GameObject buttonPrefab;
    public GameObject buttonParent;

    public GameObject activeItemText;

    private void Awake()
    {
        updateActiveItemText();
    }

    public void openInventoryButtons()
    {
        buttonParent.SetActive(true);

        initInventoryButtons();
    }

    private void initInventoryButtons()
    {
        destroyAllInvButtons();

        for (int i = 0; i < inventory.Items.Count; i++) 
        {
            int itemIndex = i;

            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            
            Button button = newButton.GetComponent<Button>();
            
            button.onClick.AddListener(() => {
                    this.itemClick(itemIndex);
                });

            newButton.GetComponentInChildren<TMP_Text>().text = inventory.Items[itemIndex].item.itemName;
            newButton.transform.Find("Image").GetComponent<Image>().sprite = inventory.Items[itemIndex].item.itemSprite;
            
            // Change active item button color to indicate which one is active
            if (inventory.Items[itemIndex].active)
            {
                // newButton.GetComponent<Button>().Select();
                var colors = newButton.GetComponent<Button>().colors;
                colors.normalColor = colors.selectedColor;
                newButton.GetComponent<Button>().colors = colors;
            }
        }
        
        updateActiveItemText();
    }

    private void destroyAllInvButtons()
    {
        for (int i = 0; i < buttonParent.transform.childCount; i++) 
        {
            Destroy(buttonParent.transform.GetChild(i).gameObject);
        }
    }

    public void closeInventoryButtons() 
    {
        destroyAllInvButtons();
        buttonParent.SetActive(false);
    }

    public void itemClick(int index)
    {
        // If there is an active item, check if item can be combined
        int activeIndex = inventory.getActiveIndex();

        if (activeIndex >= 0 && activeIndex != index)
        {
            inventory.combineItems(index, inventory.getActiveIndex());
        }
        // If there is no active item, set clicked item to active
        else
        {
            inventory.Items[index].active = true;
        }
        initInventoryButtons();
        Debug.Log("CLICKED");
    }

    public void updateActiveItemText()
    {  
        int index = inventory.getActiveIndex();

        if (index != -1) {
            activeItemText.GetComponent<TMP_Text>().text = inventory.Items[index].item.name;
        }
        else
        {
            activeItemText.GetComponent<TMP_Text>().text = "";
        }
    }
}
