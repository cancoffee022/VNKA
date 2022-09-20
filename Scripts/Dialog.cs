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
public class Dialog : ScriptableObject //��ȭ����� ��Ȳ�� ���޹޾ƾ��Ѵ�
{
    public List<DialogText> dialogText;

    public bool isQuestAccept=false;
    public bool isCompleteQuest=false;

    public int NPCdialogProgress(int dialogProcess)
    {
        switch (dialogProcess)
        {
            case 0://����Ʈ ����
                if (isQuestAccept)//����
                    return 1;
                else//����
                    return 2;
            case 1://������
                return 4;
            case 2://������
                return 3;
            case 3://������ ����Ʈ ����
                if (isQuestAccept)
                    return 1;
                else
                    return 2;
            case 4://����Ʈ ����
                if (isCompleteQuest)
                {
                    Debug.Log("isCompleteQuest");
                    return 5;
                }
                else
                    return 4;
            case 5://����Ʈ �Ϸ� ����
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
