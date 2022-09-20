using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

public enum QuestUiState
{
    OffQuestUi,
    SelectedTab,
    SelectedQuest
}
public class QuestUI : MonoBehaviour
{
    public GameObject questList;
    public GameObject questPage;
    public GameObject questMask;
    public GameObject questUiSnap;
    public GameObject questTaskTargetText;
    public GameObject questDescriptionArea;
    public GameObject noQuestPanel;
    public GameObject scrollBar;
    public Inventory inven;
    public List<GameObject> questTabs;

    public GameObject trackingPage;
    public Quest trackingQuest;
    public int trackingQuestCo;

    [SerializeField]
    private QuestUiState state;
    public QuestUiState State
    {
        get => state;
        set
        {
            QuestUiState prevState = state;
            state = value;
            if (value != prevState)
            {
                UpdateState(prevState);
            }
        }
    }

    public void EscapeQuest()
    {
        if ((int)state > 0)
        {
            State--;
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
        }

    }

    public bool isQuestUiOn => State != QuestUiState.OffQuestUi;

    public List<Quest> activeQuests;//받은 퀘스트
    public List<Quest> completeQuests; //완료한 퀘스트

    public List<int> lists = new List<int>();

    public List<GameObject> selectedTabQuests;

    public Text questDisplayName;
    public Text questDescritpionText;
    public Text QuestRewardText;

    public Sprite selectedImage;
    public Sprite deselectedImage;

    public GameObject QuestTextTarget;

    public List<GameObject> questTasks = new List<GameObject>();

    public bool isActiveQuest;
    public int selectedTab;

