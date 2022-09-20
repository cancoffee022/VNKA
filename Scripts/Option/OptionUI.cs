using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum OptionUiState
{
    OffUi,
    OnUi,
    AcceptUi,
    AcceptSaveUi,
    DontAcceptSaveUi,
    SaveUi,
    AcceptSaveBtnUi
}

public class OptionUI : MonoBehaviour
{
    public QuestUI quest;

    [Header("Change Resolution")]
    public bool fullScreen;
    FullScreenMode screenMode;

    [Header("Graphic")]
    public int resolutionIdx;
    public int screenModeIdx;
    public int graphicQuaility;
    public int frame;
    public bool isVsync;
    public bool isPostProcessing;
    public bool isAmbientOcclusion;

    public int prevResolutionIdx;
    public int prevScreenModeIdx;
    public int prevGraphicQuality;
    public int prevFrame;
    public bool prevIsVsync;
    public bool prevIsPostProcessing;
    public bool prevIsAmbientOcclusion;

    public GameObject optionUi;

    public Text resolutionSizeText;

    public GameObject optionUiContents;

    public List<GameObject> screenModeBtn;
    public List<GameObject> graphicQuailityBtn;
    public List<GameObject> frameRateBtn;
    public List<GameObject> vSyncBtn;
    public List<GameObject> postProcessingBtn;
    public List<GameObject> ambientOcclusionBtn;

    public GameObject frameRateOffPanel;
    public GameObject acceptUi;
    public GameObject acceptButton;
    public GameObject denyButton;
    public GameObject resolutionLeftButton;
    public GameObject applyBtn;
    public GameObject ToTitleBtn;
    public GameObject askSaveMenu;
    public GameObject askSaveMenu2;
    public GameObject askAutoSaveMenu;

    public Sprite clickedImage;
    public Sprite selectedImage;
    public Sprite deselectedImage;

    public Color clickedColor;
    public Color selectedColor;
    public Color deSelectedColor;

    public int resolutionWidth;
    public int resolutionHeight;
    private int resolutionNum;
    private int frameRate = 60;
    public int clickedSize;
    public int deselectedSize;
    public int saveCo;

    private List<Navigation> graphicQuailityNavVsyncFalse = new List<Navigation>();
    private List<Navigation> graphicQuailityNavVsyncTrue = new List<Navigation>();
    private List<Navigation> VsyncNavVsyncFalse = new List<Navigation>();
    private List<Navigation> VsyncNavVsyncTrue = new List<Navigation>();

    [SerializeField]
    private OptionUiState state;
    public OptionUiState State
    {
        get => state;
        set
        {
            OptionUiState prevState = state;
            state = value;
            if (prevState != value)
                UpdateOptionUi(prevState);
        }
    }

    public bool isOptionOn => State != OptionUiState.OffUi;

