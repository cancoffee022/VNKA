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

    public List<Item> inventoryItemList;//�÷��̾ ������ ������ ����Ʈ
    public List<Item> inventoryTabList;//������ �ǿ� ���� ������ ������ ����Ʈ
    public List<Item> EquipItemList;//����� �����۸���Ʈ
    public Item ConsumeItem;//�Һ��� ������
    public Text NoItem;

    public Transform character;

    public GameObject InventoryUI;
    public GameObject InventorySnap;
    public GameObject ItemPanel;
    public GameObject itemConsumeAcceptButton;
    public GameObject itemConsumeDenyButton;
    public Text itemConsumeAskText;

    public GameObject prevSelectedBtn;//�Һ� ��� �������� �����ߴٰ� ��� ������ �̺�Ʈ �ý��� ��� ���ư�

    public Transform InventorySnap_List;
    public Transform ItemConsumeAction;

    public GameObject ItemList;//������ ������
    public List<GameObject> itemLists;//������� ������ġ

    public GameObject prevSelectedTabGameObject;//����� �� ����
    public GameObject prevSelectedItemButton;//����� ������ ��ư ����

    public GameObject[] ItemTabs;//������ tabs ���ӿ�����Ʈ

    public int selectedTab;
    public int selectedItem;

    public Sprite selectedImage;
    public Sprite deSelectedImage;

    public bool isShowItemTabs;//���� ������ �� ������ �����־�����
    public bool isShowItemDescrip;//���� ������ �����г� Ȱ��ȭ��

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
    }//������ ã��
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
        for (int i = 0; i < theDatabase.items.Count; i++) // �����ͺ��̽� ������ �˻�
        {
            if (_itemID == theDatabase.items[i].itemID) // �����ͺ��̽��� ������ �߰�
            {
                for (int j = 0; j < inventoryItemList.Count; j++) // ����ǰ�� �߰��� �����۰� �������� �ִ��� �˻�
                {
                    if (inventoryItemList[j].itemID == _itemID) // ����ǰ�� ���� �������� ������ ������ ��ȯ��Ŵ
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
                inventoryItemList.Add(theDatabase.items[i]); // ����ǰ�� �ش� ������ �߰�
                inventoryItemList[inventoryItemList.Count - 1].itemAmount = _count;
                return;
            }
        }
        Debug.LogError("�����ͺ��̽���" + _itemID + "�� ���� �������� ������������");
    } // ������ ȹ��
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

    } // ������ ����

    public void RemoveItemList()
    {
        foreach (var items in itemLists)
        {
            Destroy(items);
        }
        itemLists.Clear();
    }//������ ����
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
        }//������ ���� ���� �������� �з��ϰ�, �κ��丮�� ����Ʈ�� �߰�

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
    }//������ Ȱ��ȭ(inventoryTabList�� ���ǿ� �´� �����۵鸸 �־��ְ�, �κ��丮 ���Կ� ���

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
    }//�κ��丮 State ����� ȣ��
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
            itemConsumeAskText.text = "�ش� �������� �����Ϸ� ���������� �̵��մϴ�";

            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(OpenPlayerStatus);
        }
        else if (ConsumeItem.itemType == Item.ItemType.Consume)
        {
            itemConsumeAskText.text = ConsumeItem.itemName+" ��(��)" +"\n����Ͻðڽ��ϱ�?";
            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(() => ConsumeItem.Consume());
            itemConsumeAcceptButton.GetComponent<Button>().onClick.AddListener(ConsumeButtonOnClick);
        }
        else
            itemConsumeAskText.text = "�ش� �������� ����Ͻðڽ��ϱ�?";
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