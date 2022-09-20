using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTabButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler,IMoveHandler
{
    public void OnClick(GameObject button)
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        GameManager.instance.State = UiState.Inventory;
        Inventory inventory = GameManager.instance.inven;
        if (inventory.inventoryTabList.Count != 0)
        {
            inventory.ActiveItemButtons();
            inventory.State = InventoryState.ItemsOn;
            for (int i = 0; i < inventory.ItemTabs.Length; i++)
            {
                if (inventory.ItemTabs[i] == button)
                {
                    inventory.selectedTab = i;
                    GameManager.instance.State = UiState.Inventory;
                }
            }
            if (inventory.itemLists.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(inventory.itemLists[inventory.selectedItem / 2].transform.GetChild(inventory.selectedItem % 2).gameObject);
            }
        }
        else
        {
            inventory.State = InventoryState.TabsOn;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Inventory inventory = GameManager.instance.inven;
        /*for (int i = 0; i < inventory.ItemTabs.Length; i++)
        {
            if (inventory.ItemTabs[i] == eventData.selectedObject)
            {
                inventory.selectedTab = i;
            }
        }*/
        int x = gameObject.transform.GetSiblingIndex();
        inventory.selectedTab = x;
        inventory.ShowItem();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Left)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }
        else if (eventData.moveDir == MoveDirection.Right)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }
    }
}