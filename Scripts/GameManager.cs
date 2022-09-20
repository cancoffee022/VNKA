using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Linq;
using System;
using HorizonBasedAmbientOcclusion.Universal;

public enum UiState
{
    OffMenu,
    OnMenu,
    Inventory,
    PlayerStatus,
    Monster,
    Weapon,
    Quest,
    Option
}

[System.Serializable]
public class sea<T>
    {
    public sea(int[] _target) => target = _target;
    public int[] target;
}

[System.Serializable]
public class PlayData
{
    public string saveChapterName;
    public string saveWorldName;
    public string saveAreaName;
    public string saveTime;
    public string playTime;

    public float tempPlayTime;

    public string playerName;

    public Vector3 playerPos;
    public Quaternion playerRot;

    public int playerLV;
    public int playerGold;
    public int playerExp;
    public int playerCurrentExp;

    public int playerHP;
    public int playerCurrentHp;
    public int playerSP;
    public int playerCurrentSp;
    public int playerEva;

    public int playerAtk;
    public int playerDef;

    public int trackingQuestInt;

    public int[] twoHandAbilityStat = { 0, 0, 0 };
    public int[] oneHandAbilityStat = { 0, 0, 0 };
    public int[] scytheAbilityStat = { 0, 0, 0 };
    public int[] spearAbilityStat = { 0, 0, 0 };

    public int WearingWpn;

    public int[] inventoryItemIDList;
    public int[] secondWpnList;

    //public WeaponChara[] weaponCharas;
    public int[] weaponCharaIdx;
    public int[] weaponCharaLevel;

    public int[] activeQuestsID;
    public int[] completedQuestsID;

    //퀘스트 내용
    public bool[] isReachedAry;
    public int[] valueCountAry;
    public bool[] isActiveAry;


    public bool[] isReachedComAry;
    public int[] valueCountComAry;
    public bool[] isActiveComAry;

    public sea<int[]>[] currentAmountAry;
    public sea<int[]>[] currentAmountComAry;

    //아이템 갯수
    public int[] itemAmountAry;

    public bool[] monsterBoolDictionary;
    public bool[] areaEnable;
    public bool firstSecondaryWeapon;

    //현재 전투 필드 int
    public int currentBattleFieldCnt;
}

[System.Serializable]
public class TutorialText
{
    public bool tutorialOK;
    public Sprite[] tutoSprites;
    [TextArea(3, 10)] public string[] tutoText;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public SoundController SC;
    public PlayData playData;
    public float playTime;
    public TPSCharacterController characterController;
    public BattleController BC;
    public LobbyUI lobbyUIClass;
    public ItemDatabase itemDatabase;
    public QuestDataBase questDatabase;
    public TutorialText[] tutorialRef;
    public NoneNpcChat noneNpcChat;

    public Transform directionalLightTransform;
    public Light directionalLight;
    public float dayTime;

    public GameObject tempBattleField;

    public GameObject[] battleFieldPrefabs;
    public int currentBattleFieldCnt;
    public GameObject battleTransitionBG;

    bool isStart = true;

    //아이템보여주는 정보창
    public GameObject itemGetUI;
    public Text itemName;
    public Text itemAmount;

    //상단바   0, 203 ,406 609 812 1015
    public RectTransform TopBar;

    //마우스 사라지게 하기
    float timeLeft;
    float visibleCursorTimer = 4.0f;
    float cursorPosition;
    bool catchCursor = true;

    [Header("Script")]
    public Inventory inven;
    public QuestUI quest;
    public PlayerStatus playerStatus;
    public AbilityUI weaponAbility;
    public ShopManager shopManager;
    public OptionUI option;
    public MonsterMenu monMenu;

    public GameObject CameraArmPrefab;
    public GameObject playerPrefab;
    public GameObject player;

    public Vector3 playerPos = new Vector3(0, 0, 0);
    public Quaternion playerRot = Quaternion.Euler(0, 0, 0);

    public GameObject[] SaveSlotBtns = new GameObject[5];
    
    [Header("UI")]
    public GameObject mainMenu;
    public GameObject BattleLoadingMenu;
    public Image BattleProgressBarImage;
    public string currentSceneName;
    public GameObject PlayerStatusMenu;
    public GameObject testSnap;
    public GameObject portalLoadingScreen;
    public Image loadingProgressBarImage;
    public Text loadingProgressText;

    public Sprite npcSubQuestIcon;
    public Sprite npcMainQuestIcon;
    public Sprite npcTalkIcon;
    public Sprite npcShopIcon;

    public GameObject npcChatUI;
    public Text conversationName;
    public Text conversationText;
    public Image conversationFade;
    public GameObject npcSelectUI;
    public GameObject UIBackGround;
    public GameObject lobbyUI;
    public GameObject shopMenu;
    public GameObject saveFilesMenu;
    public GameObject AutoSavedSlotBtn;
    public GameObject optionsUI;
    public GameObject monsterDictionaryUI;

    public GameObject hitMonsterObj;

    public GameObject tutorialBot;

    public GameObject AreaInfoUI;
    public GameObject TrackingQuestUI;

    public GameObject lobbyCam;
    public GameObject fieldCamera;

    public Sprite uiTabSpriteSelected;
    public Sprite uiTabSpriteDeselected;

    public GameObject prevSelectedTab;//이전 선택된 탭 저장
    public GameObject lastselect;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public Volume GlobalVolume;
    public DepthOfField DoF;
    public HBAO hbao;
    public Bloom bloom;
    public Tonemapping toneMap;
    public ShadowsMidtonesHighlights sMH;
    public ChromaticAberration chromaAberra;
    public Vignette vignette;
    public Texture2D cursorTex;

    public string saveWorldName;

    public List<string> isHoldingList;//착용중인 메인,보조 무기들(4개까지)
    public string isWearing;//착용중인 메인무기
    public string CurrentLocation; //현재 지역
    public string hitMonster; //부딪힌 몬스터

    public bool isShop;
    public bool isBattle;
    public bool isTabs;
    public bool isLobby;
    public bool isPlayingGame;

    public List<GameObject> uiTabs;

    public int uiTabClicked;

    public GameObject ES;



    

    public Coroutine runningEndBattle;

    

    [SerializeField]
    private int uiTabsSelected;
    public int UITabsSelected
    {
        get => uiTabsSelected;
        set
        {
            if (isMenuOn)
            {
                int prevSelected = uiTabsSelected;
                uiTabsSelected = value;
                TabsUpdate(value);
                /*foreach (var sprite in uiTabs)
                {
                    if (uiTabs[value] == sprite)
                    {
                        sprite.GetComponent<Image>().sprite = uiTabSelected;
                    }
                    else
                        sprite.GetComponent<Image>().sprite = uiTabDeselected;
                }*/
            }
            else if (!isMenuOn)
            {
                uiTabsSelected = value;
            }
            MoveTabUnderbar();

        }
    }


