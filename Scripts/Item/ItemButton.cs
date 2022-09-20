using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ItemButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IMoveHandler, IDeselectHandler
{
    public GameObject item_00;
    public GameObject arrowIcon;
    public Text itemNameText;
    public Text ItemCountText;
    public Image itemSprite;

    private Item item;

    private float posX;

    private void Awake()
    {
        posX = gameObject.GetComponent<RectTransform>().anchoredPosition.x;
        arrowIcon = gameObject.transform.GetChild(4).gameObject;
    }

    public void GetItem(Item _item)
    {
        item = _item;
        itemNameText.text = _item.itemName;
        ItemCountText.text = "*" + _item.itemAmount;
        itemSprite.sprite = _item.itemIcon;
    }
    public void RemoveItem()
    {
        item = null;
        itemNameText.text = "";
    }
    public void ActivePanel()
    {
        Inventory inven = GameManager.instance.inven;
        inven.ItemPanel.SetActive(true);
        inven.ItemPanel.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
        inven.ItemPanel.transform.GetChild(1).GetComponent<Text>().text = item.itemType.ToString();
        inven.ItemPanel.transform.GetChild(2).GetComponent<Text>().text = item.itemExplain;
        inven.ItemPanel.transform.GetChild(4).GetComponent<Image>().sprite = item.itemIcon;
    }
    public void OnClick(GameObject button)
    {
        Inventory inven = GameManager.instance.inven;
        inven.ChangeItemTabsImage();

        GameManager.instance.State = UiState.Inventory;

        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);

        if (item.itemType == Item.ItemType.Consume || item.itemType == Item.ItemType.Weapon)
        {
            inven.ConsumeItem = item;
            inven.State = InventoryState.ItemConsumeOn;
            inven.prevSelectedBtn = gameObject;
            Debug.Log("ItemButton");
        }
        else
        {
            inven.State = InventoryState.ItemsOn;
        }
        for (int i = 0; i < inven.itemLists.Count; i++)
        {
            if (button == inven.itemLists[i])
            {
                inven.selectedItem = i;
            }
        }

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        /*int x = gameObject.transform.GetSiblingIndex();
        Debug.Log(x);
        GameManager.instance.inven.InventorySnap.GetComponent<ScrollSnap>().ChangePage(x);*/
        arrowIcon.SetActive(true);
        //itemNameText.color = new Color(87/255f, 108/255f, 125/255f);
        //ItemCountText.color = new Color(87/255f, 108 / 255f, 125 / 255f);
        LeanTween.moveX(arrowIcon.GetComponent<RectTransform>(), -368, 0.7f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1).setIgnoreTimeScale(true);
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), posX + 20, 0.15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
        ActivePanel();
    }

    public void OnMove(AxisEventData eventData)
    {
        Inventory inven = GameManager.instance.inven;
        int x = item_00.transform.GetSiblingIndex();
        Debug.Log("인벤토리 번호" + x);
        if (eventData.moveDir == MoveDirection.Up && x > 0)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
            //inven.InventorySnap.GetComponent<ScrollSnap>().ChangePage(x - 1);
            inven.InventorySnap.GetComponent<ScrollSnap>().PreviousScreen();
        }
        else if (eventData.moveDir == MoveDirection.Down && inven.itemLists[0] != item_00)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
            //inven.InventorySnap.GetComponent<ScrollSnap>().ChangePage(x + 1);
            inven.InventorySnap.GetComponent<ScrollSnap>().NextScreen()
;       }
        else if (eventData.moveDir == MoveDirection.Left)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }
        else if (eventData.moveDir == MoveDirection.Right)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }

    }

    public void OnDeselect(BaseEventData eventData)
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), posX, 0.15f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
        arrowIcon.SetActive(false);
        LeanTween.cancel(arrowIcon);
        itemNameText.color = new Color(1, 1, 1);
        ItemCountText.color = new Color(1, 1, 1);
        arrowIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-375f, 0);
    }
}