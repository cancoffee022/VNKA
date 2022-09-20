using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect
{
    WeaponAtk,
    WeaponDef,
    WeaponEvasion,
    Attack,
    Defense,
    Evasion,
    Hp,
    Sp,
}

[CreateAssetMenu(menuName ="LevelUp/Effect",fileName ="_LevelEffect")]
public class WeaponLevelEffect : ScriptableObject
{
    public Effect effect;

    public int effectAmount;

    public void GiveEffect(int weaponIdx)
    {
        if (effect > Effect.WeaponEvasion)
        {
            switch (effect)
            {
                case Effect.Attack:
                    GameManager.instance.playerStatus.playerAtk += effectAmount;
                    break;
                case Effect.Defense:
                    GameManager.instance.playerStatus.playerDef += effectAmount;
                    break;
                case Effect.Evasion:
                    GameManager.instance.playerStatus.playerEva += effectAmount;
                    break;
                case Effect.Hp:
                    GameManager.instance.playerStatus.playerHP += effectAmount;
                    break;
                case Effect.Sp:
                    GameManager.instance.playerStatus.playerHP += effectAmount;
                    break;
                
            }
            Debug.Log(effect);
        }
        else if(effect<Effect.Attack)
        {
            
            WeaponStat(weaponIdx, (int)effect);
            Debug.Log(weaponIdx + " " + effect);
        }
        Debug.Log("weponIdx" + weaponIdx);
    }

    private void WeaponStat(int weaponIdx,int effect)
    {
        switch (weaponIdx)
        {
            case 0:
                GameManager.instance.playerStatus.twoHandAbilityStat[effect] += effectAmount;
                break;
            case 1:
                GameManager.instance.playerStatus.oneHandAbilityStat[effect] += effectAmount;
                break;
            case 2:
                GameManager.instance.playerStatus.scytheAbilityStat[effect] += effectAmount;
                break;
            case 3:
                GameManager.instance.playerStatus.spearAbilityStat[effect] += effectAmount;
                break;
            
        }
    }
}
