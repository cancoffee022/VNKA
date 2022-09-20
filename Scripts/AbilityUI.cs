using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AbilityUIState
{
    OFFUi,
    SnapWeapons,
    WeaponSelected,
    WeaponCharaTypeSelect,
    WeaponEnchantSnapSelect,
    WeaponLevelUpSelect
}

public class AbilityUI : MonoBehaviour
{
    public GameObject abilityMenu;
    public GameObject abilityMenuSnap;
    public GameObject weaponEnhanceTypesButtonsPanel;
    public GameObject weaponEnchantLevel;
    public GameObject levelUpPanel;
    public Transform weaponEnhanceTypesButtons;

    [SerializeField]
    private AbilityUIState state;
    public AbilityUIState State
    {
        get => state;
        set
        {
            AbilityUIState prevState = state;
            if (value != prevState)
            {
                UpdateAbilityUi(value);
            }
            state = value;
        }
    }

    public bool isAbilityOn => State != AbilityUIState.OFFUi;

    public List<GameObject> weaponStatus;
    public List<GameObject> weaponCharaButtons;
    public List<List<GameObject>> weaponEnchantLevelPanels = new List<List<GameObject>>();
    public List<WeaponChara> weaponCharas;

    public int OnSelectWeapon;

    public Color activeColor;
    public Color inActiveColor;

    [SerializeField]
    private GameObject selectedWeapon;
    [SerializeField]
    public int selectedWeaponNumber;
    public GameObject SelectedWeapon
    {
        get => selectedWeapon;
        set
        {
            foreach (GameObject weapons in weaponStatus)
            {
                if (value == weapons)
                {
                    selectedWeapon = value;
                }
            }
        }
    }

    public Sprite selectedCharaInButton;
    public Sprite deselectedCharaInButton;

    public List<List<LevelUpMaterial>> enchantMaterial = new List<List<LevelUpMaterial>>();
    private Navigation nav;
    public List<Navigation> prevNavs;