    // Start is called before the first frame update
    void Start()
    {
        fullScreen = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;

        #region Resolution
        if (Screen.width == 800 && Screen.height == 600)
        {
            resolutionNum = 0;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 960 && Screen.height == 540)
        {
            resolutionNum = 1;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 1280 && Screen.height == 720)
        {
            resolutionNum = 2;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 1600 && Screen.height == 900)
        {
            resolutionNum = 3;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 1920 && Screen.height == 1080)
        {
            resolutionNum = 4;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 2560 && Screen.height == 1440)
        {
            resolutionNum = 5;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 2560 && Screen.height == 1080)
        {
            resolutionNum = 6;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        else if (Screen.width == 3440 && Screen.height == 1440)
        {
            resolutionNum = 7;
            Debug.Log("해상도 확인 =" + resolutionNum);
        }
        //저장된 옵션 프로파일 가져와서 60 부분에다 적용시키기
        #endregion

        //frameRate = 60;
        //Application.targetFrameRate = frameRate;

        prevResolutionIdx = resolutionIdx;
        prevScreenModeIdx = screenModeIdx;
        prevGraphicQuality = graphicQuaility;
        prevFrame = frame;
        prevIsVsync = isVsync;
        prevIsPostProcessing = isPostProcessing;
        prevIsAmbientOcclusion = isAmbientOcclusion;
        Debug.Log("StartOptionui");

        SetSelectedImage();

        for (int i = 0; i < graphicQuailityBtn.Count; i++)
        {
            graphicQuailityNavVsyncFalse.Add(graphicQuailityBtn[i].GetComponent<Button>().navigation);
            Navigation nav = graphicQuailityBtn[i].GetComponent<Button>().navigation;
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                    nav.selectOnDown = vSyncBtn[0].GetComponent<Button>();
                    break;
                case 3:
                case 4:
                    nav.selectOnDown = vSyncBtn[1].GetComponent<Button>();
                    break;
            }
            graphicQuailityNavVsyncTrue.Add(nav);
        }

        for (int i = 0; i < vSyncBtn.Count; i++)
        {
            VsyncNavVsyncFalse.Add(vSyncBtn[i].GetComponent<Button>().navigation);
            Navigation nav = vSyncBtn[i].GetComponent<Button>().navigation;
            if (i == 0)
            {
                nav.selectOnUp = graphicQuailityBtn[1].GetComponent<Button>();
            }
            else
                nav.selectOnUp = graphicQuailityBtn[3].GetComponent<Button>();
            VsyncNavVsyncTrue.Add(nav);
        }

        applyBtn.GetComponent<Button>().onClick.AddListener(ActiveApplyUi);
        applyBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.State = UiState.Option);
        applyBtn.GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.AcceptUi);
        applyBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]));

        acceptButton.GetComponent<Button>().onClick.AddListener(ChangeGraphicsOption);
        acceptButton.GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.OnUi);
        acceptButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]));
        denyButton.GetComponent<Button>().onClick.AddListener(SetPrevGraphic);
        denyButton.GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.OnUi);

        ToTitleBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.State = UiState.Option);
        ToTitleBtn.GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.AcceptSaveUi);
        ToTitleBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]));

        askSaveMenu.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.SaveUi);
        askSaveMenu.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]));
        askSaveMenu.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.DontAcceptSaveUi);
        askSaveMenu.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]));
        askSaveMenu2.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.OffUi);
        askSaveMenu2.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]));
        askSaveMenu2.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => State = OptionUiState.OnUi);
        askSaveMenu2.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]));

        //시작할때 값 받아서 적용
        ChangeGraphicsOption();
    }
    #region "BackToLobby & Save"
    public void OnClickToLobby()
    {
        askSaveMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(askSaveMenu.transform.GetChild(4).gameObject);


        for (int i = 0; i < 6; i++)
        {
            int labis = i;

            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().ArrowIcon.SetActive(false);
            LeanTween.cancel(GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().ArrowIcon);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().ArrowIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-37, 0);
            if (labis == 0)
            {
                GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().autoSaveText.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            }
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().saveTime.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().saveWorld.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().saveLevel.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().saveArea.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().playTime.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.instance.SaveSlotBtns[labis].GetComponent<SaveSlotBtns>().Nodata.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);

        }
    }
    //1차 메뉴에서 물어보기
    public void OnClickNoSave()
    {
        askSaveMenu.SetActive(false);
        askSaveMenu2.SetActive(true);
        EventSystem.current.SetSelectedGameObject(askSaveMenu2.transform.GetChild(4).gameObject);
    }
    public void DeActiveAskSave()
    {
        askSaveMenu.SetActive(false);
    }
    public void DeActiveAskSave2()
    {
        askSaveMenu2.SetActive(false);
    }
    public void DeActiveSaveFilesMenu()
    {
        GameManager.instance.saveFilesMenu.SetActive(false);

        LeanTween.cancel(askAutoSaveMenu.transform.GetChild(3).transform.GetChild(1).gameObject);
        LeanTween.cancel(askAutoSaveMenu.transform.GetChild(4).transform.GetChild(1).gameObject);
        askAutoSaveMenu.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
        askAutoSaveMenu.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
        askAutoSaveMenu.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        askAutoSaveMenu.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
    }
    public void DeActiveAskFinalSave()
    {
        askAutoSaveMenu.SetActive(false);
    }
    public void ActiveAskSaveMenu()
    {
        Debug.Log("미니로드");
        /*askSaveMenu.SetActive(false);
        GameManager.instance.saveFilesMenu.SetActive(true);
        GameManager.instance.MiniLoad();

        Button AutoSaveBtn = GameManager.instance.SaveSlotBtns[0].GetComponent<Button>();
        AutoSaveBtn.onClick.RemoveAllListeners();
        AutoSaveBtn.onClick.AddListener(ClickAutosaveAskBtn);

        for (int i = 1; i < 6; i++)
        {
            int labis = i;
            Button btn = GameManager.instance.SaveSlotBtns[labis].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => GameManager.instance.Save(labis));
            btn.onClick.AddListener(() => { State=OptionUiState.AcceptSaveBtnUi;});
            btn.onClick.AddListener(BackToLobbyVoid);
        }
        EventSystem.current.SetSelectedGameObject(GameManager.instance.SaveSlotBtns[0]);*/
        askSaveMenu.SetActive(false);
        GameManager.instance.lobbyUIClass.saveFilesDarkBG.SetActive(true);
        GameManager.instance.saveFilesMenu.SetActive(true);
        GameManager.instance.MiniLoad();

        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            Button btn = GameManager.instance.SaveSlotBtns[labis].GetComponent<Button>();
            btn.GetComponent<Button>().enabled = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { State = OptionUiState.AcceptSaveBtnUi; });
            btn.onClick.AddListener(ClickAutosaveAskBtn);
        }
        EventSystem.current.SetSelectedGameObject(GameManager.instance.SaveSlotBtns[0]);
    }
    //2차 메뉴에서 물어보기
    public void OnClickCancel()
    {
        askSaveMenu2.SetActive(false);
        EventSystem.current.SetSelectedGameObject(ToTitleBtn);
    }

    public void OnClickNoSaveGoLobby()
    {
        askSaveMenu2.SetActive(false);
        GameManager.instance.MiniLoad();
        BackToLobbyVoid();
    }

    public void ClickAutosaveAskBtn()
    {
        askAutoSaveMenu.SetActive(true);
        Debug.Log("번호" + saveCo);
        //안지워지네...
        Button btn = askAutoSaveMenu.transform.GetChild(3).GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => GameManager.instance.Save(saveCo));
        btn.onClick.AddListener(BackToLobbyVoid);

        EventSystem.current.SetSelectedGameObject(askAutoSaveMenu.transform.GetChild(4).gameObject);
    }
    public void ClickAutosaveCancel()
    {
        askAutoSaveMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(GameManager.instance.SaveSlotBtns[saveCo]);
    }


    public void BackToLobbyVoid()
    {
        StartCoroutine(BackToLobby(GameManager.instance.playerStatus.saveAreaName));
    }
    public IEnumerator BackToLobby(string scene)
    {
        float fadeTime = GameObject.Find("CameraArm(Clone)").transform.GetChild(0).GetComponent<FadeInFadeOut>().BeginFade(1);
        yield return new WaitForSecondsRealtime(fadeTime);

        if (GameManager.instance.SC.play == true)
            GameManager.instance.SC.StartStop();

        //스크립터블 오브젝트 퀘스트 초기화
        for (int i = 0; i < quest.activeQuests.Count; i++)
        {
            quest.activeQuests[i].isActive = false;
            quest.activeQuests[i].IsReached = false;
            quest.activeQuests[i].valueCount = 0;

            for (int a = 0; a < quest.activeQuests[i].currentAmount.Count; a++)
            {
                quest.activeQuests[i].currentAmount[a] = 0;
            }

        }
        for (int i = 0; i < quest.completeQuests.Count; i++)
        {
            quest.completeQuests[i].isActive = false;
            quest.completeQuests[i].IsReached = false;
            quest.completeQuests[i].valueCount = 0;

            for (int a = 0; a < quest.completeQuests[i].currentAmount.Count; a++)
            {
                quest.completeQuests[i].currentAmount[a] = 0;
            }
        }

        //나머지 끌꺼 끄고 이동
        SceneManager.UnloadSceneAsync(scene);
        GameManager.instance.vignette.intensity.value = 0.348f;
        Destroy(GameManager.instance.tempBattleField);

        GameManager.instance.optionsUI.SetActive(false);

        GameManager.instance.lobbyUI.SetActive(true);
        GameManager.instance.lobbyUIClass.expoLobby = StartCoroutine(GameManager.instance.lobbyUIClass.exposureSize());
        RenderSettings.skybox = GameManager.instance.lobbyUIClass.skyBoxes[0];
        GameManager.instance.saveFilesMenu.SetActive(false);
        askAutoSaveMenu.SetActive(false);

        GameManager.instance.mainMenu.SetActive(false);

        GameManager.instance.isLobby = true;    
        GameManager.instance.State = UiState.OffMenu;

        Destroy(GameManager.instance.player);
        Destroy(GameObject.Find("CameraArm(Clone)"));


        GameManager.instance.lobbyUIClass.lobbyScreen.SetActive(true);

        GameManager.instance.lobbyUIClass.tempLobbyPrefab = Instantiate(GameManager.instance.lobbyUIClass.lobbyPrefab);

        GameManager.instance.lobbyCam.SetActive(true);

        GameManager.instance.lobbyUIClass.firststartBtn.GetComponent<Button>().enabled = true;

        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            GameManager.instance.SaveSlotBtns[labis].GetComponent<Button>().enabled = true;
        }

        

        for (int i = 0; i < 6; i++)
        {
            int labis = i;
            Button btn = GameManager.instance.SaveSlotBtns[labis].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => GameManager.instance.Load(labis));
        }
        GameManager.instance.lobbyUIClass.PressAnyKey.SetActive(true);
        GameManager.instance.lobbyUIClass.isPressAny = false;
        GameManager.instance.lobbyUIClass.GameStartSelection.SetActive(false);

        float fadeTime1 = GameManager.instance.lobbyCam.GetComponent<FadeInFadeOut>().BeginFade(-1);
        yield return new WaitForSecondsRealtime(fadeTime1);
        GameManager.instance.SC.PlayBgm(0);
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
    }
    #endregion

    public void ResetLeantweenBtn()
    {
        Debug.Log("취소");
        LeanTween.cancel(acceptButton.transform.GetChild(1).gameObject);
        LeanTween.cancel(denyButton.transform.GetChild(1).gameObject);
        LeanTween.cancel(askSaveMenu.transform.GetChild(3).transform.GetChild(1).gameObject);
        LeanTween.cancel(askSaveMenu.transform.GetChild(4).transform.GetChild(1).gameObject);
        LeanTween.cancel(askSaveMenu2.transform.GetChild(3).transform.GetChild(1).gameObject);
        LeanTween.cancel(askSaveMenu2.transform.GetChild(4).transform.GetChild(1).gameObject);

        acceptButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        denyButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        askSaveMenu.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        askSaveMenu.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        askSaveMenu.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
        askSaveMenu.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);

        acceptButton.transform.GetChild(1).gameObject.SetActive(false);
        denyButton.transform.GetChild(1).gameObject.SetActive(false);
        askSaveMenu.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
        askSaveMenu.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
        askSaveMenu2.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
        askSaveMenu2.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
    }

    public void UpdateOptionUi(OptionUiState prevState)
    {
        switch (State)
        {
            case OptionUiState.OffUi:
                optionUiContents.transform.localPosition = new Vector3(0, 0, 0);
                SetSelectedImage();
                SetPrevGraphic();
                break;
            case OptionUiState.OnUi:
                switch (prevState)
                {
                    case OptionUiState.AcceptUi:
                        DeActiveApplyUi();
                        break;
                    case OptionUiState.AcceptSaveUi:
                        DeActiveAskSave();
                        break;
                    case OptionUiState.DontAcceptSaveUi:
                        DeActiveAskSave2();
                        break;
                    case OptionUiState.SaveUi:
                        DeActiveSaveFilesMenu();
                        break;
                }
                if (prevState == OptionUiState.AcceptUi)
                {
                    EventSystem.current.SetSelectedGameObject(applyBtn);
                }
                else if (prevState > OptionUiState.AcceptUi)
                {
                    EventSystem.current.SetSelectedGameObject(ToTitleBtn);
                }
                ResetLeantweenBtn();
                break;
            case OptionUiState.AcceptUi:
                ActiveApplyUi();
                break;
            case OptionUiState.AcceptSaveUi:
                OnClickToLobby();
                break;
            case OptionUiState.DontAcceptSaveUi:
                OnClickNoSave();
                break;
            case OptionUiState.SaveUi:
                if (prevState < OptionUiState.SaveUi)
                    ActiveAskSaveMenu();
                else if (prevState == OptionUiState.AcceptSaveBtnUi)
                {
                    ClickAutosaveCancel();
                }
                break;
            case OptionUiState.AcceptSaveBtnUi:
                break;
        }
        /*if(State)
        if (State == OptionUiState.OnUi&&prevState!=OptionUiState.OffUi)
        {
            DeActiveApplyUi();
        }*/
    }


    #region "SetGraphics"
    public void OnclickResChangeBtn(bool isLeft)
    {
        if (isLeft)
        {
            if (resolutionIdx > 0)
            {
                resolutionIdx--;
            }
        }
        else
        {
            if (resolutionIdx < 7)
            {
                resolutionIdx++;
            }

        }
        SetResolution(resolutionIdx);
        ShowResolutionText();
    }

    public void ShowResolutionText()
    {
        resolutionSizeText.text = resolutionWidth + " X " + resolutionHeight;
    }

    public void ChangeGraphicsOption()
    {
        prevResolutionIdx = resolutionIdx;
        prevScreenModeIdx = screenModeIdx;
        prevGraphicQuality = graphicQuaility;
        prevFrame = frame;
        prevIsVsync = isVsync;
        prevIsPostProcessing = isPostProcessing;
        prevIsAmbientOcclusion = isAmbientOcclusion;
        SetResolution(resolutionIdx);
        ShowResolutionText();
        switch (screenModeIdx)
        {
            case 0:
                screenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                screenMode = FullScreenMode.Windowed;
                break;
        }
        Screen.SetResolution(resolutionWidth, resolutionHeight, screenMode);
        QualitySettings.SetQualityLevel(graphicQuaility);
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
        if (!isVsync)
        {
            switch (frame)
            {
                case 0:
                    Application.targetFrameRate = 30;
                    break;
                case 1:
                    Application.targetFrameRate = 60;
                    break;
                case 2:
                    Application.targetFrameRate = 120;
                    break;
                case 3:
                    Application.targetFrameRate = 144;
                    break;
            }
        }
        GameManager.instance.toneMap.active = isPostProcessing;
        GameManager.instance.bloom.active = isPostProcessing;
        GameManager.instance.hbao.EnableHBAO(isAmbientOcclusion);

        LobbyUI lobby = GameManager.instance.lobbyUIClass;
        lobby.resolutionIdx = resolutionIdx;
        lobby.screenModeIdx = screenModeIdx;
        lobby.SetResolution(resolutionIdx);
        lobby.ShowResolutionText();
        lobby.OnClickScreenModeBtn();
    }

    public void SetPrevGraphic()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
        resolutionIdx = prevResolutionIdx;
        screenModeIdx = prevScreenModeIdx;
        graphicQuaility = prevGraphicQuality;
        frame = prevFrame;
        isVsync = prevIsVsync;
        isPostProcessing = prevIsPostProcessing;
        isAmbientOcclusion = prevIsAmbientOcclusion;
        SetResolution(resolutionIdx);
        SetSelectedImage();
    }
    public void SetResolution(int _resolutionNum)
    {
        switch (_resolutionNum)
        {
            case 0:
                resolutionWidth = 800;
                resolutionHeight = 600;
                break;
            case 1:
                resolutionWidth = 960;
                resolutionHeight = 540;
                break;
            case 2:
                resolutionWidth = 1280;
                resolutionHeight = 720;
                break;
            case 3:
                resolutionWidth = 1600;
                resolutionHeight = 900;
                break;
            case 4:
                resolutionWidth = 1920;
                resolutionHeight = 1080;
                break;
            case 5:
                resolutionWidth = 2560;
                resolutionHeight = 1440;
                break;
            case 6:
                resolutionWidth = 2560;
                resolutionHeight = 1080;
                break;
            case 7:
                resolutionWidth = 3440;
                resolutionHeight = 1440;
                break;
        }
    }
    public void SetFullScreen(int _index)
    {
        screenModeIdx = _index;
    }

    public void SetFrameRate(int _frameRate)
    {
        frame = _frameRate;
    }

    #endregion

    #region ChangeImage
    public void SetSelectedImage()
    {
        Debug.Log("setSelectedImage");
        //ResolutionSnap.GetComponent<ScrollSnap>().ChangePage(prevResolutionIdx);
        SetResolution(resolutionIdx);
        ShowResolutionText();

        for (int i = 0; i < screenModeBtn.Count; i++)
        {
            screenModeBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (i == prevScreenModeIdx)
            {
                screenModeBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                screenModeBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            screenModeBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            screenModeBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        for (int i = 0; i < graphicQuailityBtn.Count; i++)
        {
            graphicQuailityBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (i == prevGraphicQuality)
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            graphicQuailityBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        for (int i = 0; i < frameRateBtn.Count; i++)
        {
            frameRateBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (!prevIsVsync)
            {
                frameRateBtn[i].GetComponent<Button>().interactable = true;
                if (i == prevFrame)
                {
                    frameRateBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                    frameRateBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                    continue;
                }
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
            }
            else
            {
                frameRateBtn[i].GetComponent<Button>().interactable = false;
            }
        }

        vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        if (prevIsVsync)
        {
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }

        postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        if (prevIsPostProcessing)
        {
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }

        ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        if (prevIsAmbientOcclusion)
        {
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        frameRateOffPanel.SetActive(prevIsVsync);
    }//전체 이미지 결정된 값에 따라 바꾸기

    public void SetSelectedScreenModeBtn(BaseEventData baseEventData)
    {
        for (int i = 0; i < screenModeBtn.Count; i++)
        {
            if (baseEventData.selectedObject == screenModeBtn[i])
            {
                screenModeBtn[i].GetComponentInChildren<Text>().color = selectedColor;
                screenModeBtn[i].transform.Find("Image").GetComponent<Image>().color = Color.white;
                continue;
            }
            else if (i == screenModeIdx)
            {
                screenModeBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                screenModeBtn[i].GetComponentInChildren<Text>().color = clickedColor;
            }
            else
            {
                screenModeBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
                screenModeBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
            }
        }
    }
    public void DeSelectedScreenModeBtn()
    {
        for (int i = 0; i < screenModeBtn.Count; i++)
        {
            screenModeBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (i == screenModeIdx)
            {
                screenModeBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                screenModeBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            screenModeBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            screenModeBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    public void OnClickScreenModeBtn()
    {
        for (int i = 0; i < screenModeBtn.Count; i++)
        {
            if (i == screenModeIdx)
            {
                screenModeBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                screenModeBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            screenModeBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            screenModeBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    public void SetSelectedGraphicQuailityBtn(BaseEventData baseEventData)
    {
        for (int i = 0; i < graphicQuailityBtn.Count; i++)
        {
            if (baseEventData.selectedObject == graphicQuailityBtn[i])
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = selectedColor;
                graphicQuailityBtn[i].transform.Find("Image").GetComponent<Image>().color = Color.white;
                continue;
            }
            else if (i == graphicQuaility)
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = clickedColor;
            }
            else
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
            }
        }
    }
    public void DeSelectedGraphicQuailityBtn()
    {
        for (int i = 0; i < graphicQuailityBtn.Count; i++)
        {
            graphicQuailityBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (i == graphicQuaility)
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            graphicQuailityBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    public void OnClickGraphicQuailityBtn()
    {
        for (int i = 0; i < graphicQuailityBtn.Count; i++)
        {
            if (i == graphicQuaility)
            {
                graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                graphicQuailityBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            graphicQuailityBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            graphicQuailityBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }

    public void SetSelectedFrameRateBtn(BaseEventData baseEventData)
    {
        for (int i = 0; i < frameRateBtn.Count; i++)
        {
            if (baseEventData.selectedObject == frameRateBtn[i])
            {
                frameRateBtn[i].transform.Find("Image").GetComponent<Image>().color = Color.white;
                frameRateBtn[i].GetComponentInChildren<Text>().color = selectedColor;
                continue;
            }
            else if (i == frame)
            {
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = clickedColor;
            }
            else
            {
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
            }
        }
    }
    public void DeSelectedFrameRateBtn()
    {
        for (int i = 0; i < frameRateBtn.Count; i++)
        {
            frameRateBtn[i].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            if (i == frame)
            {
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = clickedColor;
            }
            else
            {
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
            }
        }
    }
    public void OnClickFrameRateBtn()
    {
        for (int i = 0; i < frameRateBtn.Count; i++)
        {
            if (i == frame)
            {
                frameRateBtn[i].GetComponentInChildren<Text>().fontSize = clickedSize;
                frameRateBtn[i].GetComponentInChildren<Text>().color = clickedColor;
                continue;
            }
            frameRateBtn[i].GetComponentInChildren<Text>().fontSize = deselectedSize;
            frameRateBtn[i].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }

    public void SetSelectedVsyncBtn(BaseEventData eventData)
    {
        if (isVsync)
        {
            if (eventData.selectedObject == vSyncBtn[0])
            {
                vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                vSyncBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                vSyncBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                vSyncBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                vSyncBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
            }
            else
            {
                vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                vSyncBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                vSyncBtn[0].GetComponentInChildren<Text>().color = clickedColor;
                vSyncBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                vSyncBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
        }
        else
        {
            if (eventData.selectedObject == vSyncBtn[1])
            {
                vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                vSyncBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                vSyncBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
                vSyncBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                vSyncBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
            else
            {
                vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                vSyncBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                vSyncBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                vSyncBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                vSyncBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            }
        }
    }
    public void DeSelectedVsyncBtn()
    {
        vSyncBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        vSyncBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        if (isVsync)
        {
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;

            foreach (var btn in frameRateBtn)
            {
                btn.GetComponent<Button>().interactable = false;
            }
            for (int i = 0; i < graphicQuailityBtn.Count; i++)
            {
                graphicQuailityBtn[i].GetComponent<Button>().navigation = graphicQuailityNavVsyncTrue[i];
            }
            for (int i = 0; i < vSyncBtn.Count; i++)
            {
                vSyncBtn[i].GetComponent<Button>().navigation = VsyncNavVsyncTrue[i];
            }
        }
        else
        {
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;

            foreach (var btn in frameRateBtn)
            {
                btn.GetComponent<Button>().interactable = true;
            }
            for (int i = 0; i < graphicQuailityBtn.Count; i++)
            {
                graphicQuailityBtn[i].GetComponent<Button>().navigation = graphicQuailityNavVsyncFalse[i];
            }
            for (int i = 0; i < vSyncBtn.Count; i++)
            {
                vSyncBtn[i].GetComponent<Button>().navigation = VsyncNavVsyncFalse[i];
            }
        }
    }
    public void OnClickVsyncBtn()
    {
        if (isVsync)
        {
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;

            foreach (var btn in frameRateBtn)
            {
                btn.GetComponent<Button>().interactable = false;
            }
            for (int i = 0; i < graphicQuailityBtn.Count; i++)
            {
                graphicQuailityBtn[i].GetComponent<Button>().navigation = graphicQuailityNavVsyncTrue[i];
            }
            for (int i = 0; i < vSyncBtn.Count; i++)
            {
                vSyncBtn[i].GetComponent<Button>().navigation = VsyncNavVsyncTrue[i];
            }
        }
        else
        {
            vSyncBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            vSyncBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            vSyncBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            vSyncBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;

            foreach (var btn in frameRateBtn)
            {
                btn.GetComponent<Button>().interactable = true;
            }
            for (int i = 0; i < graphicQuailityBtn.Count; i++)
            {
                graphicQuailityBtn[i].GetComponent<Button>().navigation = graphicQuailityNavVsyncFalse[i];
            }
            for (int i = 0; i < vSyncBtn.Count; i++)
            {
                vSyncBtn[i].GetComponent<Button>().navigation = VsyncNavVsyncFalse[i];
            }
        }
        frameRateOffPanel.SetActive(isVsync);
    }

    public void SetSelectedPostProcessingBtn(BaseEventData eventData)
    {
        if (isPostProcessing)
        {
            if (eventData.selectedObject == postProcessingBtn[0])
            {
                postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                postProcessingBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                postProcessingBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
            }
            else
            {
                postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                postProcessingBtn[0].GetComponentInChildren<Text>().color = clickedColor;
                postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                postProcessingBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
        }
        else
        {
            if (eventData.selectedObject == postProcessingBtn[1])
            {
                postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                postProcessingBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
                postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                postProcessingBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
            else
            {
                postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                postProcessingBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                postProcessingBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            }
        }
    }
    public void DeSelectedPostProcessingBtn()
    {
        postProcessingBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        postProcessingBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        if (isPostProcessing)
        {
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    public void OnClickPostProcessingBtn()
    {
        if (isPostProcessing)
        {
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            postProcessingBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            postProcessingBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            postProcessingBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            postProcessingBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }

    public void SetSelectedAmibentOcculsionBtn(BaseEventData eventData)
    {
        if (isAmbientOcclusion)
        {
            if (eventData.selectedObject == ambientOcclusionBtn[0])
            {
                ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
            }
            else
            {
                ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
                ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = clickedColor;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
        }
        else
        {
            if (eventData.selectedObject == ambientOcclusionBtn[1])
            {
                ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = Color.white;

                ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = selectedColor;
            }
            else
            {
                ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = Color.white;
                ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

                ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
                ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = selectedColor;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
                ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            }
        }
    }
    public void DeSelectedAmibentOcculsioBtn()
    {
        ambientOcclusionBtn[0].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        ambientOcclusionBtn[1].transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);

        if (isAmbientOcclusion)
        {
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    public void OnClickAmibentOcculsioBtn()
    {
        if (isAmbientOcclusion)
        {
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = deSelectedColor;
        }
        else
        {
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().fontSize = clickedSize;
            ambientOcclusionBtn[1].GetComponentInChildren<Text>().color = clickedColor;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().fontSize = deselectedSize;
            ambientOcclusionBtn[0].GetComponentInChildren<Text>().color = deSelectedColor;
        }
    }
    #endregion


    public void SetGraphicQuality(int x)
    {
        graphicQuaility = x;
    }

    public void SetVsync(bool isTrue)
    {
        isVsync = isTrue;
    }
    public void SetPostProcessing(bool isTrue)
    {
        isPostProcessing = isTrue;
    }
    public void SetAmbientOcclusion(bool isTrue)
    {
        isAmbientOcclusion = isTrue;
    }

    public void ChangeStateOnUi()
    {
        GameManager.instance.State = UiState.Option;
        State = OptionUiState.OnUi;
    }
    public void ActiveApplyUi()
    {
        acceptUi.SetActive(true);
        EventSystem.current.SetSelectedGameObject(acceptButton);
    }
    public void DeActiveApplyUi()
    {
        acceptUi.SetActive(false);
    }

    public void EscapeOption()
    {
        if ((int)State > 0)
        {
            switch (State)
            {
                case OptionUiState.OnUi:
                case OptionUiState.AcceptUi:
                    State--;
                    break;
                case OptionUiState.AcceptSaveUi:
                case OptionUiState.DontAcceptSaveUi:
                case OptionUiState.SaveUi:
                    State = OptionUiState.OnUi;
                    break;
                case OptionUiState.AcceptSaveBtnUi:
                    State--;
                    break;
            }
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
        }
    }
}