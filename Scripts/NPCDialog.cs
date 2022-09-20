using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPCDialog : MonoBehaviour
{
    [SerializeField]
    private Dialog dialog;
    [SerializeField]
    private int dialogProcess;
    [SerializeField]
    private int questProcess;

    public int QuestProcess 
    {
        get => questProcess;
        set
        {
            questProcess = value;
        }
    }

    public void Start()
    {
        dialog.isCompleteQuest = false;
        dialog.isQuestAccept = false;
    }

    public DialogText GiveDialog()
    {
        return dialog.dialogText[dialogProcess];
    }
  
    public void ChangeDialogProcess()
    {
        dialogProcess=dialog.NPCdialogProgress(dialogProcess);
    }
    public string[] DialogText()
    {
        return dialog.dialogText[dialogProcess].text;
    }
    public void ChangeQuestAcceptTrue()
    {
        dialog.isQuestAccept= true;
    }
    public void ChangeQuestAcceptFalse()
    {
        dialog.isQuestAccept = false;
    }
    public void ChangeCompleteQuestTrue()
    {
        dialog.isCompleteQuest = true;
    }
    public void ChangeCompleteQuestFalse()
    {
        dialog.isCompleteQuest = false;
    }
    public void StartQuest()
    {
        //gameObject.GetComponent<QuestGiver>().StartQuest(QuestProcess);
    }
    public void CompleteQuest()
    {
        //gameObject.GetComponent<QuestGiver>().CompleteQuest(QuestProcess);
    }
    /*public bool IscompleteQuests()
    {
        //Debug.Log(gameObject.GetComponent<QuestGiver>().isCompletableQuest());
        //return gameObject.GetComponent<QuestGiver>().isCompletableQuest();
    }*/
}
