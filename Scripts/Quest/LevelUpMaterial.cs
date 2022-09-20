using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "LevelUpMaterial/material", fileName = "materailItem_")]
public class LevelUpMaterial : ScriptableObject
{
    public int gold;
    public List<Item> item;
    public int[] itemAmount;
    public bool Confirm()
    {
        bool[] isTrue = new bool[itemAmount.Length];
        for (int i = 0; i < itemAmount.Length; i++)
        {
            isTrue[i] = GameManager.instance.inven.SearchItemsValue(item[i], itemAmount[i]);
        }
        bool isContainFalse = isTrue.Contains(false);
        Debug.Log(!isContainFalse);
        return !isContainFalse;
    }
    public void RemoveItems()
    {
        Inventory inven = GameManager.instance.inven;
        for(int i = 0; i < item.Count; i++)
        {
            inven.RemoveAnItem(item[i].itemID, itemAmount[i]);
        }
    }
}
