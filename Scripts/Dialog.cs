using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogState
{
    InActive,
    GiveQuest,
    CompleteQuest
}
[CreateAssetMenu(menuName ="Dialog/NPC",fileName ="_Dialog")]
public class Dialog : ScriptableObject //대화내용과 상황을 전달받아야한다
{
    public List<DialogText> dialogText;

    public bool isQuestAccept=false;
    public bool isCompleteQuest=false;

    public int NPCdialogProgress(int dialogProcess)
    {
        switch (dialogProcess)
        {
            case 0://퀘스트 전달
                if (isQuestAccept)//수락
                    return 1;
                else//거절
                    return 2;
            case 1://수락시
                return 4;
            case 2://거절시
                return 3;
            case 3://간단한 퀘스트 전달
                if (isQuestAccept)
                    return 1;
                else
                    return 2;
            case 4://퀘스트 독촉
                if (isCompleteQuest)
                {
                    Debug.Log("isCompleteQuest");
                    return 5;
                }
                else
                    return 4;
            case 5://퀘스트 완료 축하
                break;
        }
        return dialogProcess;
    }
}
[System.Serializable]
public class DialogText
{
    public string[] text;

    [SerializeField]
    public DialogState dialogStates;

    public DialogState State => dialogStates;

    public bool isInactive => State == DialogState.InActive;
    public bool isGiveQuest => State == DialogState.GiveQuest;
    public bool isCompleteQuest => State == DialogState.CompleteQuest;
}
