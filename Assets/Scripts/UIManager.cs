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
            
            Debug.Log(button);
            button.onClick.AddListener(() => {
                    this.itemClick(itemIndex);
                });

            newButton.GetComponentInChildren<TMP_Text>().text = inventory.Items[itemIndex].item.itemName;
        }
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
        inventory.disableAllActiveItems();
        inventory.Items[index].active = true;
        updateActiveItemText();
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