    public void UpdateUseItems(Item _item)
    {
        Debug.Log("아이템 사용 여부 검사 실시");
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].goalType == GoalType.Use)
            {
                activeQuests[i].valueCount = 0;
                activeQuests[i].IsReached = false;

                for (int a = 0; a < activeQuests[i].targetItems.Count; a++)
                {
                    Debug.Log("아이템 사용여부 검사 실시2");
                    if (activeQuests[i].targetItems[a].itemID == _item.itemID)
                    {
                        activeQuests[i].currentAmount[a]++;
                    }

                    if (activeQuests[i].currentAmount[a] >= activeQuests[i].targetValues[a])
                    {
                        activeQuests[i].valueCount++;
                        if (activeQuests[i].valueCount >= activeQuests[i].targetItems.Count)
                        {
                            activeQuests[i].IsReached = true;
                        }
                    }

                }
            }
        }
    }

    public void UpdateSubWpnWearing()
    {
        Debug.Log("장착 여부 검사 실시");
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].goalType == GoalType.Wear)
            {
                activeQuests[i].valueCount = 0;
                activeQuests[i].IsReached = false;
                

                for (int a = 0; a < activeQuests[i].targetItems.Count; a++)
                {
                    Debug.Log("장착 여부 검사 실시2");
                    activeQuests[i].currentAmount[a] = 0;

                    for (int b=0; b< GameManager.instance.playerStatus.secondWpnList.Count; b++)
                    {
                        if (activeQuests[i].targetItems[a].itemID == GameManager.instance.playerStatus.secondWpnList[b].itemID)
                        {
                            activeQuests[i].currentAmount[a]++;
                        }
                    }

                    if (activeQuests[i].currentAmount[a] >= activeQuests[i].targetValues[a])
                    {
                        activeQuests[i].valueCount++;
                        if (activeQuests[i].valueCount >= activeQuests[i].targetItems.Count)
                        {
                            activeQuests[i].IsReached = true;
                        }
                    }

                }
            }
        }
    }




    public void UpdateKillMons()
    {
        Debug.Log("몬스터 검사 실시");
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].goalType == GoalType.Kill)
            {
                activeQuests[i].valueCount = 0;
                activeQuests[i].IsReached = false;

                for (int a = 0; a < activeQuests[i].targetMonNames.Count; a++)
                {
                    Debug.Log("몬스터 검사 실시2");

                    for (int b = 0; b < GameManager.instance.BC.killedMonNames.Count; b++)
                    {
                        if (GameManager.instance.BC.killedMonNames[b] == activeQuests[i].targetMonNames[a])
                        {
                            activeQuests[i].currentAmount[a]++;
                            GameManager.instance.BC.killedMonNames[b] = "None";
                        }
                    }


                    if (activeQuests[i].currentAmount[a] >= activeQuests[i].targetValues[a])
                    {
                        activeQuests[i].valueCount++;
                        if (activeQuests[i].valueCount >= activeQuests[i].targetMonNames.Count)
                        {
                            activeQuests[i].IsReached = true;
                        }
                    }

                }
            }
        }
    }

    public void UpdateState(QuestUiState prevState)
    {
        switch (State)
        {
            case QuestUiState.OffQuestUi:
                selectedTab = 0;
                foreach (var tab in questTabs)
                {
                    Debug.Log("삭제");
                    tab.GetComponent<Image>().sprite = deselectedImage;
                }
                if (selectedTabQuests.Count != 0)
                {
                    questUiSnap.GetComponent<ScrollSnap>().ChangePage(0);
                    //버튼 leantween이랑 글씨 색 초기화 여기서 넣기
                }
                break;
            case QuestUiState.SelectedTab:
                if ((int)prevState > 1)
                {
                    EventSystem.current.SetSelectedGameObject(questTabs[selectedTab]);
                    questUiSnap.GetComponent<ScrollSnap>().ChangePage(0);
                }
                break;
            case QuestUiState.SelectedQuest:
                break;
        }
    }

    public void UpdateGatherItems()
    {
        Debug.Log("아이템 검사 실시");
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].goalType == GoalType.Gathering)
            {
                activeQuests[i].valueCount = 0;
                activeQuests[i].IsReached = false;

                for (int a = 0; a < activeQuests[i].targetItems.Count; a++)
                {
                    Debug.Log("아이템 검사 실시2");
                    int amount = inven.SearchItemsAmount(activeQuests[i].targetItems[a].itemName);

                    activeQuests[i].currentAmount[a] = amount;


                    if (activeQuests[i].currentAmount[a] >= activeQuests[i].targetValues[a])
                    {
                        activeQuests[i].valueCount++;
                        if (activeQuests[i].valueCount >= activeQuests[i].targetItems.Count)
                        {
                            activeQuests[i].IsReached = true;
                        }
                    }

                }
            }
        }
    }
    //추적중인 퀘스트 내용 업데이트
    public void UpdateTrackingQuest()
    {
        RectTransform questRect = GameManager.instance.TrackingQuestUI.transform.GetChild(0).GetComponent<RectTransform>();
        Text questName = GameManager.instance.TrackingQuestUI.transform.GetChild(1).GetComponent<Text>();
        Text questObjectives = GameManager.instance.TrackingQuestUI.transform.GetChild(2).GetComponent<Text>();
        if (trackingQuest != null)
        {

            if (trackingQuest.isActive == false && trackingQuest.IsReached == true)
            {
                questName.text = "";
                questObjectives.text = "추적중인 퀘스트가 없습니다";
                trackingQuest = null;
            }
            else
            {
                questName.text = trackingQuest.questName;
                questObjectives.text = "";
                if ((trackingQuest.goalType == GoalType.Gathering) || (trackingQuest.goalType == GoalType.Use) || (trackingQuest.goalType == GoalType.Wear))
                {
                    for (int i = 0; i < trackingQuest.targetItems.Count; i++)
                    {
                        Item targetItem = trackingQuest.targetItems[i];
                        int targetValue = trackingQuest.targetValues[i];
                        int currentAmount = trackingQuest.currentAmount[i];

                        questObjectives.text += targetItem.itemName + " (" + currentAmount + "/" + targetValue + ") ";
                        if (i + 1 < trackingQuest.targetItems.Count)
                        {
                            questObjectives.text += "\n";
                        }
                    }
                }
                else if (trackingQuest.goalType == GoalType.Kill)
                {
                    for (int i = 0; i < trackingQuest.targetMonNames.Count; i++)
                    {
                        string targetMonName = trackingQuest.targetMonNames[i];
                        int targetValue = trackingQuest.targetValues[i];
                        int currentAmount = trackingQuest.currentAmount[i];

                        questObjectives.text += targetMonName + " (" + currentAmount + "/" + targetValue + ") ";
                        if (i + 1 < trackingQuest.targetMonNames.Count)
                        {
                            questObjectives.text += "\n";
                        }
                    }
                }   
                questRect.sizeDelta = new Vector2(562f, 96 + 20*(trackingQuest.targetValues.Count));

            }
        }
    }

    //퀘스트 버튼 누르면 추적중으로 옮기기
    public void TrackQuest()
    {
        RectTransform questRect = GameManager.instance.TrackingQuestUI.transform.GetChild(0).GetComponent<RectTransform>();
        Text questName = GameManager.instance.TrackingQuestUI.transform.GetChild(1).GetComponent<Text>();
        Text questObjectives = GameManager.instance.TrackingQuestUI.transform.GetChild(2).GetComponent<Text>();

        Quest targetQuest = activeQuests[lists[trackingQuestCo]];
        trackingQuest = targetQuest;

        if (selectedTabQuests[trackingQuestCo].transform.GetChild(1).gameObject.activeSelf == true)
        {
            selectedTabQuests[trackingQuestCo].transform.GetChild(1).gameObject.SetActive(false);
            trackingPage = null;
            trackingQuest = null;
            questRect.sizeDelta = new Vector2(562f, 96 + 20);
            //LeanTween.size(questRect, new Vector2(562f, 96 +20), 0f).setIgnoreTimeScale(true);
        }   
        else
        {
            if (trackingPage != null)
                trackingPage.transform.GetChild(1).gameObject.SetActive(false);

            trackingPage = selectedTabQuests[trackingQuestCo];
            trackingPage.transform.GetChild(1).gameObject.SetActive(true);

            int targetCo = targetQuest.targetValues.Count;
            questName.text = targetQuest.questName;
            questObjectives.text = "";

            if ((targetQuest.goalType == GoalType.Gathering) || (targetQuest.goalType == GoalType.Use)||(targetQuest.goalType==GoalType.Wear))
            {
                for (int i = 0; i < targetQuest.targetItems.Count; i++)
                {
                    Item targetItem = targetQuest.targetItems[i];
                    int targetValue = targetQuest.targetValues[i];
                    int currentAmount = targetQuest.currentAmount[i];

                    questObjectives.text += targetItem.itemName + " (" + currentAmount + "/" + targetValue + ") ";
                    if (i + 1 != targetQuest.targetItems.Count)
                    {
                        questObjectives.text += "\n";
                    }
                }
            }
            else if (targetQuest.goalType == GoalType.Kill)
            {
                for (int i = 0; i < targetQuest.targetMonNames.Count; i++)
                {
                    string targetMonName = targetQuest.targetMonNames[i];
                    int targetValue = targetQuest.targetValues[i];
                    int currentAmount = targetQuest.currentAmount[i];

                    questObjectives.text += targetMonName + " (" + currentAmount + "/" + targetValue + ") ";
                    if (i + 1 != targetQuest.targetMonNames.Count)
                    {
                        questObjectives.text += "\n";
                    }
                }
            }
            questRect.sizeDelta = new Vector2(562f, 94 + (20 * targetCo));
            //LeanTween.size(questRect, new Vector2(562f, 94 + (20 * targetCo)), 0f).setIgnoreTimeScale(true);
        }

    }

    public void CreateActiveMainQuestButton()
    {
        ClearSeletedTabQuest();
        lists.Clear();
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questID < 2000)
            {
                Debug.Log("MainQuestCreate");
                selectedTabQuests.Add(Instantiate(questPage, questList.transform));
                lists.Add(i);
                selectedTabQuests[i].transform.GetChild(0).GetComponent<Text>().text = activeQuests[i].questName;
                selectedTabQuests[i].GetComponent<Button>().onClick.RemoveAllListeners();
                selectedTabQuests[i].GetComponent<Button>().onClick.AddListener(TrackQuest);

                if(activeQuests[i] == trackingQuest)
                {
                    trackingPage = selectedTabQuests[i];
                    trackingPage.transform.GetChild(1).gameObject.SetActive(true);
                }

            }
        }

        if (selectedTabQuests.Count == 0)
        {
            noQuestPanel.SetActive(true);
            scrollBar.SetActive(false);
            questUiSnap.SetActive(false);
            questDescriptionArea.SetActive(false);
        }
        else
        {
            noQuestPanel.SetActive(false);
            scrollBar.SetActive(true);
            questUiSnap.SetActive(true);
            questDescriptionArea.SetActive(true);
        }

        for (int i = 0; i < selectedTabQuests.Count; i++)
        {
            Navigation buttonNav = selectedTabQuests[i].GetComponent<Button>().navigation;
            if (i > 0)
            {
                buttonNav.selectOnUp = selectedTabQuests[i - 1].GetComponent<Button>();
            }
            if (selectedTabQuests.Count - 1 > i)
            {
                buttonNav.selectOnDown = selectedTabQuests[i + 1].GetComponent<Button>();
            }
            selectedTabQuests[i].GetComponent<Button>().navigation = buttonNav;
        }
    }
    public void CreateActiveSubQuestButton()
    {
        ClearSeletedTabQuest();
        lists.Clear();
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questID > 2000)
            {
                Debug.Log("subQuestCreate");
                selectedTabQuests.Add(Instantiate(questPage, questList.transform));
                lists.Add(i);
                selectedTabQuests[i].transform.GetChild(0).GetComponent<Text>().text = activeQuests[i].questName;
            }
        }

        if (selectedTabQuests.Count == 0)
        {
            noQuestPanel.SetActive(true);
            scrollBar.SetActive(false);
            questUiSnap.SetActive(false);
            questDescriptionArea.SetActive(false);
        }
        else
        {
            scrollBar.SetActive(true);
            noQuestPanel.SetActive(false);
            questUiSnap.SetActive(true);
            questDescriptionArea.SetActive(true);
        }

        for (int i = 0; i < selectedTabQuests.Count; i++)
        {
            Navigation buttonNav = selectedTabQuests[i].GetComponent<Button>().navigation;
            if (i > 0)
            {
                buttonNav.selectOnUp = selectedTabQuests[i - 1].GetComponent<Button>();
            }
            if (selectedTabQuests.Count - 1 > i)
            {
                buttonNav.selectOnDown = selectedTabQuests[i + 1].GetComponent<Button>();
            }
            selectedTabQuests[i].GetComponent<Button>().navigation = buttonNav;
        }
    }
    public void CreateCompleteQuestButton()
    {
        ClearSeletedTabQuest();
        lists.Clear();
        for (int i = 0; i < completeQuests.Count; i++)
        {
            Debug.Log("completeQuestCreate");
            selectedTabQuests.Add(Instantiate(questPage, questList.transform));
            lists.Add(i);
            selectedTabQuests[i].transform.GetChild(0).GetComponent<Text>().text = completeQuests[i].questName;
        }

        if (selectedTabQuests.Count == 0)
        {
            noQuestPanel.SetActive(true);
            scrollBar.SetActive(false);
            questUiSnap.SetActive(false);
            questDescriptionArea.SetActive(false);
        }
        else
        {
            noQuestPanel.SetActive(false);
            scrollBar.SetActive(true);
            questUiSnap.SetActive(true);
            questDescriptionArea.SetActive(true);
        }

        for (int i = 0; i < selectedTabQuests.Count; i++)
        {
            Navigation buttonNav = selectedTabQuests[i].GetComponent<Button>().navigation;
            if (i > 0)
            {
                buttonNav.selectOnUp = selectedTabQuests[i - 1].GetComponent<Button>();
            }
            if (selectedTabQuests.Count - 1 > i)
            {
                buttonNav.selectOnDown = selectedTabQuests[i + 1].GetComponent<Button>();
            }
            selectedTabQuests[i].GetComponent<Button>().navigation = buttonNav;
        }
    }

    public void ClearSeletedTabQuest()
    {
        foreach (var gameObject in selectedTabQuests)
        {
            Destroy(gameObject);
        }
        selectedTabQuests.Clear();
    }

    public void QuestDescriptionArea(int x)
    {
        if (selectedTabQuests.Count > 0)
        {
            foreach (var quest in questTasks)
            {
                Destroy(quest);
            }
            questTasks.Clear();
            if (isActiveQuest)
            {
                questDisplayName.text = activeQuests[lists[x]].questName;
                questDescritpionText.text = activeQuests[lists[x]].description;

                if (activeQuests[lists[x]].goalType == GoalType.Gathering||(activeQuests[lists[x]].goalType == GoalType.Wear))
                {
                    for (int i = 0; i < activeQuests[lists[x]].targetItems.Count; i++)
                    {

                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = activeQuests[lists[x]].targetItems[i].itemName + "\n" +
                            activeQuests[lists[x]].currentAmount[i].ToString() + "/" + activeQuests[lists[x]].targetValues[i].ToString();

                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = activeQuests[lists[x]].targetItems[i].itemIcon;

                    }
                }
                if (activeQuests[lists[x]].goalType == GoalType.Kill)
                {
                    for (int i = 0; i < activeQuests[lists[x]].targetMonNames.Count; i++)
                    {
                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = activeQuests[lists[x]].targetMonNames[i] + "\n" +
                            activeQuests[lists[x]].currentAmount[i].ToString() + "/" + activeQuests[lists[x]].targetValues[i].ToString();

                        //questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = activeQuests[lists[x]].targetItems[i].itemIcon;
                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = deselectedImage;
                    }
                    
                }
                if (activeQuests[lists[x]].goalType == GoalType.Use)
                {
                    for (int i = 0; i < activeQuests[lists[x]].targetItems.Count; i++)
                    {
                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = activeQuests[lists[x]].targetItems[i].itemName + "\n" +
                            activeQuests[lists[x]].currentAmount[i].ToString() + "/" + activeQuests[lists[x]].targetValues[i].ToString();

                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = activeQuests[lists[x]].targetItems[i].itemIcon;
                    }
                }
                QuestRewardText.text = "보상\n";
                for (int i = 0; i < activeQuests[lists[x]].itemReward.Count; i++)
                {
                    if (i == activeQuests[lists[x]].itemReward.Count - 1)
                    {
                        QuestRewardText.text += activeQuests[lists[x]].itemReward[i].itemName + " X" + activeQuests[lists[x]].itemRewardAmount[i];
                        break;
                    }
                    QuestRewardText.text += activeQuests[lists[x]].itemReward[i].itemName +" X"+activeQuests[lists[x]].itemRewardAmount[i] + ",";
                }
                QuestRewardText.text += "\nGold +" + activeQuests[lists[x]].goldReward + "\nEXP +" + activeQuests[lists[x]].expReward;
            }
            else
            {
                questDisplayName.text = completeQuests[lists[x]].questName;
                questDescritpionText.text = completeQuests[lists[x]].description;
                if (completeQuests[lists[x]].goalType == GoalType.Gathering || (completeQuests[lists[x]].goalType == GoalType.Wear))
                {
                    for (int i = 0; i < completeQuests[lists[x]].targetItems.Count; i++)
                    {

                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = completeQuests[lists[x]].targetItems[i].itemName + "\n" +
                            completeQuests[lists[x]].currentAmount[i].ToString() + "/" + completeQuests[lists[x]].targetValues[i].ToString();

                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = completeQuests[lists[x]].targetItems[i].itemIcon;
                    }
                }
                if (completeQuests[lists[x]].goalType == GoalType.Kill)
                {
                    for (int i = 0; i < completeQuests[lists[x]].targetMonNames.Count; i++)
                    {
                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = completeQuests[lists[x]].targetMonNames[i] + "\n" +
                            completeQuests[lists[x]].currentAmount[i].ToString() + "/" + completeQuests[lists[x]].targetValues[i].ToString();

                        //questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = completeQuests[lists[x]].targetItems[i].itemIcon;
                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = deselectedImage;
                    }
                }
                if (completeQuests[lists[x]].goalType == GoalType.Use)
                {
                    for (int i = 0; i < completeQuests[lists[x]].targetItems.Count; i++)
                    {
                        questTasks.Add(Instantiate(questTaskTargetText, QuestTextTarget.transform));

                        questTasks[i].GetComponent<Text>().text = completeQuests[lists[x]].targetItems[i].itemName + "\n" +
                            completeQuests[lists[x]].currentAmount[i].ToString() + "/" + completeQuests[lists[x]].targetValues[i].ToString();

                        questTasks[i].transform.GetChild(0).GetComponent<Image>().sprite = completeQuests[lists[x]].targetItems[i].itemIcon;
                    }
                }

                QuestRewardText.text = "보상\n";
                for (int i = 0; i < completeQuests[lists[x]].itemReward.Count; i++)
                {
                    if (i == completeQuests[lists[x]].itemReward.Count - 1)
                    {
                        QuestRewardText.text += completeQuests[lists[x]].itemReward[i].itemName + " X" + completeQuests[lists[x]].itemRewardAmount[i];
                        break;
                    }
                    QuestRewardText.text += completeQuests[lists[x]].itemReward[i].itemName + " X" + completeQuests[lists[x]].itemRewardAmount[i] + ",";
                }
                QuestRewardText.text += "\nGold +" + completeQuests[lists[x]].goldReward + "\nEXP +" + completeQuests[lists[x]].expReward;
            }
        }
    }

    public void ChangeTabImage()
    {
        for (int i = 0; i < questTabs.Count; i++)
        {
            if (i == selectedTab)
            {
                questTabs[i].GetComponent<Image>().sprite = selectedImage;
                continue;
            }
            questTabs[i].GetComponent<Image>().sprite = deselectedImage;
        }
    }

}