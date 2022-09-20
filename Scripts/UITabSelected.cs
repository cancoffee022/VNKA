using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITabSelected : MonoBehaviour, ISelectHandler,IMoveHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("onselect");
        GameManager gm = GameManager.instance;
        for (int i = 0; i < gm.uiTabs.Count; i++)
        {
            if (eventData.selectedObject == gm.uiTabs[i])
            {
                gm.UITabsSelected = i;
                gm.ChangeUiTabImage(i);
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("onclick");
        Debug.Log(gameObject.name);
        GameManager gm = GameManager.instance;

        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);

        for (int i = 0; i < gm.uiTabs.Count; i++)
        {
            if (gameObject == gm.uiTabs[i])
            {
                switch (i)
                {
                    case 0://inventory
                        gm.State = UiState.Inventory;
                        gm.inven.State = InventoryState.TabsOn;
                        EventSystem.current.SetSelectedGameObject(gm.inven.ItemTabs[0]);
                        break;
                    case 1://playerStatus
                        gm.State = UiState.PlayerStatus;
                        EventSystem.current.SetSelectedGameObject(gm.playerStatus.mainWpnBtn);
                        break;
                    case 2://weaponStatus
                        gm.State = UiState.Weapon;
                        gm.weaponAbility.State = AbilityUIState.SnapWeapons;
                        EventSystem.current.SetSelectedGameObject(gm.weaponAbility.weaponStatus[0]);
                        break;
                    case 3://Monster
                        gm.State = UiState.Monster;
                        gm.monMenu.State = MonsterMenuState.MonsterMenuOn;
                        EventSystem.current.SetSelectedGameObject(gm.monMenu.monsterDictionaryList.transform.GetChild(0).gameObject);
                        break;
                    case 4://Quest
                        gm.State = UiState.Quest;
                        gm.quest.State = QuestUiState.SelectedTab;
                        EventSystem.current.SetSelectedGameObject(gm.quest.questTabs[0]);
                        break;
                    case 5://Option
                        gm.State = UiState.Option;
                        gm.option.State = OptionUiState.OnUi;
                        EventSystem.current.SetSelectedGameObject(gm.option.resolutionLeftButton);
                        break;
                }
                gm.UITabsSelected = i;
                gm.ChangeUiTabImage(i);
            }
        }
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