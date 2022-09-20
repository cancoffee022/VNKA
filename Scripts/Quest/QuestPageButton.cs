using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class QuestPageButton : MonoBehaviour, ISelectHandler, IMoveHandler, IDeselectHandler
{
    QuestUI questUi;
    float posX;
    GameObject arrowIcon;
    void Awake()
    {
        questUi = GameManager.instance.quest;
        posX = gameObject.GetComponent<RectTransform>().anchoredPosition.x;
        arrowIcon = gameObject.transform.GetChild(2).gameObject;
    }
    public void OnClick()
    {
        Debug.Log("questPageButtonClick");
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        GameManager.instance.State = UiState.Quest;
    }
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("questPageButtonSelect");
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), posX +20, 0.15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
        int x = 0;
        for (int i = 0; i < questUi.selectedTabQuests.Count; i++)
        {
            if (questUi.selectedTabQuests[i] == gameObject)
            {
                x = i;
            }
        }
        questUi.trackingQuestCo = x;
        questUi.State = QuestUiState.SelectedQuest;
        questUi.ChangeTabImage();
        questUi.QuestDescriptionArea(x);

        arrowIcon.SetActive(true);
        LeanTween.moveX(arrowIcon.GetComponent<RectTransform>(), -283, 0.7f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1).setIgnoreTimeScale(true);
    }

    public void OnMove(AxisEventData eventData)
    {
        int x = gameObject.transform.GetSiblingIndex();
        if (eventData.moveDir == MoveDirection.Up && x > 0)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
            //questUi.questUiSnap.GetComponent<ScrollSnap>().ChangePage(x - 1);
            questUi.questUiSnap.GetComponent<ScrollSnap>().PreviousScreen();
            Debug.Log("위로이동");
        }
        else if (eventData.moveDir == MoveDirection.Down && questUi.selectedTabQuests[0] != gameObject)
        {
            Debug.Log("아래로이동");
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
            //questUi.questUiSnap.GetComponent<ScrollSnap>().ChangePage(x + 1);
            questUi.questUiSnap.GetComponent<ScrollSnap>().NextScreen();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), posX, 0.15f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
        arrowIcon.SetActive(false);
        LeanTween.cancel(arrowIcon);
        arrowIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-290f, 0);
    }
}