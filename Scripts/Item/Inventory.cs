using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using System.Linq;

public enum InventoryState
{
    InventoryOff,
    TabsOn,
    ItemsOn,
    ItemConsumeOn
}
public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField]
    private InventoryState state;
    public InventoryState State
    {
        get => state;
        set
        {
            InventoryState prevState = state;
            state = value;
            if (value != prevState)
            {
                UpdateButtons();
            }
        }
    }

    public bool IsInventoryActive => State != InventoryState.InventoryOff;
    public bool IsTabsActive => State == InventoryState.TabsOn;
    public bool IsItemActive => State == InventoryState.ItemsOn;
    public bool IsItemConsume => State == InventoryState.ItemConsumeOn;

    private ItemDatabase theDatabase;
    //private TPSCharacterController characterController;

    public List<Item> inventoryItemList;//플레이어가 소지한 아이템 리스트
    public List<Item> inventoryTabList;//선택한 탭에 따라 보여질 아이템 리스트
    public List<Item> EquipItemList;//장비한 아이템리스트
    public Item ConsumeItem;//소비할 아이템
    public Text NoItem;

    public Transform character;

    public GameObject InventoryUI;
    public GameObject InventorySnap;
    public GameObject ItemPanel;
    public GameObject itemConsumeAcceptButton;
    public GameObject itemConsumeDenyButton;
    public Text itemConsumeAskText;

    public GameObject prevSelectedBtn;//소비나 장비 눌렀을때 저장했다가 취소 누르면 이벤트 시스템 얘로 돌아감

    public Transform InventorySnap_List;
    public Transform ItemConsumeAction;

    public GameObject ItemList;//아이템 프리펩
    public List<GameObject> itemLists;//프리펩들 저장위치

    public GameObject prevSelectedTabGameObject;//실행된 탭 저장
    public GameObject prevSelectedItemButton;//설명된 아이템 버튼 저장

    public GameObject[] ItemTabs;//아이템 tabs 게임오브젝트

    public int selectedTab;
    public int selectedItem;

    public Sprite selectedImage;
    public Sprite deSelectedImage;

    public bool isShowItemTabs;//현재 아이템 탭 아이템 보여주었을시
    public bool isShowItemDescrip;//현재 아이템 설명패널 활성화시

    public PlayerStatus PS;

    void Start()
    {
        instance = this;
        inventoryTabList = new List<Item>();
        theDatabase = FindObjectOfType<ItemDatabase>();

        foreach (var tab in ItemTabs)
        {
            tab.GetComponent<Button>().onClick.AddListener(ChangeItemTabsImage);
        }

    }
    public int SearchItemsAmount(string _itemName)
    {
        Debug.Log("serachItems" + _itemName);
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemName == _itemName)
            {
                return inventoryItemList[i].itemAmount;
            }
        }
        return 0;
    }//아이템 찾기
    public bool SearchItemsValue(Item _item, int _count)
    {
        bool isTrue = false;
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i] == _item)
            {
                isTrue = (inventoryItemList[i].itemAmount >= _count) ? true : false;
            }
        }
        return isTrue;
    }
    public void GetAnItem(int _itemID, int _count)
    {
        for (int i = 0; i < theDatabase.items.Count; i++) // 데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.items[i].itemID) // 데이터베이스에 아이템 발견
            {
                for (int j = 0; j < inventoryItemList.Count; j++) // 소지품에 발견한 아이템과 같은것이 있는지 검색
                {
                    if (inventoryItemList[j].itemID == _itemID) // 소지품에 같은 아이템이 있으면 개수만 변환시킴
                    {
                        //QuestSystem.Instance.ReceiveReport("Item", (theDatabase.items[i].itemName), _count);
                        if (inventoryItemList[j].itemType == Item.ItemType.Consume || inventoryItemList[j].itemType == Item.ItemType.Quest || inventoryItemList[j].itemType == Item.ItemType.Etc)
                        {
                            inventoryItemList[j].itemAmount += _count;
                            //Debug.Log(inventoryItemList[j].itemAmount);
                        }
                        else
                        {
                            inventoryItemList.Add(theDatabase.items[i]);
                        }
                        return;
                    }
                }
                //QuestSystem.Instance.ReceiveReport("Item", (theDatabase.items[i].itemName), _count);
                inventoryItemList.Add(theDatabase.items[i]); // 소지품에 해당 아이템 추가
                inventoryItemList[inventoryItemList.Count - 1].itemAmount = _count;
                return;
            }
        }
        Debug.LogError("데이터베이스에" + _itemID + "을 가진 아이템이 존재하지않음");
    } // 아이템 획득
    public void RemoveAnItem(int _itemID, int _count)
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (_itemID == inventoryItemList[i].itemID)
            {
                inventoryItemList[i].itemAmount -= _count;
                if (inventoryItemList[i].itemAmount <= 0)
                {
                    inventoryItemList.RemoveAt(i);
                    break;
                }
            }
        }
        ShowItem();

    } // 아이템 제거

    public void RemoveItemList()
    {
        foreach (var items in itemLists)
        {
            Destroy(items);
        }
        itemLists.Clear();
    }//프리펩 제거
    public void ShowItem()
    {
        RemoveItemList();
        inventoryTabList.Clear();
        switch (selectedTab)
        {
            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Etc == inventoryItemList[i].itemType)
                    {
                        inventoryTabList.Add(inventoryItemList[i]);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Consume == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Weapon == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        }//각자의 탭을 따라서 아이템을 분류하고, 인벤토리탭 리스트에 추가

        if (inventoryTabList.Count == 0)
        {
            NoItem.gameObject.SetActive(true);
        }
        else
            NoItem.gameObject.SetActive(false);

        if (inventoryTabList.Count % 2 == 0)
        {
            for (int i = 0; i < inventoryTabList.Count / 2; i++)
            {
                itemLists.Add(Instantiate(ItemList, InventorySnap_List.transform));
            }
        }
        else
        {
            for (int i = 0; i <= inventoryTabList.Count / 2 + 1; i++)
            {
                if (i == inventoryTabList.Count / 2 + 1)
                {
                    itemLists[i - 1].transform.GetChild(1).gameObject.SetActive(false);
                    break;
                }
                itemLists.Add(Instantiate(ItemList, InventorySnap_List.transform));
            }
        }
        int k = 0;
        foreach (var items in itemLists)
        {
            items.transform.GetChild(0).GetComponent<ItemButton>().GetItem(inventoryTabList[k]);
            k++;
            if (inventoryTabList.Count - k > 0)
            {
                items.transform.GetChild(1).GetComponent<ItemButton>().GetItem(inventoryTabList[k]);
                k++;
            }
        }
        for (int i = 0; i < itemLists.Count; i++)
        {
            Navigation buttonNav0 = itemLists[i].transform.GetChild(0).GetComponent<Button>().navigation;
            Navigation buttonNav1 = itemLists[i].transform.GetChild(1).GetComponent<Button>().navigation;
            if (i > 0)
            {
                buttonNav0.selectOnUp = itemLists[i - 1].transform.GetChild(0).GetComponent<Button>();
                buttonNav1.selectOnUp = itemLists[i - 1].transform.GetChild(1).GetComponent<Button>();
                //Debug.Log(buttonNav0.selectOnUp.transform.position + "" + buttonNav1.selectOnUp.transform.position);
            }
            if (itemLists.Count - 1 > i)
            {
                buttonNav0.selectOnDown = itemLists[i + 1].transform.GetChild(0).GetComponent<Button>();
                buttonNav1.selectOnDown = itemLists[i + 1].transform.GetChild(1).GetComponent<Button>();
                //Debug.Log(buttonNav0.selectOnDown.transform.position + "" + buttonNav1.selectOnDown.transform.position);
            }
            itemLists[i].transform.GetChild(0).GetComponent<Button>().navigation = buttonNav0;
            itemLists[i].transform.GetChild(1).GetComponent<Button>().navigation = buttonNav1;
        }
        isShowItemTabs = true;
    }//아이템 활성화(inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력

    public void UpdateButtons()
    {
        if (State == InventoryState.InventoryOff)
        {
            selectedTab = 0;
            if (inventoryTabList.Count != 0)
            {
                InventorySnap.GetComponent<ScrollSnap>().ChangePage(0);
            }
            foreach (var tab in ItemTabs)
            {
                tab.GetComponent<Image>().sprite = deSelectedImage;
            }
            InActiveItemButtons();
        }
        if (State == InventoryState.TabsOn)
        {
            ActiveItemTabs();
            if (itemLists.Count == 0)
            {
                InActiveItemButtons();
            }
            InActiveConsumeButton();
            EventSystem.current.SetSelectedGameObject(ItemTabs[selectedTab]);
            prevSelectedBtn = null;
        }
        if (State == InventoryState.ItemsOn)
        {
            ActiveItemButtons();
            InActiveConsumeButton();
            if (itemLists.Count > 0)
            {
                Debug.Log("gma..");
                EventSystem.current.SetSelectedGameObject(itemLists[0].transform.GetChild(0).gameObject);
            }
            if (prevSelectedBtn != null)
            {
                EventSystem.current.SetSelectedGameObject(prevSelectedBtn);
            }
            LeanTween.cancel(itemConsumeAcceptButton.transform.GetChild(1).gameObject);
            LeanTween.cancel(itemConsumeDenyButton.transform.GetChild(1).gameObject);
            itemConsumeDenyButton.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-59.4f, 2.95f);
            itemConsumeAcceptButton.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-59.4f, 4.3f);
            itemConsumeDenyButton.transform.GetChild(0).GetComponent<Text>().color = new Color(255 / 255, 255 / 255, 255 / 255);
            itemConsumeAcceptButton.transform.GetChild(0).GetComponent<Text>().color = new Color(255 / 255, 255 / 255, 255 / 255);
            itemConsumeDenyButton.transform.GetChild(1).gameObject.SetActive(false);
            itemConsumeAcceptButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (State == InventoryState.ItemConsumeOn)
        {
            ActiveConsumeButton();
            //InActiveItemButtons();
            //InActiveItemTabs();
            EventSystem.current.SetSelectedGameObject(itemConsumeDenyButton);
        }
    }//인벤토리 State 변경시 호출
    public void ActiveItemButtons()
    {
        if (inventoryTabList.Count != 0)
        {
            //EventSystem.current.SetSelectedGameObject(itemLists[0].transform.GetChild(0).gameObject);
            ActiveItemTabs();
            foreach (var items in itemLists)
            {
                items.transform.GetChild(0).GetComponent<Button>().interactable = true;
                items.transform.GetChild(1).GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            ActiveItemTabs();
        }
    }
    public void InActiveItemButtons()
    {
        foreach (var items in itemLists)
        {
            /* items.transform.GetChild(0).GetComponent<Button>().interactable = false;
             items.transform.GetChild(1).GetComponent<Button>().interactable = false;*/
        }
        ItemPanel.SetActive(false);
    }
    public void ActiveItemTabs()
    {
        //state = InventoryState.TabsOn;
        //EventSystem.current.SetSelectedGameObject(ItemTabs[0]);
        foreach (var items in ItemTabs)
        {
            items.GetComponent<Button>().interactable = true;
        }
    }
    public void ActiveConsumeButton()
    {
        ItemConsumeAction.gameObject.SetActive(true);

        itemConsumeDenyButton.GetComponent<Button>().onClick.AddListener(() => { State = InventoryState.ItemsOn; });
        //itemConsumeDenyButton.GetComponent<Button>().onClick.AddListener(CancelItemUse);
        if (ConsumeItem.itemType == Item.ItemType.Weapon)
        {
            itemConsumeAskText.text = "해당 아이템을 장착하러 정보탭으로 이동합니다";

            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(OpenPlayerStatus);
        }
        else if (ConsumeItem.itemType == Item.ItemType.Consume)
        {
            itemConsumeAskText.text = ConsumeItem.itemName+" 을(를)" +"\n사용하시겠습니까?";
            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(() => ConsumeItem.Consume());
            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(ConsumeButtonOnClick);
        }
        else
            itemConsumeAskText.text = "해당 아이템을 사용하시겠습니까?";
    }
    public void CancelItemUse()
    {
        State = InventoryState.ItemsOn;
        EventSystem.current.SetSelectedGameObject(prevSelectedBtn);
    }
    public void InActiveConsumeButton()
    {
        ConsumeItem = null;
        itemConsumeAcceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ItemConsumeAction.gameObject.SetActive(false);
    }
    public void OpenPlayerStatus()
    {
        GameManager.instance.State = UiState.PlayerStatus;
    }

    public void EscapeInventory()
    {
        if ((int)state > 0)
        {
            State--;
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
        }
    }

    public void ChangeItemTabsImage()
    {
        for (int i = 0; i < ItemTabs.Length; i++)
        {
            if (i == selectedTab)
            {
                ItemTabs[i].GetComponent<Image>().sprite = selectedImage;
                continue;
            }
            ItemTabs[i].GetComponent<Image>().sprite = deSelectedImage;
        }
    }
    public void ConsumeButtonOnClick()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[28]);
        RemoveAnItem(ConsumeItem.itemID, 1);
        prevSelectedBtn = null;
        State = InventoryState.ItemsOn;
    }
}