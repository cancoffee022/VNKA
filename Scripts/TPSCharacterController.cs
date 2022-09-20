using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPSCharacterController : MonoBehaviour
{
    public GameManager GM;
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    public Transform realCam;
    private Vector3 dirNormalized;
    private Vector3 finalDir;
    private Vector3 cameraPos;

    public float minDistance;
    public float maxDistance;
    private float finalDistance;

    public float mouseSensitivity;

    public bool isTalk;
    public bool stopMove;
    public bool stopCam;
    public bool run;
    public bool jump;
    public float runSpeed=5f;
    public float walkSpeed=2f;
    public float finalSpeed;

    Animator animator;

    public GameObject npc;
    public GameObject shop;
    public GameObject item;
    public bool npcChat;

    // Start is called before the first frame update
    void Start()
    {
        characterBody = this.transform;
        cameraArm = GameObject.Find("CameraArm(Clone)").transform;
        realCam = cameraArm.transform.GetChild(0);


        animator = characterBody.GetComponent<Animator>();
        GM = GameObject.Find("UI").GetComponent<GameManager>();

        dirNormalized = realCam.localPosition.normalized;
        finalDistance = realCam.localPosition.magnitude;

        


        /*headDir = head.localPosition.normalized;
        headDistance = head.localPosition.magnitude*/;
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.E) && npc != null && !GM.gameObject.GetComponent<NoneNpcChat>().tutorial)
        {
            Debug.Log("EEEEEEEEEEE");
            if (npcChat == false)
            {
                Debug.Log("말을 걸어 버려따");
                npcChat = true;
                npc.GetComponent<TypeWriterEffect>().ChatStart();
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && shop!= null&&!shop.GetComponent<ShopSeller>().isArrived)
        {
            GameManager.instance.isShop = true;
            shop.GetComponent<ShopSeller>().isArrived = true;
            shop.GetComponent<ShopSeller>().OpenShopUI();
            GameManager.instance.DoF.active = true;
            GameManager.instance.CloseTrackingQuest();
            //GameManager.instance.UIBackGround.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("상점 입장");
        }
        if (Input.GetKeyDown(KeyCode.E) && item != null)
        {
            //GameManager.instance.npcSelectUI.SetActive(false);
            GameManager.instance.CloseNpcChatUI();
            ItemPickup itemPickup = item.transform.GetComponent<ItemPickup>();
            GameManager.instance.itemName.text = itemPickup.itemName;
            GameManager.instance.itemAmount.text = "X"+itemPickup.count.ToString();
            GameManager.instance.ShowItemGet();
            GameManager.instance.CloseItemGet();
            Inventory.instance.GetAnItem(itemPickup.itemID, itemPickup.count);
            Destroy(item);
            item = null;
            Debug.Log("아이템 획득");

            GameManager.instance.quest.UpdateGatherItems();
            GameManager.instance.quest.UpdateTrackingQuest();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAround();
        Move();
        CameraMove();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
    }
    private void Move()
    {
        if (!stopMove) {
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            bool isMove = moveInput.magnitude != 0;
            if (isMove)
            {
                finalSpeed = (run) ? runSpeed : walkSpeed;

                Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
                Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
                Vector3 MoveDir = (lookForward * moveInput.y + lookRight * moveInput.x).normalized;
                characterBody.transform.rotation = Quaternion.Slerp(characterBody.rotation, Quaternion.LookRotation(MoveDir), 10f * Time.deltaTime);
                    
                transform.position += MoveDir * Time.deltaTime*finalSpeed;
                //GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * finalSpeed * Time.deltaTime);

            }
            float percent = ((run) ? 1 : 0.5f) * moveInput.magnitude * Time.deltaTime;
            animator.SetFloat("MovingSpeed", percent / Time.deltaTime, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("MovingSpeed", 0);
        }
         //Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x,0,cameraArm.forward.z).normalized, Color.red);
    }

    private void LookAround()
    {
        if (!stopMove)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime);
            Vector3 camAngle = cameraArm.rotation.eulerAngles;
            float x = camAngle.x - mouseDelta.y;

            if (x < 180f)
            {
                x = Mathf.Clamp(x, -1f, 70f);
            }
            else
            {
                x = Mathf.Clamp(x, 335f, 361f);
            }

            cameraArm.rotation = Quaternion.Euler(x, (camAngle.y + mouseDelta.x), camAngle.z);
        }
    }
    private void CameraMove()
    {
        if (!stopCam)
        {
            cameraPos = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
            cameraArm.position = Vector3.MoveTowards(cameraArm.position, cameraPos, 10f * Time.deltaTime);

            //로컬 위치 벡터를 월드 위치 벡터로 바꾼다
            //cameraArm 벡터에 Main Camera의 벡터를 더한 finalDir를 월드 공간으로 변환 
            //cameraArm위치에서 realCam위치를 벡터화 시켜서 finalDir에 저장함
            finalDir = cameraArm.TransformPoint(dirNormalized * maxDistance);


            RaycastHit hit;

            if (Physics.Linecast(cameraArm.position, finalDir, out hit))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
                //Debug.Log(finalDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }
            realCam.localPosition = Vector3.Lerp(realCam.localPosition, dirNormalized * finalDistance, Time.deltaTime * 10f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "TutoArea_0" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[1])) 
        {
            GM.noneNpcChat.TutoAreaStart(0);
        }
        if (other.name == "TutoArea_1" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[1]))
        {
            GM.noneNpcChat.TutoAreaStart(1);
        }
        if (other.name == "TutoArea_2" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[1]))
        {
            GM.noneNpcChat.TutoAreaStart(2);
        }
        if (other.name == "TutoArea_3" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[1]))
        {
            GM.noneNpcChat.TutoAreaStart(3);
        }
        if (other.name == "TutoArea_4" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[1]))
        {
            GM.noneNpcChat.TutoAreaStart(4);
        }
        if (other.name == "TutoArea_5" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[5]))
        {
            GM.noneNpcChat.TutoAreaStart(5);
        }
        if (other.name == "TutoArea_6" && GM.quest.activeQuests.Contains(GM.questDatabase.quests[5]))
        {
            // GM.CurrentLocation = "SerenaBossArea";
            //GM.hitMonster = "Serena";
            //아래는 테스트용 위에 만들면 지우면 됨
            GM.CurrentLocation = "OrtusA_1";
            GM.hitMonster = "Scouter";
            GM.noneNpcChat.TutoAreaStart(6);
            GM.noneNpcChat.serenaBattleCnt = 7;
           
        }
        if (other.tag == "Enemy")
        {
            GameManager.instance.isBattle = true;

            if(GM.SC.play == true)
                GM.SC.StartStop();
            GM.SC.Effects[0].PlayOneShot(GM.SC.uiEffectSounds[9]);
            
            Debug.Log("아야!");
            //other.GetComponent<BoxCollider>().enabled = false;

            GM.hitMonster = other.transform.parent.gameObject.GetComponent<EnemyController>().enemyName;
            GM.CurrentLocation = other.transform.parent.gameObject.gameObject.GetComponent<EnemyController>().location;

            /*GM.isHoldingList = new List<string>() { "글라디우스", "페니텐시아", "데빌리언 사이드" };
            GM.isWearing = "글라디우스";*/
            GM.isHoldingList.Clear();
            GM.isHoldingList = new List<string>(GM.playerStatus.playerisHoldingList);
            GM.isWearing = GM.playerStatus.WearingWpn.itemName;
            GM.playerPos = this.transform.position;
            GM.playerRot = this.transform.rotation;
            GM.hitMonsterObj = other.transform.parent.gameObject;

            //GM.isInside = other.transform.parent.gameObject.GetComponent<EnemyController>().isInside;

            //Destroy(other.transform.parent.gameObject);
            StartCoroutine(GM.StartBattle());
            //StartCoroutine(GM.LoadBattleScene());
        }
        if (other.tag == "Item")
        {
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = "???";
            GameManager.instance.ShowNpcChatUI();
            //GameManager.instance.npcSelectUI.SetActive(true);
            item = other.gameObject;
        }
        if (other.tag == "NPC" && GameManager.instance.State == UiState.OffMenu && GM.gameObject.GetComponent<NoneNpcChat>().tutorial == false)
        {
            npc = other.gameObject;
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = npc.GetComponent<TypeWriterEffect>().npcName;

            if (other.gameObject.GetComponent<QuestGiver>().NPCquest.Count == 0)
            {
                GameManager.instance.npcSelectUI.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.instance.npcTalkIcon;
            }
            else
            {
                //아이콘 변경
                if (other.gameObject.GetComponent<QuestGiver>().NPCquest[0].questID < 2000)
                {
                    GameManager.instance.npcSelectUI.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.instance.npcMainQuestIcon;
                }
                else
                {
                    GameManager.instance.npcSelectUI.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.instance.npcSubQuestIcon;
                }
            }

            if (npcChat == false)
                GameManager.instance.ShowNpcChatUI();
            //GameManager.instance.npcSelectUI.SetActive(true);
        }
        if (other.tag == "Shop")
        {
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = "자판기";
            GameManager.instance.npcSelectUI.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.instance.npcShopIcon;
            GameManager.instance.ShowNpcChatUI();
            //GameManager.instance.npcSelectUI.SetActive(true);
            shop = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "NPC")
        {
            npc = null;
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = null;
            //GameManager.instance.npcSelectUI.SetActive(false);
            GameManager.instance.CloseNpcChatUI();
        }
        if (other.tag == "Shop")
        {
            shop = null;
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = null;
            //GameManager.instance.npcSelectUI.SetActive(false);
            GameManager.instance.CloseNpcChatUI();
        }
        if(other.tag == "Item")
        {
            item = null;
            GameManager.instance.npcSelectUI.transform.GetChild(0).GetComponent<Text>().text = null;
            //GameManager.instance.npcSelectUI.SetActive(false);
            GameManager.instance.CloseNpcChatUI();
        }
    }
}
