using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestTabButton : MonoBehaviour, ISelectHandler,IMoveHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        QuestUI quest = GameManager.instance.quest;
        int x = gameObject.transform.GetSiblingIndex();

        switch (x)
        {
            case 0:
                quest.isActiveQuest = true;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateActiveMainQuestButton();
                quest.QuestDescriptionArea(0);
                break;
            case 1:
                quest.isActiveQuest = true;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateActiveSubQuestButton();
                quest.QuestDescriptionArea(0);
                break;
            case 2:
                quest.isActiveQuest = false;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateCompleteQuestButton();
                quest.QuestDescriptionArea(0);
                break;
        }
        quest.ChangeTabImage();
    }
    public void OnClick()
    {
        QuestUI quest = GameManager.instance.quest;
        int x = gameObject.transform.GetSiblingIndex();

        switch (x)
        {
            case 0:
                quest.isActiveQuest = true;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateActiveMainQuestButton();
                break;
            case 1:
                quest.isActiveQuest = true;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateActiveSubQuestButton();
                break;
            case 2:
                quest.isActiveQuest = false;
                quest.selectedTab = x;
                quest.ClearSeletedTabQuest();
                quest.CreateCompleteQuestButton();
                break;
        }

        if (quest.selectedTabQuests.Count != 0)
        {
            Debug.Log("selectedTabOnclick");
            quest.State = QuestUiState.SelectedQuest;
            EventSystem.current.SetSelectedGameObject(quest.selectedTabQuests[0]);
        }
        else
        {
            quest.State = QuestUiState.SelectedTab;
        }
        GameManager.instance.State = UiState.Quest;

        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
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