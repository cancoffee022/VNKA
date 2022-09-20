using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class WeaponStatusButton : MonoBehaviour, IMoveHandler
{
    public Button weaponCharacterTypeButton;
    public Button weaponLevelUpButton;
    public GameObject weaponEnchantSnap;
    public GameObject weaponLock;

    public Transform weaponEnchantSnapList;

    [SerializeField]
    private int level;
    public int Level
    {
        get => level;
        set
        {
            int prevLevel = level;
            level = value;
            if (value != prevLevel)
            {
                UpdateLevel();
            }
        }
    }
    public Text levelText;

    private ScrollSnap scrollSnap;
    private AbilityUI abilityUI;
    private int idx;

    public string[] levelUpEffect;

    void Start()
    {
        scrollSnap = GameManager.instance.weaponAbility.abilityMenuSnap.GetComponent<ScrollSnap>();
        abilityUI = GameManager.instance.weaponAbility;
        if (weaponLock.activeSelf)
        {
            gameObject.GetComponent<Button>().interactable = false;
            weaponCharacterTypeButton.interactable = false;
            weaponLevelUpButton.interactable = false;
            weaponEnchantSnap.GetComponent<Button>().interactable = false;
        }
        idx = GameManager.instance.weaponAbility.weaponStatus.IndexOf(this.gameObject);
        levelText.text = "Level " + Level;
    }
    void UpdateLevel()
    {
        levelText.text = "Level " + Level;
    }
    public void OnMove(AxisEventData axisEventData)
    {
        if (axisEventData.moveDir == MoveDirection.Right)
        {
            if (!abilityUI.weaponStatus[idx + 1].GetComponent<WeaponStatusButton>().weaponLock.activeSelf && this.gameObject != GameManager.instance.weaponAbility.weaponStatus[0])
                scrollSnap.GetComponent<ScrollSnap>().NextScreen();
            //Debug.Log(axisEventData.moveDir);
        }
        else if (axisEventData.moveDir == MoveDirection.Left)
        {
            scrollSnap.GetComponent<ScrollSnap>().PreviousScreen();
            //Debug.Log(axisEventData.moveDir);
        }
    }
}