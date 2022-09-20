using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class LobbyUI : MonoBehaviour
{
    public GameObject PressAnyKey;

    public GameObject MenuSelection;
    public GameObject AcceptUi;
    public GameObject GameStartSelection;
    public GameObject EndGameMenu;
    public GameObject LobbyOption;

    public GameObject lobbyScreen;

    public GameObject lobbyPrefab;
    public GameObject tempLobbyPrefab;

    public Material[] skyBoxes;

    public GameObject gamestartBtn;
    public GameObject firststartBtn;
    public GameObject firstSelectConfirmMenu;
    public GameObject FirstStartConfirmBtn;
    public GameObject continueStartBtn;
    public GameObject endGameBtn;
    public GameObject EndGameCancelBtn;

    public GameObject gameOptionsBtn;
    public GameObject applyBtn;
    public List<GameObject> screenModeBtn;


    public GameObject saveFilesDarkBG;
    public GameObject SaveFilesMenu;
    public GameObject saveSnap;

    public GameObject lastselect;

    public Text resolutionSizeText;

    public int resolutionIdx;
    public int screenModeIdx;
    FullScreenMode screenMode;
    public int prevScreenModeIdx;
    public int prevResolutionIdx;
    private int resolutionWidth;
    private int resolutionHeight;
    private int clickedSize;
    private int deselectedSize;
    private Color selectedColor;
    private Color deSelectedColor;
    private Color clickedColor;
    public Coroutine expoLobby;

    public bool isPressAny;

    void Awake()
    {
        tempLobbyPrefab = GameObject.Find("LobbyPrefab");
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.isLobby = true;
        LeanTween.scale(PressAnyKey, new Vector2(1.01f, 1.01f), 1.0f).setEaseInOutSine().setLoopPingPong(-1);
        LeanTween.alphaText(PressAnyKey.GetComponent<RectTransform>(), 0, 1).setDelay(1f).setEaseInOutSine().setLoopPingPong(-1);
        SetResolution(resolutionIdx);
        ShowResolutionText();
        selectedColor = GameManager.instance.option.selectedColor;
        deSelectedColor = GameManager.instance.option.deSelectedColor;
        clickedColor = GameManager.instance.option.clickedColor;
        clickedSize = GameManager.instance.option.clickedSize;
        deselectedSize = GameManager.instance.option.deselectedSize;
        GameManager.instance.option.resolutionIdx = resolutionIdx;
        OnClickScreenModeBtn();
        expoLobby = StartCoroutine(exposureSize());
    }

    public IEnumerator exposureSize()
    {
        Debug.Log("expo시작");
        RenderSettings.skybox.SetFloat("_Exposure", 0.4f);
        float time = 15f;
        float rotExposure;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            rotExposure = Mathf.Lerp(0.4f, 0.68f, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Exposure", rotExposure);
            yield return null;
        }
        RenderSettings.skybox.SetFloat("_Exposure", 0.68f);

        elapsedTime = 0;
        while (elapsedTime < time)
        {
            rotExposure = Mathf.Lerp(0.68f, 0.4f, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Exposure", rotExposure);
            yield return null;
        }
        RenderSettings.skybox.SetFloat("_Exposure", 0.4f);

        if (GameManager.instance.isLobby)
        {
            StartCoroutine(exposureSize());
        }
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }

        if (!isPressAny)
        {
            if (Input.anyKey)
            {
                PressAnyKey.SetActive(false);
                MenuSelection.SetActive(true);
                LeanTween.cancel(PressAnyKey);
                PressAnyKey.transform.localScale = new Vector3(1, 1, 1);
                //PressAnyKey
                isPressAny = true;
                GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
                EventSystem.current.SetSelectedGameObject(gamestartBtn);
            }
        }

        if (LobbyOption.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (AcceptUi.activeSelf)
                {
                    AcceptUi.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(applyBtn);
                }
                else
                {
                    OnClickScreenModeBtn();
                    foreach (GameObject game in screenModeBtn)
                    {
                        game.transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    }
                    LobbyOption.SetActive(false);
                    MenuSelection.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(gameOptionsBtn);
                }
            }
        }


        if (GameStartSelection.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SaveFilesMenu.activeSelf)
                {
                    GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);

                    GameManager.instance.DoF.active = false;
                    SaveFilesMenu.SetActive(false);
                    LeanTween.cancelAll();
                    for(int i = 0; i < GameManager.instance.SaveSlotBtns.Length; i++)
                    {
                        GameManager.instance.SaveSlotBtns[i].transform.GetChild(8).gameObject.SetActive(false);
                    }

                    EventSystem.current.SetSelectedGameObject(continueStartBtn);
                }
                else
                {
                    GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
                    Debug.Log("엄...");

                    LeanTween.cancel(continueStartBtn.transform.GetChild(1).gameObject);
                    continueStartBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
                    continueStartBtn.transform.GetChild(1).gameObject.SetActive(false);

                    GameStartSelection.SetActive(false);
                    MenuSelection.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(gamestartBtn);
                }
            }
        }
        if (EndGameMenu.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeanTween.cancel(endGameBtn.transform.GetChild(1).gameObject);
                LeanTween.cancel(EndGameCancelBtn.transform.GetChild(1).gameObject);
                endGameBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
                EndGameCancelBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
                endGameBtn.transform.GetChild(1).gameObject.SetActive(false);
                EndGameCancelBtn.transform.GetChild(1).gameObject.SetActive(false);
                GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
                EndGameMenu.SetActive(false);
                EventSystem.current.SetSelectedGameObject(gamestartBtn);
            }
        }

        /*if (SaveFilesMenu.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex() > 1 && (Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.DownArrow)))
            {
                saveSnap.GetComponent<UnityEngine.UI.Extensions.ScrollSnap>().NextScreen();
            }
            if (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex() > 1 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                saveSnap.GetComponent<UnityEngine.UI.Extensions.ScrollSnap>().PreviousScreen();
            }
        }*/
    }



    //게임 시작 버튼
    public void OnclickGame()
    {
        MenuSelection.SetActive(false);
        GameStartSelection.SetActive(true);
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        EventSystem.current.SetSelectedGameObject(firststartBtn);
    }

    //옵션 버튼
    public void OnclickOption()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);

        MenuSelection.SetActive(false);
        LobbyOption.SetActive(true);
        EventSystem.current.SetSelectedGameObject(LobbyOption.GetComponentInChildren<Button>().gameObject);


    }



    //게임 종료 버튼
    public void OnclickEndGameMenu()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        EndGameMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(EndGameCancelBtn);
    }

    public void OnclickEndGameBtn()
    {
        Application.Quit();
    }
    public void OnclickCancelEndGame()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        EndGameMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(gamestartBtn);
    }

    public void ShowResolutionText()
    {
        resolutionSizeText.text = resolutionWidth + " X " + resolutionHeight;
    }

    public void SetResolution(int idx)
    {
        switch (idx)
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

    public void SetScreenMode(int idx)
    {
        switch (idx)
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
    }
    public void OnclickResRightBtn()
    {
        if (resolutionIdx < 7)
        {
            resolutionIdx++;
        }
        SetResolution(resolutionIdx);
        ShowResolutionText();
    }

    public void OnClickResLeftBtn()
    {
        if (resolutionIdx > 0)
        {
            resolutionIdx--;
        }
        SetResolution(resolutionIdx);
        ShowResolutionText();
    }


    #region ScreenModeBtn
    public void SetFullScreen(int _screenIdx)
    {
        screenModeIdx = _screenIdx;
    }
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
    #endregion

    public void OnclickApplyBtn()
    {
        if (prevResolutionIdx != resolutionIdx || prevScreenModeIdx != screenModeIdx)
        {
            AcceptUi.SetActive(true);
            EventSystem.current.SetSelectedGameObject(AcceptUi.transform.Find("AcceptButton").gameObject);

            AcceptUi.transform.Find("AcceptButton").GetComponent<Button>().onClick.AddListener(OnClickAcceptChangeBtn);
            AcceptUi.transform.Find("DenyButton").GetComponent<Button>().onClick.AddListener(OnClickDenyChangeBtn);
        }
    }

    public void OnClickAcceptChangeBtn()
    {
        OptionUI option = GameManager.instance.option;
        prevResolutionIdx = resolutionIdx;
        prevScreenModeIdx = screenModeIdx;
        SetScreenMode(screenModeIdx);

        Screen.SetResolution(resolutionWidth, resolutionHeight, screenMode);
        option.screenModeIdx = GameManager.instance.option.prevScreenModeIdx = screenModeIdx;
        option.resolutionIdx = GameManager.instance.option.prevResolutionIdx = resolutionIdx;

        option.SetSelectedImage();
        AcceptUi.transform.Find("AcceptButton").GetComponent<Button>().onClick.RemoveAllListeners();
        AcceptUi.transform.Find("DenyButton").GetComponent<Button>().onClick.RemoveAllListeners();
        AcceptUi.SetActive(false);
        EventSystem.current.SetSelectedGameObject(applyBtn);
    }
    public void OnClickDenyChangeBtn()
    {
        resolutionIdx = prevResolutionIdx;
        screenModeIdx = prevScreenModeIdx;
        SetResolution(resolutionIdx);
        ShowResolutionText();
        OnClickScreenModeBtn();
        AcceptUi.SetActive(false);
        EventSystem.current.SetSelectedGameObject(applyBtn);
    }



    //처음부터 버튼
    public void OnclickFirstStart()
    {
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[4]);
        firstSelectConfirmMenu.SetActive(true);

        string path = (Application.persistentDataPath + "/playData.json");
        File.Delete(path);
    }

    //이어하기 버튼
    public void OnclickContinueStart()
    {

    }
    
}
