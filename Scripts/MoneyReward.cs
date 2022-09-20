using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Money", fileName = "MoneyReward")]
public class MoneyReward : Item
{
    public override void Give(Quest quest)
    {
        Debug.Log("°ñµå È¹µæ : " + itemAmount);
    }
}
