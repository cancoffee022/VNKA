using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{
    private string[] dialogText;
    public Text talkText;
    public NPCDialog npcDialog;

    private bool pressEnter;
    private bool onAction = false;

    public GameObject dialogPanel;
    public GameObject Player;
    public GameObject selectionButton;
    public GameObject AcceptButton;
    public GameObject DenyButton;

    [SerializeField]
    private State state = State.NotInitialized;
    enum State
    {
        NotInitialized,
        Playing,
        PlayingSkipping
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)&&state!=State.PlayingSkipping)
        {
            Skip();
        }
    }
    public void Action()
    {
        dialogText = npcDialog.DialogText();
        dialogPanel.SetActive(true);
        StartCoroutine(Run());
        onAction = true;
        Player.GetComponent<TPSCharacterController>().stopMove = true;
    }
    public void QuestAction()
    {
        dialogText = npcDialog.DialogText();
        dialogPanel.SetActive(true);
        StartCoroutine(ChoiceRun());
        onAction = true;
        Player.GetComponent<TPSCharacterController>().stopMove = true;
    }

    IEnumerator Run()
    {
        for (int i = 0; i <= dialogText.Length; i++)
        {
            if (i == dialogText.Length)
            {
                StopAction();
                break;
            }

            yield return PlayLine(dialogText[i]);
        }
    }IEnumerator ChoiceRun()
    {
        for (int i = 0; i <= dialogText.Length; i++)
        {
            if (i == dialogText.Length)
            {
                selectionButton.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(AcceptButton);
                break;
            }

            yield return PlayLine(dialogText[i]);
        }
    }
    IEnumerator PlayLine(string text)
    {
        state = State.Playing;
        for (int i = 0; i < text.Length + 1; i++)
        {
            if (state == State.PlayingSkipping)
            {
                talkText.text = text;
                state = State.Playing;
                break;
            }
            yield return new WaitForSeconds(0.3f);
            talkText.text = text.Substring(0, i);
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 25; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (state == State.PlayingSkipping)
            {
                state = State.Playing;
                break;
            }
        }
    }
    public void AcceptQuest()
    {
        npcDialog.StartQuest();
        npcDialog.ChangeQuestAcceptTrue();
        npcDialog.ChangeDialogProcess();
        dialogText = npcDialog.DialogText();
        StartCoroutine(Run());
        selectionButton.SetActive(false);
    }
    public void DenyQuest()
    {
        npcDialog.ChangeQuestAcceptFalse();
        npcDialog.ChangeDialogProcess();
        dialogText = npcDialog.DialogText();
        StartCoroutine(Run());
        selectionButton.SetActive(false);
    }

    public void Skip()
    {
        state = State.PlayingSkipping;
    }
    public void StopAction()
    {
        dialogPanel.SetActive(false);
        state = State.NotInitialized;
        onAction = false;
        dialogText = null;
        talkText.text = "";
        Player.GetComponent<TPSCharacterController>().stopMove = false;
        Player.GetComponentInChildren<CharacterInteraction>().EndInteraction();
        npcDialog.ChangeDialogProcess();
    }
}