    [SerializeField]
    private UiState state;
    public UiState State
    {
        get => state;
        set
        {
            UiState prevState = state;
            //MenuUpdate(value);
            state = value;
            if (prevState != value)
            {
                MenuUpdate(prevState);
            }
        }
    }   
    public bool isMenu => State != UiState.OffMenu;
    public bool isMenuOn => State == UiState.OnMenu;
    public bool isInventoryOn => State == UiState.Inventory;
    public bool isPlayerStatusOn => State == UiState.PlayerStatus;
    public bool isMonsterOn => State == UiState.Monster;
    public bool isWeaponOn => State == UiState.Weapon;
    public bool isQuestOn => State == UiState.Quest;
    public bool isOptionOn => State == UiState.Option;
    float UIWaitTime = 0;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //Load(0);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.ForceSoftware);
        GlobalVolume.profile.TryGet(out hbao);

        hbao.EnableHBAO(true);
        GlobalVolume.profile.TryGet(out sMH);
        GlobalVolume.profile.TryGet(out DoF);
        GlobalVolume.profile.TryGet(out chromaAberra);

        GlobalVolume.profile.TryGet(out toneMap);
        GlobalVolume.profile.TryGet(out bloom);
        GlobalVolume.profile.TryGet(out vignette);

        SC = GetComponent<SoundController>();
        noneNpcChat = GetComponent<NoneNpcChat>();
    }
    void Start()
    {
        Debug.Log(Application.persistentDataPath+ "/playData.json");
        //sMH.midtones.Override(new Vector4(1f,1f,1f,-0.3f));
        SC.PlayBgm(0);
    }


    void Update()
    {

        UIWaitTime -= Time.unscaledDeltaTime;

        if (!isBattle || isMenu)
        {
            if (UIWaitTime <= 0)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll > 0)
                { // forward
                    if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Selectable>().FindSelectableOnUp() != null)
                        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject);
                    UIWaitTime = 0.2f;
                }
                else if (scroll < 0f)
                { // backwards
                    if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Selectable>().FindSelectableOnDown() != null) 
                        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject);
                    UIWaitTime = 0.2f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            StartCoroutine(TimeChange());
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SC.Effects[0].PlayOneShot(SC.uiEffectSounds[0]);
        }


        if (isPlayingGame)
        {
            playTime += Time.unscaledDeltaTime;
            //dayTime += Time.deltaTime;
            //ChangeLight();
        }

        //마우스 사라지게 하는곳
        if (isMenu)
        {
            if (catchCursor)
            {
                catchCursor = false;
                cursorPosition = Input.GetAxis("Mouse X");
            }
            if (Input.GetAxis("Mouse X") == cursorPosition)
            {
                timeLeft -= Time.unscaledDeltaTime;
                if (timeLeft < 0)
                {
                    timeLeft = visibleCursorTimer;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    catchCursor = true;
                }
            }
            else
            {
                timeLeft = visibleCursorTimer;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

/*
            if (Input.anyKeyDown && !Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1) 
                && !Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse3) && !Input.GetKey(KeyCode.Mouse4))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }*/

        }

        if (isMenu || npcChatUI.activeSelf)
        {
            //CloseNpcChatUI();
            //npcSelectUI.SetActive(false);
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastselect);
            }
            else
            {
                lastselect = EventSystem.current.currentSelectedGameObject;
            }

            if (npcChatUI.activeSelf && Input.GetMouseButtonDown(0))
            {
                lastselect.GetComponent<Button>().onClick.Invoke();
            }
        }



        if (!isLobby && !noneNpcChat.tutorial)
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Save(0);
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                inven.GetAnItem(2001, 1);
                inven.GetAnItem(2001, 1);
                inven.GetAnItem(2001, 1);
                inven.GetAnItem(2002, 1);
                inven.GetAnItem(2002, 1);
                inven.GetAnItem(2002, 1);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !isBattle && !characterController.npcChat && !isShop)
            {
                MainMenuOnOffState();
            }
            if (Input.GetKeyDown(KeyCode.I) && !isBattle && !characterController.npcChat && !isShop&&!isMenu)
            {
                CloseTrackingQuest();
                SC.Effects[0].PlayOneShot(SC.uiEffectSounds[3]);
                State = UiState.Inventory;
                LeanTween.moveX(TopBar, 0, 0.1f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
            }
            if (Input.GetKeyDown(KeyCode.P) && !isBattle && !characterController.npcChat && !isShop && !isMenu)
            {
                CloseTrackingQuest();
                SC.Effects[0].PlayOneShot(SC.uiEffectSounds[3]);
                State = UiState.PlayerStatus;
                LeanTween.moveX(TopBar, 204, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
            }

            if (inven.IsInventoryActive)
            {
                if (!inven.isShowItemTabs)
                {
                    inven.prevSelectedTabGameObject = EventSystem.current.currentSelectedGameObject;
                    for (int i = 0; i < inven.ItemTabs.Length; i++)
                    {
                        if (EventSystem.current.currentSelectedGameObject == inven.ItemTabs[i])
                        {
                            inven.selectedTab = i;
                            inven.ShowItem();
                            break;
                        }
                    }
                }
                else if (inven.isShowItemTabs)
                {
                    if (inven.prevSelectedTabGameObject != EventSystem.current.currentSelectedGameObject)
                    {
                        inven.isShowItemTabs = false;
                    }
                }
            }
            if (inven.IsItemActive)
            {
                if (!inven.isShowItemDescrip && EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>() != null)
                {
                    inven.prevSelectedItemButton = EventSystem.current.currentSelectedGameObject;
                    EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>().ActivePanel();
                    inven.isShowItemDescrip = true;
                }
                else if (inven.isShowItemDescrip)
                {
                    if (inven.prevSelectedItemButton != EventSystem.current.currentSelectedGameObject)
                        inven.isShowItemDescrip = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q) && !isBattle && !characterController.npcChat && !isShop && !isMenu)
            {
                SC.Effects[0].PlayOneShot(SC.uiEffectSounds[3]);

                CloseTrackingQuest();
                State = UiState.Quest;
                quest.UpdateGatherItems();
                LeanTween.moveX(TopBar, 816, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
            }
            if (shopMenu.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (shopManager.shopConfirmUI.activeSelf)
                    {
                        characterController.shop.GetComponent<ShopSeller>().OnClickNo();

                    }
                    else if (shopManager.isTabs)
                    {
                        shopMenu.SetActive(false);
                        shopManager.isTabs = false;
                        characterController.stopMove = false;
                        //UIBackGround.SetActive(false);
                        DoF.active = false;
                        Time.timeScale = 1f;
                        EventSystem.current.SetSelectedGameObject(null);
                        isShop = false;
                        ShowTrackingQuest();
                        characterController.shop.GetComponent<ShopSeller>().CloseShop();
                    }
                    else if (shopManager.isShopping)
                    {
                        shopManager.isShopping = false;
                        shopManager.buyBtn.GetComponent<Button>().interactable = true;
                        shopManager.sellBtn.GetComponent<Button>().interactable = true;
                        shopManager.isTabs = true;
                        EventSystem.current.SetSelectedGameObject(shopManager.buyBtn);

                    }

                }

            }
        }

    }

    [ContextMenu("ToJson")]
    public void Save(int _saveIndex)
    {
        //위치
        playData.playerPos = player.GetComponent<Transform>().transform.position;
        playData.playerRot = player.GetComponent<Transform>().transform.rotation;

        //지역 정보 및 플탐 저장시간   저장
        string hour;
        string minute;
        string second;

        playData.tempPlayTime = GameManager.instance.playTime;

        hour = ((Mathf.RoundToInt(playData.tempPlayTime) / 60) / 60).ToString("00");
        minute = ((Mathf.RoundToInt(playData.tempPlayTime) / 60)%60).ToString("00");
        second = (Mathf.RoundToInt(playData.tempPlayTime) % 60).ToString("00");

        playerStatus.playTime = hour+":"+minute+":"+second;
        playerStatus.saveTime = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
        playData.saveChapterName = playerStatus.saveChapterName;
        playData.saveWorldName = saveWorldName;
        playData.saveAreaName = playerStatus.saveAreaName;
        playData.saveTime = playerStatus.saveTime;
        playData.playTime = playerStatus.playTime;

        //이름 레벨 골드 경험치 저장
        playData.playerName = playerStatus.playerName;
        playData.playerLV = playerStatus.playerLV;
        playData.playerGold = playerStatus.playerGold;
        playData.playerExp = playerStatus.playerExp;

        //체력이랑 SP 저장
        playData.playerHP = playerStatus.playerHP;
        playData.playerCurrentHp = playerStatus.playerCurrentHp;
        playData.playerSP = playerStatus.playerSP;
        playData.playerCurrentSp = playerStatus.playerCurrentSp;

        // 공격력 방어력 수비력 저장
        playData.playerEva = playerStatus.playerEva;
        playData.playerAtk = playerStatus.playerAtk;
        playData.playerDef = playerStatus.playerDef;

        //어빌리티 강화된 수치 저장
        playData.twoHandAbilityStat = playerStatus.twoHandAbilityStat;
        playData.oneHandAbilityStat = playerStatus.oneHandAbilityStat;
        playData.scytheAbilityStat = playerStatus.scytheAbilityStat;
        playData.spearAbilityStat = playerStatus.spearAbilityStat;

        playData.secondWpnList = new int[playerStatus.secondWpnList.Count];

        //장착 무기및 보조무기 3개 저장
        playData.WearingWpn = playerStatus.WearingWpn.itemID;
        for(int i =0; i < playerStatus.secondWpnList.Count; i++)
        {
            playData.secondWpnList[i] = playerStatus.secondWpnList[i].itemID;
        }

        //인벤토리 저장
        playData.inventoryItemIDList = new int[inven.inventoryItemList.Count];
        playData.itemAmountAry = new int[inven.inventoryItemList.Count];

        for (int i=0; i < inven.inventoryItemList.Count; i++)
        {
            playData.inventoryItemIDList[i] = inven.inventoryItemList[i].itemID;
            playData.itemAmountAry[i] = inven.inventoryItemList[i].itemAmount;
        }



        //어빌리티 강화 정도 저장
        playData.weaponCharaIdx = new int[weaponAbility.weaponCharas.Count];
        playData.weaponCharaLevel = new int[weaponAbility.weaponCharas.Count];

        for (int i = 0; i < weaponAbility.weaponCharas.Count; i++)
        {
            playData.weaponCharaIdx[i] = weaponAbility.weaponCharas[i].Charaidx;
            playData.weaponCharaLevel[i] = weaponAbility.weaponCharas[i].Level;
        }

        //진행중 퀘스트, 완료 퀘스트 저장
        if (quest.trackingQuest != null)
        {
            playData.trackingQuestInt = quest.trackingQuest.questID;
        }
        else
        {
            playData.trackingQuestInt = 999999999;
        }


        playData.isReachedAry = new bool[quest.activeQuests.Count];
        playData.valueCountAry = new int[quest.activeQuests.Count];
        playData.isActiveAry = new bool[quest.activeQuests.Count];
        playData.currentAmountAry = new sea<int[]>[quest.activeQuests.Count];
        playData.activeQuestsID = new int[quest.activeQuests.Count];

        playData.isReachedComAry = new bool[quest.completeQuests.Count];
        playData.valueCountComAry = new int[quest.completeQuests.Count];
        playData.isActiveComAry = new bool[quest.completeQuests.Count];
        playData.currentAmountComAry = new sea<int[]>[quest.completeQuests.Count];
        playData.completedQuestsID = new int[quest.completeQuests.Count];

        for (int i = 0; i < quest.activeQuests.Count; i++)
        {
            playData.isReachedAry[i] = quest.activeQuests[i].IsReached;
            playData.valueCountAry[i] = quest.activeQuests[i].valueCount;
            playData.isActiveAry[i] = quest.activeQuests[i].isActive;
            playData.activeQuestsID[i] = quest.activeQuests[i].questID;

            for (int a = 0; a < quest.activeQuests[i].currentAmount.Count; a++)
            {
                playData.currentAmountAry[i] = new sea<int[]>(quest.activeQuests[i].currentAmount.ToArray());
            }

        }
        for (int i = 0; i < quest.completeQuests.Count; i++)
        {
            playData.isReachedComAry[i] = quest.completeQuests[i].IsReached;
            playData.valueCountComAry[i] = quest.completeQuests[i].valueCount;
            playData.isActiveComAry[i] = quest.completeQuests[i].isActive;
            playData.completedQuestsID[i] = quest.completeQuests[i].questID;

            for (int a = 0; a < quest.completeQuests[i].currentAmount.Count; a++)
            {
                playData.currentAmountComAry[i] = new sea<int[]>(quest.completeQuests[i].currentAmount.ToArray());
            }
        }

        playData.areaEnable = new bool[noneNpcChat.cA.Length];
        for (int i = 0; i < noneNpcChat.cA.Length; i++) 
        {
            playData.areaEnable[i] = noneNpcChat.cA[i].areaEnable;
        }

        playData.firstSecondaryWeapon = playerStatus.firstSecondaryWeapon;
        //도감 정보 저장
        playData.monsterBoolDictionary = new bool[monMenu.monsterBoolDictionary.Length];
        playData.monsterBoolDictionary = monMenu.monsterBoolDictionary;

        //현재 전투 필드 cnt 저장
        playData.currentBattleFieldCnt = currentBattleFieldCnt;

        Debug.Log("저장완료");
        string jsonData = JsonUtility.ToJson(playData, true);
        string path = (Application.persistentDataPath + "/playData_" + _saveIndex + ".json");

        File.WriteAllText(path, jsonData);

    }

    [ContextMenu("FromJson")]
    public void Load(int _saveIndex)
    {
        string path = (Application.persistentDataPath + "/playData_"+_saveIndex+".json");
        string jsonData = File.ReadAllText(path);

        playData = JsonUtility.FromJson<PlayData>(jsonData);

        SC.Effects[0].PlayOneShot(SC.uiEffectSounds[4]);

        //인벤토리 퀘스트 내용 초기화실행
        for (int i = 0; i < questDatabase.quests.Count; i++)
        {
            questDatabase.quests[i].IsReached = false;
            questDatabase.quests[i].valueCount = 0;
            questDatabase.quests[i].isActive = false;

            for (int a = 0; a < questDatabase.quests[i].currentAmount.Count; a++)
            {
                questDatabase.quests[i].currentAmount[a] = 0;
            }
        }

        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            itemDatabase.items[i].itemAmount = 0;
        }



        //위치 로드
        playerPos = playData.playerPos ;
        playerRot = playData.playerRot ;

        GameManager.instance.playTime = playData.tempPlayTime;

        //지역 정보 및 플탐 저장시간 로드
        playerStatus.saveChapterName = playData.saveChapterName;
        playerStatus.saveAreaName = playData.saveAreaName;
        playerStatus.saveTime = playData.saveTime;
        playerStatus.playTime = playData.playTime;
        saveWorldName = playData.saveWorldName;

        //이름 레벨 골드 경험치 로드
        playerStatus.playerName = playData.playerName;
        playerStatus.playerLV = playData.playerLV;
        playerStatus.playerGold = playData.playerGold;
        playerStatus.playerExp = playData.playerExp;

        //체력이랑 SP 로드
        playerStatus.playerHP = playData.playerHP;
        playerStatus.playerCurrentHp = playData.playerCurrentHp;
        playerStatus.playerSP = playData.playerSP;
        playerStatus.playerCurrentSp = playData.playerCurrentSp;

        // 공격력 방어력 수비력 로드
        playerStatus.playerEva = playData.playerEva;
        playerStatus.playerAtk = playData.playerAtk;
        playerStatus.playerDef = playData.playerDef;

        //어빌리티 강화된 수치 로드
        playerStatus.twoHandAbilityStat = playData.twoHandAbilityStat;
        playerStatus.oneHandAbilityStat = playData.oneHandAbilityStat;
        playerStatus.scytheAbilityStat = playData.scytheAbilityStat;
        playerStatus.spearAbilityStat = playData.spearAbilityStat;

        playerStatus.secondWpnList.Clear();
        playerStatus.playerisHoldingList.Clear();

        for (int i = 0; i < playerStatus.playerisHoldingList.Count; i++)
        {
            
            Debug.Log("제거제거"+playerStatus.playerisHoldingList[i]);
        }

        //장착 무기및 보조무기 3개 로드
        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            if(playData.WearingWpn == itemDatabase.items[i].itemID)
            {
                playerStatus.WearingWpn = itemDatabase.items[i];
                playerStatus.playerisHoldingList.Add(itemDatabase.items[i].itemName);
            }
            for(int a = 0; a < playData.secondWpnList.Length; a++)
            {
                if(playData.secondWpnList[a]== itemDatabase.items[i].itemID)
                {
                    playerStatus.secondWpnList.Add(itemDatabase.items[i]);
                    playerStatus.playerisHoldingList.Add(itemDatabase.items[i].itemName);
                }
            }
        }
        for(int i =0; i< playerStatus.playerisHoldingList.Count; i++)
        {
            Debug.Log(playerStatus.playerisHoldingList[i]);
        }

        //인벤토리 로드

        inven.inventoryItemList.Clear();

        for(int i = 0; i < playData.inventoryItemIDList.Length; i++)
        {
            for (int a = 0; a < itemDatabase.items.Count; a++)
            {
                if (playData.inventoryItemIDList[i] == itemDatabase.items[a].itemID)
                {
                    inven.inventoryItemList.Add(itemDatabase.items[a]);
                    inven.inventoryItemList[i].itemAmount = playData.itemAmountAry[i];
                }
            }
        }
        //어빌리티 강화 정도 로드
        for (int i = 0; i < playData.weaponCharaIdx.Length; i++)
        {
            weaponAbility.weaponCharas[i].Charaidx = playData.weaponCharaIdx[i];
            weaponAbility.weaponCharas[i].level = playData.weaponCharaLevel[i];
        }

        //진행중 퀘스트, 완료 퀘스트 로드

        quest.activeQuests.Clear();
        quest.completeQuests.Clear();



        for(int i = 0; i < playData.activeQuestsID.Length; i++)
        {
            for(int a = 0; a < questDatabase.quests.Count; a++)
            {
                if (playData.activeQuestsID[i] == questDatabase.quests[a].questID)
                {
                    quest.activeQuests.Add(questDatabase.quests[a]);
                }
            }
        }

        for (int i = 0; i < playData.completedQuestsID.Length; i++)
        {
            for (int a = 0; a < questDatabase.quests.Count; a++)
            {
                if (playData.completedQuestsID[i] == questDatabase.quests[a].questID)
                {
                    quest.completeQuests.Add(questDatabase.quests[a]);
                }
            }
        }

        for (int i = 0; i < playData.activeQuestsID.Length; i++)
        {
            quest.activeQuests[i].IsReached = playData.isReachedAry[i];
            quest.activeQuests[i].valueCount = playData.valueCountAry[i];
            quest.activeQuests[i].isActive = playData.isActiveAry[i];

            for (int a = 0; a < quest.activeQuests[i].currentAmount.Count; a++)
            {
                quest.activeQuests[i].currentAmount = playData.currentAmountAry[i].target.ToList();
            }

            if(quest.activeQuests[i].questID == playData.trackingQuestInt)
            {
                quest.trackingQuest = quest.activeQuests[i];
            }
        }
        for (int i = 0; i < playData.completedQuestsID.Length; i++)
        {

            quest.completeQuests[i].IsReached = playData.isReachedComAry[i];
            quest.completeQuests[i].valueCount = playData.valueCountComAry[i];
            quest.completeQuests[i].isActive = playData.isActiveComAry[i];

            for (int a = 0; a < quest.completeQuests[i].currentAmount.Count; a++)
            {
                quest.completeQuests[i].currentAmount = playData.currentAmountComAry[i].target.ToList();
            }
        }

        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            SaveSlotBtns[labis].GetComponent<Button>().enabled = false;
        }

        for (int i = 0; i < noneNpcChat.cA.Length; i++)
        {
            noneNpcChat.cA[i].areaEnable = playData.areaEnable[i];
        }
        playerStatus.firstSecondaryWeapon = playData.firstSecondaryWeapon;

        //몬스터 메뉴 초기화 및 로드
        monMenu.monsterBoolDictionary = playData.monsterBoolDictionary;

        for (int i = 0; i < monMenu.monsterBoolDictionary.Length; i++)
        {
            monMenu.monsterDictionaryList.transform.GetChild(i).GetComponent<MonsterDictionary>().GetMonsterBtnNames();
        }

        //현재 전투 필드 cnt 
        currentBattleFieldCnt = playData.currentBattleFieldCnt;

        StartCoroutine(StartLoadScene(playerStatus.saveAreaName, playerStatus.saveChapterName));
    }
    public void StartLoadSceneVoid(string sceneArea, string chapterName)
    {
        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            SaveSlotBtns[labis].GetComponent<Button>().enabled = false;

        }
        StartCoroutine(StartLoadScene(sceneArea, chapterName));
    }
    public IEnumerator StartLoadScene(string sceneArea,string chapterName)
    {
        if(SC.play == true)
            SC.StartStop();

        EventSystem.current.SetSelectedGameObject(null);
        float fadeTime1 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DoF.active = false;
        portalLoadingScreen.SetActive(true);
        vignette.intensity.value = 0.18f;

        StopCoroutine(lobbyUIClass.expoLobby);
        RenderSettings.skybox = lobbyUIClass.skyBoxes[1];
        RenderSettings.skybox.SetFloat("_Exposure", 0.63f);
        Destroy(lobbyUIClass.tempLobbyPrefab);
        lobbyUIClass.lobbyScreen.SetActive(false);

        if (tempBattleField != null)
        {
            Destroy(tempBattleField);
        }
        tempBattleField = Instantiate(battleFieldPrefabs[currentBattleFieldCnt]);

        AsyncOperation desScene;

        lobbyUI.SetActive(false);
        saveFilesMenu.SetActive(false);
        GameObject camtemp = Instantiate(CameraArmPrefab);
        fieldCamera = camtemp;
        player = Instantiate(playerPrefab);

        characterController = player.GetComponent<TPSCharacterController>();

        playerPos = playData.playerPos;
        playerRot = playData.playerRot;

        yield return new WaitForSeconds(0.1f);

        desScene = SceneManager.LoadSceneAsync(sceneArea, LoadSceneMode.Additive);
        desScene.allowSceneActivation = false;
        float fadeTime2 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(-1);
        yield return new WaitForSeconds(fadeTime2);
        while (!desScene.isDone)
        {
            float progress = Mathf.Clamp01(desScene.progress / 0.9f);

            loadingProgressBarImage.fillAmount = progress;
            loadingProgressText.text = (progress * 100f).ToString() + "%";

            if (desScene.progress >= 0.9f)
            {
                desScene.allowSceneActivation = true;
            }
            yield return null;
        }

        camtemp.transform.position = playerPos;
        player.transform.position = playerPos;
        player.transform.rotation = playerRot;
        playerStatus.saveChapterName = chapterName;
        playerStatus.saveAreaName = sceneArea;


        //yield return new WaitForSeconds(0.25f);
        float fadeTime3 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime3);
        portalLoadingScreen.SetActive(false);
        lobbyCam.SetActive(false);
        camtemp.transform.GetChild(0).GetComponent<FadeInFadeOut>().enabled = true;


        SC.PlayBgm(playerStatus.saveAreaNum);

        isLobby = false;
        isPlayingGame = true;

        ShowAreaInfo();
        if (quest.trackingQuest != null)
        {
            quest.UpdateTrackingQuest();
            ShowTrackingQuest();
        }

        playerPos = new Vector3(0, 0, 0);
        playerRot = Quaternion.Euler(0, 0, 0);
    }



    public void FirstStart()
    {
        SC.Effects[0].PlayOneShot(SC.uiEffectSounds[4]);
        lobbyUIClass.firststartBtn.GetComponent<Button>().enabled = false;
        noneNpcChat.tutorial = true;

        inven.inventoryItemList.Clear();
        quest.completeQuests.Clear();
        quest.activeQuests.Clear();

        for (int i = 0; i < questDatabase.quests.Count; i++)
        {
            questDatabase.quests[i].IsReached = false;
            questDatabase.quests[i].valueCount = 0;
            questDatabase.quests[i].isActive = false;

            for (int a = 0; a < questDatabase.quests[i].currentAmount.Count; a++)
            {
                questDatabase.quests[i].currentAmount[a] = 0;
            }
        }

        for(int i = 0; i < itemDatabase.items.Count; i++)
        {
            itemDatabase.items[i].itemAmount = 0;
        }

        for (int i = 0; i < monMenu.monsterBoolDictionary.Length; i++)
        {
            monMenu.monsterBoolDictionary[i] = false;
            monMenu.monsterDictionaryList.transform.GetChild(i).GetComponent<MonsterDictionary>().GetMonsterBtnNames();
        }

        StartCoroutine(FirstStartBtn());
    }

    public IEnumerator FirstStartBtn()
    {
        EventSystem.current.SetSelectedGameObject(null);
        float fadeTime1 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DoF.active = false;
        vignette.intensity.value = 0.18f;
        StopCoroutine(lobbyUIClass.expoLobby);
        RenderSettings.skybox = lobbyUIClass.skyBoxes[1];
        RenderSettings.skybox.SetFloat("_Exposure", 0.63f);
        Destroy(lobbyUIClass.tempLobbyPrefab);
        lobbyUIClass.lobbyScreen.SetActive(false);

        if (tempBattleField != null)
        {
            Destroy(tempBattleField);
        }
        currentBattleFieldCnt = 0;
        tempBattleField = Instantiate(battleFieldPrefabs[0]);


        portalLoadingScreen.SetActive(true);
        AsyncOperation desScene;

        lobbyUI.SetActive(false);
        saveFilesMenu.SetActive(false);
        GameObject camtemp = Instantiate(CameraArmPrefab);
        fieldCamera = camtemp;
        player = Instantiate(playerPrefab);
        characterController = player.GetComponent<TPSCharacterController>();
        characterController.stopMove = true;
        playerPos = new Vector3(34, 0f, -51);
        playerRot = Quaternion.Euler(0f, -93, 0f);
        fieldCamera.transform.rotation = Quaternion.Euler(10.9f, -64, 0);

        yield return new WaitForSeconds(0.1f);
        desScene = SceneManager.LoadSceneAsync("Area_A1", LoadSceneMode.Additive);
        desScene.allowSceneActivation = false;
        float fadeTime2 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(-1);
        yield return new WaitForSeconds(fadeTime2);
        while (!desScene.isDone)
        {
            float progress = Mathf.Clamp01(desScene.progress / 0.9f);

            loadingProgressBarImage.fillAmount = progress;
            loadingProgressText.text = (progress * 100f).ToString() + "%";

            if (desScene.progress >= 0.9f)
            {
                desScene.allowSceneActivation = true;
            }
            yield return null;
        }
        camtemp.transform.position = new Vector3(41, 4f, -63);
        player.transform.position = playerPos;  
        player.transform.rotation = playerRot;
        playerStatus.saveChapterName = "프롤로그";
        playerStatus.saveAreaName = "Area_A1";
        saveWorldName = "오르투스";

        float fadeTime3 = lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime3);
        portalLoadingScreen.SetActive(false);
        lobbyCam.SetActive(false);
        noneNpcChat.TutorialStart();
        camtemp.transform.GetChild(0).GetComponent<FadeInFadeOut>().enabled = true;
        isLobby = false;
        isPlayingGame = true;
        ShowAreaInfo();
        ShowTrackingQuest();
        playerPos = new Vector3(0, 0, 0);
        playerRot = Quaternion.Euler(0, 0, 0);
    }

    public void MiniLoad()
    {
        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            string path = (Application.persistentDataPath + "/playData_" + i + ".json");

            if (!File.Exists(path))
            {
                continue;

            }

            lobbyUIClass.continueStartBtn.GetComponent<Button>().interactable = true;
            string jsonData = File.ReadAllText(path);

            playData = JsonUtility.FromJson<PlayData>(jsonData);


            //지역 정보 및 플탐 저장시간 로드
            SaveSlotBtns[i].transform.GetChild(0).GetComponent<Text>().text = playData.saveTime;
            SaveSlotBtns[i].transform.GetChild(1).GetComponent<Text>().text = playData.saveChapterName+":";
            SaveSlotBtns[i].transform.GetChild(2).GetComponent<Text>().text = playData.saveAreaName;
            SaveSlotBtns[i].transform.GetChild(3).GetComponent<Text>().text = "LEVEL "+ playData.playerLV.ToString();
            SaveSlotBtns[i].transform.GetChild(5).GetComponent<Text>().text = "PLAY TIME  " + playData.playTime;



            for (int j = 0; j < itemDatabase.items.Count; j++)
            {
                if (playData.WearingWpn == itemDatabase.items[j].itemID)
                {
                    SaveSlotBtns[i].transform.GetChild(6).GetChild(0).GetComponent<Image>().sprite = itemDatabase.items[j].itemIcon;
                }
                for (int a = 0; a < playData.secondWpnList.Length; a++)
                {
                    if (playData.secondWpnList[a] == itemDatabase.items[j].itemID)
                    {
                        SaveSlotBtns[i].transform.GetChild(6).GetChild(a+1).GetComponent<Image>().sprite = itemDatabase.items[j].itemIcon;
                    }
                }
            }

            Button btn = GameManager.instance.SaveSlotBtns[labis].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            //btn.onClick.AddListener(() => );
            btn.onClick.AddListener(() => GameManager.instance.Load(labis));
            Debug.Log(labis);

            SaveSlotBtns[i].transform.GetChild(7).gameObject.SetActive(false);
        }
    }

    public void SelectStart()
    {
        SC.Effects[0].PlayOneShot(SC.uiEffectSounds[4]);
        //lobbyUIClass.saveFilesDarkBG.SetActive(false);
        saveFilesMenu.SetActive(true);
        DoF.active = true;
        EventSystem.current.SetSelectedGameObject(AutoSavedSlotBtn);
    }
    public void CheckPlayerLV()
    {
        if (playerStatus.playerCurrentExp >= playerStatus.playerExp)
        {
            int overloadedExp;
            overloadedExp = playerStatus.playerCurrentExp - playerStatus.playerExp;

            playerStatus.playerLV++;
            playerStatus.playerAtk += 2;
            playerStatus.playerDef += 2;
            playerStatus.playerHP += 10;
            playerStatus.playerSP += 10;

            playerStatus.playerCurrentHp = playerStatus.playerHP;
            playerStatus.playerCurrentSp = playerStatus.playerSP;

            playerStatus.playerExp = Mathf.RoundToInt(playerStatus.playerExp * 1.2f);
            playerStatus.playerCurrentExp = overloadedExp;
        }
        if (playerStatus.playerCurrentExp >= playerStatus.playerExp)
        {
            CheckPlayerLV();
        }
    }

    public IEnumerator StartBattle()
    {
        Time.timeScale = 0f;
        scenesToLoad.Clear();
        CloseTrackingQuest();

        battleTransitionBG.SetActive(true);

        Image t = battleTransitionBG.GetComponent<Image>();

        float time = 0.6f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            chromaAberra.intensity.value = Mathf.Lerp(0, 1, (elapsedTime / time));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.01f);
        float colortime = 0.4f;
        float elapsedcolorTime = 0;
        while (elapsedcolorTime < colortime)
        {
            float a = Mathf.Lerp(0, 1, (elapsedcolorTime / colortime));
            t.color = new Color(0, 0, 0, a);
            elapsedcolorTime += Time.unscaledDeltaTime;
            yield return null;
        }
        t.color = new Color(0, 0, 0, 1);

        isBattle = true;

        yield return new WaitForSecondsRealtime(0.1f);

        scenesToLoad.Add(SceneManager.LoadSceneAsync(("BattleScene"), LoadSceneMode.Additive));

        float totalProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                yield return null;
            }
        }


        Time.timeScale = 1f;
        player.SetActive(false);
        fieldCamera.SetActive(false);
        chromaAberra.intensity.value = 0;
        Destroy(hitMonsterObj);

    }

    public void ShowAreaInfo()
    {
        RectTransform infoUI = AreaInfoUI.GetComponent<RectTransform>();

        AreaInfoUI.transform.GetChild(1).GetComponent<Text>().text = saveWorldName;
        AreaInfoUI.transform.GetChild(2).GetComponent<Text>().text = playerStatus.saveAreaName;

        LeanTween.cancel(infoUI);
        LeanTween.moveX(infoUI, 0, 1.0f).setDelay(0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
        LeanTween.moveX(infoUI, -360, 1.0f).setDelay(2.6f).setEase(LeanTweenType.easeInCubic).setIgnoreTimeScale(true);
    }

    public void ShowTrackingQuest()
    {
        RectTransform infoUI = TrackingQuestUI.GetComponent<RectTransform>();

        if (quest.trackingQuest != null)
        {
            quest.UpdateTrackingQuest();
            LeanTween.moveX(infoUI, 1365, 1.0f).setDelay(0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
        }
    }

    public void CloseTrackingQuest()
    {
        RectTransform infoUI = TrackingQuestUI.GetComponent<RectTransform>();
        LeanTween.moveX(infoUI, 1921, 0.5f).setDelay(0.1f).setEase(LeanTweenType.easeInCubic).setIgnoreTimeScale(true);
    }
    
    public void StartEndBattle()
    {
        StartCoroutine(EndBattle());
    }

    public IEnumerator EndBattle()
    {
        if (SC.play == true)
            SC.StartStop();
            

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        BattleLoadingMenu.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        player.SetActive(true);
        player.transform.position = playerPos;
        SceneManager.UnloadSceneAsync("BattleScene");
        fieldCamera.SetActive(true);
        characterController.stopCam = false;
        BattleLoadingMenu.SetActive(false);

        SC.PlayBgm(playerStatus.saveAreaNum);

        ShowAreaInfo();
        ShowTrackingQuest();
        isBattle = false;
        yield return new WaitForSeconds(0.2f);

        if (noneNpcChat.tutorial)
        {
            noneNpcChat.tutorial = false;
            npcChatUI.transform.GetChild(3).gameObject.SetActive(true);
            characterController.npc.GetComponent<TypeWriterEffect>().ChatStart();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");

        if (scene.name == "MainScene")
        {
            player.transform.position = playerPos;
            player.transform.rotation = playerRot;
            playerPos = new Vector3(0, 0, 0);
            playerRot = Quaternion.Euler(0, 0, 0);
            BattleLoadingMenu.SetActive(false);
        }


    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ShowItemGet()
    {
        RectTransform infoUI = itemGetUI.GetComponent<RectTransform>();

        LeanTween.moveX(infoUI, -964, 1.0f).setDelay(0.1f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
    }

    public void CloseItemGet()
    {
        RectTransform infoUI = itemGetUI.GetComponent<RectTransform>();

        LeanTween.moveX(infoUI, -662f, 1.0f).setDelay(2.2f).setEase(LeanTweenType.easeInCubic).setIgnoreTimeScale(true);
    }

    public void ShowNpcChatUI()
    {
        LeanTween.size(npcSelectUI.GetComponent<RectTransform>(), new Vector2(442,42), 0.55f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
    }
    public void CloseNpcChatUI()
    {
        LeanTween.size(npcSelectUI.GetComponent<RectTransform>(), new Vector2(0, 42), 0.55f).setEase(LeanTweenType.easeInCubic).setIgnoreTimeScale(true);
    }

    #region Update
    public void MenuUpdate(UiState prevState)
    {
        CloseNpcChatUI();
        InventoryOff();
        StatusOff();
        WeaponOff();
        QuestOff();
        OptionOff();
        MonsterMenuOff();
        switch (State)
        {
            case UiState.OffMenu:
                EventSystem.current.SetSelectedGameObject(null);
                OffMenu();
                foreach (GameObject gameObject in uiTabs)
                {
                    gameObject.GetComponent<Image>().sprite = uiTabSpriteDeselected;
                }
                break;
            case UiState.OnMenu:
                OnMenu();
                inven.State = InventoryState.InventoryOff;
                quest.State = QuestUiState.OffQuestUi;
                monMenu.State = MonsterMenuState.MonsterMenuOff;
                weaponAbility.State = AbilityUIState.OFFUi;
                playerStatus.State = PlayerStatusState.StstusOff;
                option.State = OptionUiState.OffUi;
                break;
            case UiState.Inventory:
                UITabsSelected = 0;
                InventoryOn(prevState);
                break;
            case UiState.PlayerStatus:
                UITabsSelected = 1;
                StatusOn(prevState);
                break;
            case UiState.Weapon:
                UITabsSelected = 2;
                WeaponOn(prevState);
                break;
            case UiState.Monster:
                UITabsSelected = 3;
                MonsterMenuOn(prevState);
                break;
            case UiState.Quest:
                UITabsSelected = 4;
                QuestOn(prevState);
                break;
            case UiState.Option:
                UITabsSelected = 5;
                OptionOn(prevState);
                break;
        }
        if(State > UiState.OffMenu)
        {
            CloseTrackingQuest();
        }
        isTabs = false;
    }//State가 변경되었을때 State에 맞게 실행할 함수들
    public void MoveTabUnderbar()
    {
        switch (UITabsSelected)
        {
            case 0:
                LeanTween.moveX(TopBar, 0, 0.1f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
            case 1:
                LeanTween.moveX(TopBar, 204, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
            case 2:
                LeanTween.moveX(TopBar, 409, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
            case 3:
                LeanTween.moveX(TopBar, 614, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
            case 4:
                LeanTween.moveX(TopBar, 818, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
            case 5:
                LeanTween.moveX(TopBar, 1021, 0.2f).setEase(LeanTweenType.easeOutCubic).setIgnoreTimeScale(true);
                break;
        }
    }
    public IEnumerator TimeChange()
    {
        float daytime = 7f;
        float nightTime = 7f;

        float skyBocExpoCurrentValue = 0;

        float currentColorX = 178f;
        float currentColorY = 158f;
        float currentColorZ = 157f;

        float lightCurrentRot = 67;

        float elapsedTime = 0;
        while (elapsedTime < daytime)
        {
            //스카이박스 로테이션
            //RenderSettings.skybox.SetFloat("_Rotation", skyBoxRotvalue);
            //skyBoxRotvalue = Mathf.Lerp(skyBoxRotstartingValue, skyBoxRotfinalValue, (elapsedTime / daytime));

            //스카이박스 exposure
            RenderSettings.skybox.SetFloat("_Exposure", skyBocExpoCurrentValue);
            skyBocExpoCurrentValue = Mathf.Lerp(0.16f, 0.7f, (elapsedTime / daytime));

            //directionalLight 컬러값
            directionalLight.color = new Color(currentColorX / 255, currentColorY / 255, currentColorZ / 255);
            currentColorX = Mathf.Lerp(82, 178, (elapsedTime / daytime));
            currentColorY = Mathf.Lerp(79, 157, (elapsedTime / daytime));
            currentColorZ = Mathf.Lerp(79, 157, (elapsedTime / daytime));

            //directionalLight 로테이션
            directionalLightTransform.localRotation = Quaternion.Euler(lightCurrentRot, 0, 0);
            lightCurrentRot = Mathf.Lerp(0, 90, (elapsedTime / daytime));

            //sMH.midtones.value = 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);
        Debug.Log("해가 집니다");
        elapsedTime = 0;
        while (elapsedTime < nightTime)
        {
            //스카이박스 로테이션
            //RenderSettings.skybox.SetFloat("_Rotation", skyBoxRotvalue);
            //skyBoxRotvalue = Mathf.Lerp(skyBoxRotstartingValue, skyBoxRotfinalValue, (elapsedTime / daytime));

            //스카이박스 exposure
            RenderSettings.skybox.SetFloat("_Exposure", skyBocExpoCurrentValue);
            skyBocExpoCurrentValue = Mathf.Lerp(0.7f, 0.16f, (elapsedTime / nightTime));

            //directionalLight 컬러값
            directionalLight.color = new Color(currentColorX / 255, currentColorY / 255, currentColorZ / 255);
            currentColorX = Mathf.Lerp(178, 82, (elapsedTime / nightTime));
            currentColorY = Mathf.Lerp(157, 79, (elapsedTime / nightTime));
            currentColorZ = Mathf.Lerp(157, 79,  (elapsedTime / nightTime));

            //directionalLight 로테이션
            directionalLightTransform.localRotation = Quaternion.Euler(lightCurrentRot, 0, 0);
            lightCurrentRot = Mathf.Lerp(90, 180, (elapsedTime / nightTime  ));

            //sMH.midtones.value = 

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
    
    
    public void TabsUpdate(int _uiTabs)
    {
        InventoryOff();
        StatusOff();
        WeaponOff();
        QuestOff();
        OptionOff();
        MonsterMenuOff();
        //바꿔
        playerStatus.ChangeHoldinglist();
        switch (_uiTabs)
        {
            case 0:
                InventoryOn(State);
                //inven.InActiveItemTabs();
                //EventSystem.current.SetSelectedGameObject(uiTabs[0]);
                break;
            case 1:
                StatusOn(State);
                //playerStatus.InactiveStatBtn();
                //EventSystem.current.SetSelectedGameObject(uiTabs[1]);
                break;
            case 2:
                WeaponOn(State);
                weaponAbility.InActiveAllButtons();
                //EventSystem.current.SetSelectedGameObject(uiTabs[2]);
                break;
            case 3:
                MonsterMenuOn(State);
                break;
            case 4:
                QuestOn(State);
                break;
            case 5:
                OptionOn(State);
                break;
        }
    }//OnMenu 상태일때 실행
    public void ChangeUiTabImage(int x)
    {
        for (int i = 0; i < uiTabs.Count; i++)
        {
            uiTabs[i].GetComponent<Image>().sprite = uiTabSpriteDeselected;
            if (i == x)
            {
                uiTabs[i].GetComponent<Image>().sprite = uiTabSpriteSelected;
            }
        }
    }
    #endregion


    public void MonsterMenuOn(UiState prevState)
    {
        if (isMonsterOn && !isBattle)
        {
            if (prevState != UiState.OnMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                ChangeUiTabImage(0);
            }
            monsterDictionaryUI.SetActive(true);
        }
        else if (isMenuOn)
        {
            Debug.Log("켜졌다");
            monsterDictionaryUI.SetActive(true);
        }
    }
    public void MonsterMenuOff()
    {
        monsterDictionaryUI.SetActive(false);
    }
    public void MonsterStateOn()
    {
        State = UiState.Monster;
    }

    #region MenuFuction
    public void OnMenu()
    {
        if (EventSystem.current.currentSelectedGameObject == null || State == UiState.OnMenu)
        {
            EventSystem.current.SetSelectedGameObject(uiTabs[UITabsSelected]);
            TabsUpdate(UITabsSelected);
            ChangeUiTabImage(uiTabsSelected);
            //Debug.Log("Onmenu");
        }
        ActiveMenuButtons();
        Time.timeScale = 0f;
        mainMenu.SetActive(true);
        DoF.active = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<TPSCharacterController>().enabled = false;
    }
    public void OffMenu()
    {
        InActiveMenuButtons();
        if (!isLobby)
        {
            ShowTrackingQuest();
        }
        Time.timeScale = 1f;
        mainMenu.SetActive(false);
        DoF.active = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<TPSCharacterController>().enabled = true;
    }
    public void MainMenuOnOffState()
    {
        if (!isMenu)
        {
            State = UiState.OnMenu;
            SC.Effects[0].PlayOneShot(SC.uiEffectSounds[3]);
        }
        else if (isMenuOn)
        {
            State = UiState.OffMenu;
            SC.Effects[0].PlayOneShot(SC.uiEffectSounds[7]);
        }
        else if (isInventoryOn)
        {
            inven.EscapeInventory();
            if (!inven.IsInventoryActive)
            {
                State = UiState.OnMenu;
            }
        }
        else if (isPlayerStatusOn)
        {
            playerStatus.EscapePlayerStatus();
            if (!playerStatus.IsPlayerStatusActive)
            {
                State = UiState.OnMenu;
            }
        }
        else if (isMonsterOn)
        {
            monMenu.EscapeMonMenu();
            if (!monMenu.IsMonmenuActive)
            {
                State = UiState.OnMenu;
            }
        }
        else if (isWeaponOn)
        {
            weaponAbility.EscapeAbilityUI();
            if (!weaponAbility.isAbilityOn)
            {
                State = UiState.OnMenu;
            }
        }
        else if (isQuestOn)
        {
            quest.EscapeQuest();
            if (!quest.isQuestUiOn)
            {
                State = UiState.OnMenu;
            }
        }
        else if (isOptionOn)
        {
            option.EscapeOption();
            if (!option.isOptionOn)
            {
                State = UiState.OnMenu;
            }
        }
    }
    public void InActiveMenuButtons()
    {
        foreach (var buttons in uiTabs)
        {
            Navigation NewNav = new Navigation();
            NewNav.mode = Navigation.Mode.None;

            //buttons.GetComponent<Button>().navigation = NewNav;
            //buttons.GetComponent<Button>().interactable = false;
        }
    }
    public void ActiveMenuButtons()
    {
        /*foreach (var buttons in uiTabs)
        {
            Navigation NewNav = new Navigation();
            NewNav.mode = Navigation.Mode.Automatic;

            //buttons.GetComponent<Button>().navigation = NewNav;

            //buttons.GetComponent<Button>().interactable = true;
        }*/
    }

    #endregion
    #region InventoryFunction
    public void InventoryOn(UiState prevState)
    {
        if (isInventoryOn && !isBattle)
        {
            if (prevState != UiState.OnMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                ChangeUiTabImage(0);
                EventSystem.current.SetSelectedGameObject(inven.ItemTabs[0]);
            }
            inven.InventoryUI.SetActive(true);
            inven.ActiveItemTabs();
            inven.ShowItem();
        }
        else if (isMenuOn)
        {
            inven.InventoryUI.SetActive(true);
            inven.ActiveItemTabs();
            inven.ShowItem();
        }
    }
    public void InventoryOff()
    {
        inven.InActiveItemButtons();
        inven.InventoryUI.SetActive(false);
        inven.isShowItemDescrip = false;
    }
    public void InventoryStateOn()
    {
        State = UiState.Inventory;
    }
    #endregion
    #region StatusFunction
    public void StatusOn(UiState prevState)
    {
        if (isPlayerStatusOn && !isBattle)
        {
            if (prevState != UiState.OnMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                ChangeUiTabImage(1);
                EventSystem.current.SetSelectedGameObject(playerStatus.mainWpnBtn);
            }
            PlayerStatusMenu.SetActive(true);
            playerStatus.deleteSecondWpns();
            playerStatus.deleteOtherWpns();
            playerStatus.getOtherWpns();
            playerStatus.getEquipWpns();
            playerStatus.ActiveStatBtn();
            playerStatus.UpdatePreviewPlayerStats();
        }
        else if (isMenuOn)
        {
            PlayerStatusMenu.SetActive(true);
            playerStatus.deleteSecondWpns();
            playerStatus.deleteOtherWpns();
            playerStatus.getOtherWpns();
            playerStatus.getEquipWpns();
            playerStatus.UpdatePreviewPlayerStats();
        }
    }

    public void StatusOff()
    {
        PlayerStatusMenu.SetActive(false);
    }
    public void StatusStateOn()
    {
        State = UiState.PlayerStatus;
    }
    #endregion
    #region WeaponStatus
    public void WeaponOn(UiState prevState)
    {
        if (isWeaponOn && !isBattle)
        {
            if (prevState != UiState.OnMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                ChangeUiTabImage(2);
                EventSystem.current.SetSelectedGameObject(weaponAbility.weaponStatus[0]);
            }
            weaponAbility.abilityMenu.SetActive(true);
            weaponAbility.ActiveAllButtons();
        }
        else if (isMenuOn)
        {
            weaponAbility.abilityMenu.SetActive(true);
        }
    }
    public void WeaponOff()
    {
        weaponAbility.abilityMenu.SetActive(false);
    }
    public void WeaponStateOn()
    {
        State = UiState.Weapon;
    }
    #endregion
    #region QuestFunction
    public void QuestOn(UiState prevState)
    {
        if (isQuestOn && !isBattle)
        {
            Debug.Log("state:" + State + " prevState:" + prevState);
            if (prevState == UiState.OffMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                EventSystem.current.SetSelectedGameObject(quest.questTabs[0]);
                ChangeUiTabImage(4);
            }
            quest.questMask.SetActive(true);


            if (quest.selectedTabQuests.Count != 0)
            {
                quest.QuestDescriptionArea(0);
            }
            else
            {
                switch (quest.selectedTab)
                {
                    case 0:
                        quest.isActiveQuest = true;
                        quest.CreateActiveMainQuestButton();
                        break;
                    case 1:
                        quest.isActiveQuest = true;
                        quest.CreateActiveSubQuestButton();
                        break;
                    case 2:
                        quest.isActiveQuest = false;
                        quest.CreateCompleteQuestButton();
                        break;
                }
            }
        }
        if (isMenuOn)
        {
            quest.questMask.SetActive(true);
            switch (quest.selectedTab)
            {
                case 0:
                    quest.CreateActiveMainQuestButton();
                    break;
                case 1:
                    quest.CreateActiveSubQuestButton();
                    break;
                case 2:
                    quest.CreateCompleteQuestButton();
                    break;
            }
            quest.isActiveQuest = true;
            if (quest.activeQuests.Count != 0)
            {
                quest.QuestDescriptionArea(quest.selectedTab);
            }
        }
    }
    public void QuestOff()
    {
        quest.questMask.SetActive(false);
    }
    public void QuestStateOn()
    {
        State = UiState.Quest;
    }
    #endregion
    #region OptionFunction
    public void OptionOn(UiState prevState)
    {
        if (isOptionOn && !isBattle)
        {
            if (prevState == UiState.OffMenu)
            {
                Time.timeScale = 0f;
                mainMenu.SetActive(true);
                DoF.active = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterController.enabled = false;
                ChangeUiTabImage(5);
            }
            option.optionUi.SetActive(true);
        }
        else if (isMenuOn)
        {
            option.optionUi.SetActive(true);
        }
    }
    public void OptionOff()
    {
        option.optionUi.SetActive(false);
    }
    #endregion
}