    private void Start()
    {
        for (int i = 0; i < weaponStatus.Count; i++)
        {
            int idx = i;
            weaponStatus[i].GetComponent<Button>().onClick.AddListener(() => { ChangeSelectedWeaponClick(idx); });

            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.onClick.
                AddListener(() => { ChangeSelectedWeaponClick(idx); });

            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.
                GetComponent<Button>().onClick.AddListener(() => { ChangeSelectedWeaponClick(idx); });

            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponLevelUpButton.
                GetComponent<Button>().onClick.AddListener(() => { ChangeSelectedWeaponClick(idx); });
        }

        foreach (var weapon in weaponStatus)
        {
            weapon.GetComponent<Button>().onClick.AddListener(SaveSelecteButton);
            weapon.GetComponent<Button>().onClick.AddListener(ChangeStateWeaponSelected);

            weapon.GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.onClick.AddListener(WeaponCharacButton);
            weapon.GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.onClick.AddListener(ChangeStateWeaponCharaTypeSelect);

            Button weaponEnchantSnap = weapon.GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>();
            weaponEnchantSnap.onClick.AddListener(ChangeStateWeaponEnchantSnapSelect);
            weaponEnchantSnap.onClick.AddListener(WeaponEnchantSnapButton);
            prevNavs.Add(weaponEnchantSnap.navigation);

            weapon.GetComponent<WeaponStatusButton>().weaponLevelUpButton.onClick.AddListener(ChangeWeaponLevelUpSelect);
            weapon.GetComponent<WeaponStatusButton>().weaponLevelUpButton.onClick.AddListener(WeaponLevelUpButton);
        }

        for (int i = 0; i < weaponCharas.Count; i++)
        {
            weaponStatus[i].GetComponent<WeaponStatusButton>().Level = weaponCharas[i].Level;
            List<GameObject> games = new List<GameObject>();
            for (int j = 0; j < weaponCharas[i].levelUpEffect.Length; j++)
            {
                games.Add(Instantiate(weaponEnchantLevel, weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnapList));
                games[j].GetComponent<WeaponEnchantLevel>().levelText.text = (j + 1).ToString();
                games[j].GetComponent<WeaponEnchantLevel>().detailText.text = weaponCharas[i].levelUpEffect[j];
            }
            weaponEnchantLevelPanels.Add(games);
        }
        foreach (var weaponchara in weaponStatus)
        {
            weaponchara.GetComponent<Button>().onClick.AddListener(UpdateCharaSelectButton);
        }
        WeaponEnchantLevelPanelUpdate();
    }
    public void UpdateAbilityUi(AbilityUIState state)
    {
        switch (state)
        {
            case AbilityUIState.OFFUi:
                InActiveAllButtons();
                break;
            case AbilityUIState.SnapWeapons:
                ActiveAllButtons();
                EventSystem.current.SetSelectedGameObject(weaponStatus[selectedWeaponNumber]);
                levelUpPanel.SetActive(false);
                break;
            case AbilityUIState.WeaponSelected:
                InActiveButtonsPanel();
                UpdateSelectedWeapon();
                EventSystem.current.SetSelectedGameObject(weaponStatus[selectedWeaponNumber].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.gameObject);
                for (int i = 0; i < weaponStatus.Count; i++)
                {
                    weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>().navigation = prevNavs[i];
                }
                levelUpPanel.SetActive(false);
                break;
            case AbilityUIState.WeaponCharaTypeSelect:
                EventSystem.current.SetSelectedGameObject(weaponCharaButtons[0]);
                UpdateCharaSelectButton();
                break;
            case AbilityUIState.WeaponEnchantSnapSelect:
                break;
            case AbilityUIState.WeaponLevelUpSelect:
                break;
        }
    }
    public void EscapeAbilityUI()
    {
        if ((int)State > 0)
        {
            switch ((int)State)
            {
                case 1:
                case 2:
                    State--;
                    break;
                case 3:
                case 4:
                case 5:
                    State = (AbilityUIState)2;
                    break;
            }
        }
        GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[7]);
    }
    /*public void SetMaterial()
    {
        for(int i = 0; i < weaponCharas.Count; i++)
        {
            int j = 0;
            switch (j)
            {
                case 0:
                    enchantMaterial[i].Add(weaponCharas[i].materials[0]);
                break;
            }
        }
    }
*/
    public void InActiveAllButtons()
    {
        foreach (var weapons in weaponStatus)
        {
            weapons.GetComponent<Button>().interactable = false;
            weapons.GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Image>().color = inActiveColor;
            weapons.GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>().interactable = false;
            weapons.GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.interactable = false;
            weapons.GetComponent<WeaponStatusButton>().weaponLevelUpButton.interactable = false;
        }
    }
    public void ActiveAllButtons()
    {
        foreach (var weapons in weaponStatus)
        {
            if (!weapons.GetComponent<WeaponStatusButton>().weaponLock.activeSelf)
            {
                weapons.GetComponent<Button>().interactable = true;
                weapons.GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Image>().color = activeColor;
                weapons.GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>().interactable = true;
                weapons.GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.interactable = true;
                weapons.GetComponent<WeaponStatusButton>().weaponLevelUpButton.interactable = true;
            }
        }
        if (selectedWeapon != null)
        {
            //EventSystem.current.SetSelectedGameObject(selectedWeapon);
        }
    }
    public void UpdateSelectedWeapon()
    {
        SelectedWeapon = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < weaponStatus.Count; i++)
        {
            if (i == selectedWeaponNumber)
            {
                weaponStatus[i].GetComponent<Button>().interactable = false;
                weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Image>().color = activeColor;
                weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>().interactable = true;
                weaponStatus[i].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.interactable = true;
                weaponStatus[i].GetComponent<WeaponStatusButton>().weaponLevelUpButton.interactable = true;
                selectedWeaponNumber = i;
                continue;
            }
            weaponStatus[i].GetComponent<Button>().interactable = false;
            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Image>().color = inActiveColor;
            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponEnchantSnap.GetComponent<Button>().interactable = false;
            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.interactable = false;
            weaponStatus[i].GetComponent<WeaponStatusButton>().weaponLevelUpButton.interactable = false;
        }
        EventSystem.current.SetSelectedGameObject(weaponStatus[selectedWeaponNumber].gameObject);
    }
    public void WeaponCharacButton()
    {
        Button clickObject = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        weaponEnhanceTypesButtonsPanel.transform.Find("WeaponIconImage").GetComponent<Image>().sprite
            = weaponStatus[selectedWeaponNumber].transform.Find("RawImage").GetComponent<Image>().sprite;
        weaponEnhanceTypesButtonsPanel.transform.Find("WeaponTypeText").GetComponent<Text>().text
            = weaponCharas[selectedWeaponNumber].weaponType;
        string str = (weaponCharas[selectedWeaponNumber].Level > 10) ? " " + weaponCharas[selectedWeaponNumber].ToString() : " 0" + weaponCharas[selectedWeaponNumber].Level.ToString();
        weaponEnhanceTypesButtonsPanel.transform.Find("WeaponLevelText").GetComponent<Text>().text
            = "Level" + str;

        weaponEnhanceTypesButtonsPanel.SetActive(true);
        for (int i = 0; i < weaponCharas.Count; i++)
        {
            if (clickObject == weaponStatus[i].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton)
            {
                for (int j = 0; j < 3; j++)
                {
                    weaponCharaButtons[j].GetComponentInChildren<Text>().text = weaponCharas[i].Text[j];
                    weaponCharaButtons[j].transform.Find("ButtonImage").GetComponent<Image>().sprite = weaponCharas[i].charaImage[j];
                    int temp = j;
                    weaponEnhanceTypesButtons.GetChild(j).GetComponent<Button>().onClick.AddListener(() => { weaponCharas[selectedWeaponNumber].Charaidx = temp; });
                }
            }
        }
        InActiveAllButtons();
    }

    public void WeaponEnchantLevelPanelUpdate()
    {
        for (int i = 0; i < weaponCharas.Count; i++)
        {
            for (int j = 0; j < weaponCharas[i].Level; j++)
            {
                weaponEnchantLevelPanels[i][j].GetComponent<WeaponEnchantLevel>().panel.SetActive(false);
            }
        }
    }
    public void WeaponLevelUpButton()
    {
        int materialNum = weaponCharas[selectedWeaponNumber].Level;
        string stritem = null;
        string strgold = weaponCharas[selectedWeaponNumber].materials[materialNum].gold.ToString();
        levelUpPanel.SetActive(true);
        if (weaponCharas[selectedWeaponNumber].materials[materialNum].item.Count == 0)
        {
            stritem = "없음";
        }
        for (int i = 0; i < weaponCharas[selectedWeaponNumber].materials[materialNum].item.Count; i++)
        {
            stritem += weaponCharas[selectedWeaponNumber].materials[materialNum].item[i].itemName + " * "
                + weaponCharas[selectedWeaponNumber].materials[materialNum].itemAmount[i];
        }
        levelUpPanel.transform.GetChild(0).GetComponent<Text>().text = "필요아이템은 " + stritem + "\n필요 골드는 " + strgold;
        levelUpPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(LevelUpButton);
        levelUpPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ChangeStateWeaponSelected);
        EventSystem.current.SetSelectedGameObject(levelUpPanel.transform.GetChild(1).gameObject);
    }

    public void WeaponEnchantSnapButton()
    {
        WeaponEnchant weaponEnchant = EventSystem.current.currentSelectedGameObject.GetComponent<WeaponEnchant>();
        nav = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation;
        nav.mode = Navigation.Mode.None;
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().navigation = nav;
        weaponEnchant.isClicked = true;
        weaponStatus[selectedWeaponNumber].GetComponent<WeaponStatusButton>().weaponLevelUpButton.interactable = false;
        weaponStatus[selectedWeaponNumber].GetComponent<WeaponStatusButton>().weaponCharacterTypeButton.interactable = false;
    }
    public void LevelUpButton()
    {
        if (weaponCharas[selectedWeaponNumber].materials[weaponCharas[selectedWeaponNumber].Level].Confirm())
        {
            weaponCharas[selectedWeaponNumber].Level++;
            weaponStatus[selectedWeaponNumber].GetComponent<WeaponStatusButton>().Level = weaponCharas[selectedWeaponNumber].Level;
            weaponCharas[selectedWeaponNumber].materials[weaponCharas[selectedWeaponNumber].Level].RemoveItems();
        }
        else
        {
            Debug.Log("재료 없음");
        }

        WeaponEnchantLevelPanelUpdate();
        levelUpPanel.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
    }
    public void SaveSelecteButton()
    {
        selectedWeapon = EventSystem.current.currentSelectedGameObject;
    }
    public void ChangeSelectedWeaponClick(int x)
    {
        selectedWeaponNumber = x;
    }
    public void InActiveButtonsPanel()
    {
        weaponEnhanceTypesButtonsPanel.SetActive(false);
    }
    public void UpdateCharaSelectButton()
    {

        for (int i = 0; i < 3; i++)
        {
            if (i == weaponCharas[selectedWeaponNumber].Charaidx)
            {
                weaponCharaButtons[i].transform.GetChild(2).GetComponent<Image>().sprite = selectedCharaInButton;
                continue;
            }
            weaponCharaButtons[i].transform.GetChild(2).GetComponent<Image>().sprite = deselectedCharaInButton;
        }
    }
    #region ChangeState
    public void ChangeStateSnapWeapon()
    {
        State = AbilityUIState.SnapWeapons;
    }
    public void ChangeStateWeaponSelected()
    {
        State = AbilityUIState.WeaponSelected;
    }
    public void ChangeStateWeaponCharaTypeSelect()
    {
        State = AbilityUIState.WeaponCharaTypeSelect;
    }
    public void ChangeStateWeaponEnchantSnapSelect()
    {
        State = AbilityUIState.WeaponEnchantSnapSelect;
    }
    public void ChangeWeaponLevelUpSelect()
    {
        State = AbilityUIState.WeaponLevelUpSelect;
    }
    #endregion
}
[System.Serializable]
public class WeaponChara
{
    public string weaponType;
    [SerializeField]
    private string[] text = new string[3];
    public List<Sprite> charaImage;
    [SerializeField]
    private int charaidx;
    public int Charaidx
    {
        get => charaidx;
        set
        {
            int prevInt = charaidx;
            charaidx = value;
            if (value != prevInt)
            {
                GameManager.instance.weaponAbility.UpdateCharaSelectButton();
            }
        }
    }
    public string[] Text => text;

    public int level = 1;
    public int Level
    {
        get => level;
        set
        {
            if (value <= levelUpEffect.Length)
            {
                if (value > 0)
                {
                    Debug.Log("Level:" + value);
                    effects[value - 1].GiveEffect(GameManager.instance.weaponAbility.selectedWeaponNumber);
                }
                level = value;
            }
            else
                Debug.Log("최대 레벨입니다");
        }
    }
    public List<WeaponLevelEffect> effects;

    public string[] levelUpEffect;

    public List<LevelUpMaterial> materials;

}