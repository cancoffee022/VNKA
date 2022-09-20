using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteraction: MonoBehaviour
{
    public GameObject InteractionKey;
    public Collider InteractiveCollider;

    public DialogManager dialogManager;

    public bool inputKey;
    public bool doOther;
    public bool canInteraction;

    private void Start()
    {
        InteractionKey = GameObject.Find("CharacterInteraction _FIXPLEASE");
    }

    private void Update()
    {
        if (InteractionKey)
        {
            if (InteractionKey.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E) && !doOther)
                {
                    inputKey = true;
                    StartCoroutine(PressInteractionKey());
                }
                InteractionTrigger();
            }
            else
                InteractionTrigger();
        }
        else 
        {
            InteractionKey = GameObject.Find("CharacterInteraction _FIXPLEASE");
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        InteractiveCollider = collider;
        /*if (collider.CompareTag("NPC"))
        {
            canInteraction = true;
            *//*
            if (inputKey)
            {
                doOther = true;
                CharacterDialog();
            }*//*
        }*/
        /*if (collider.CompareTag("Item"))
        {
            canInteraction = true;
            if (inputKey)
            {
                AddItem();
                EndInteraction();
            }
        }*/
/*        else if (collider.transform.CompareTag("Shop"))
        {
            canInteraction = true;
            if (inputKey)
            {
                doOther = true;
            }
        }*/
        /*else
        {
            canInteraction = false;
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        EndInteraction();
    }
    public void EndInteraction()
    {
        doOther = false;
        canInteraction = false;
    }
    
    public void InteractionTrigger()
    {
        if (!doOther)
        {
            if (canInteraction)
                InteractionKey.SetActive(true);
            else
                InteractionKey.SetActive(false);
        }
        else
            InteractionKey.SetActive(false);
    }
    IEnumerator PressInteractionKey()
    {
        yield return new WaitForFixedUpdate();
        yield return inputKey = false;
    }

    public void CharacterDialog()
    {
        //어떤 npc와 대화하는지 검사
        NPCDialog npcdialog = InteractiveCollider.transform.GetComponent<NPCDialog>();
        DialogText dialogText = npcdialog.GiveDialog();

        dialogManager.npcDialog = npcdialog;

        if (dialogText.isGiveQuest)
        {
            dialogManager.QuestAction();
        }
        else if (dialogText.isCompleteQuest)
        {
           /* if (npcdialog.IscompleteQuests())
            {
                Debug.Log("확인");
                npcdialog.ChangeCompleteQuestTrue();
                npcdialog.ChangeDialogProcess();
                npcdialog.ChangeCompleteQuestFalse();
                dialogManager.Action();
                npcdialog.CompleteQuest();
            }
            else
                dialogManager.Action();*/

        }
        else
            dialogManager.Action();
            
    }
   /* public void AddItem()
    {
        ItemPickup itemPickup = InteractiveCollider.transform.GetComponent<ItemPickup>();
        GameManager.instance.itemName.text = itemPickup.itemName;
        GameManager.instance.itemAmount.text = itemPickup.count.ToString();
        GameManager.instance.ShowItemGet();
        GameManager.instance.CloseItemGet();
        Inventory.instance.GetAnItem(itemPickup.itemID, itemPickup.count);
        Destroy(InteractiveCollider.gameObject);
    }*/

}
