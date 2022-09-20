using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "Items_")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemID;

    public int shopBuyPrice;
    public int shopSellPrice;

    [TextArea(3,10)]
    public string itemExplain;
    [TextArea(3, 10)]
    public string itemEffect;
    public Sprite itemIcon;
    public int itemAmount;
    public ItemType itemType;
    public WeaponType weaponType;


    public int consumeHP;
    public int consumeSP;
    public int consumeBP;

    public string weaponSpeed;
    public int weaponAtk;
    public int weaponDef;
    public int weaponEva;

    public enum ItemType
    {
        Etc,
        Consume,
        Weapon,
        Quest
    }

    public enum WeaponType
    {
        twoHandedSword,
        oneHandedSword,
        Spear,
        Scythe
    }

    public virtual void Give(Quest quest) { }
    public void Consume()
    {
        GameManager.instance.quest.UpdateUseItems(this);
        if (GameManager.instance.quest.trackingQuest != null)
        {
            if (GameManager.instance.quest.trackingQuest.goalType == GoalType.Use)
            {
                GameManager.instance.quest.UpdateTrackingQuest();
            }
        }

        Debug.Log("Consume");
        GameManager.instance.playerStatus.playerCurrentHp += consumeHP;
        //GameManager.instance.playerStatus.playerBP += consumeBP;
        GameManager.instance.playerStatus.playerCurrentSp += consumeSP;

        if (GameManager.instance.playerStatus.playerCurrentHp >= GameManager.instance.playerStatus.playerHP)
        {
            GameManager.instance.playerStatus.playerCurrentHp = GameManager.instance.playerStatus.playerHP;
            Debug.Log("HPvn");
        }


        if (GameManager.instance.playerStatus.playerCurrentSp >= GameManager.instance.playerStatus.playerSP)
        {
            GameManager.instance.playerStatus.playerCurrentSp = GameManager.instance.playerStatus.playerSP;
            Debug.Log("SPf");
        }
    }
}


