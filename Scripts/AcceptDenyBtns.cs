using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AcceptDenyBtns : MonoBehaviour,ISelectHandler,IDeselectHandler,IPointerEnterHandler,IMoveHandler
{
    RectTransform arrowIcon;
    Text text;
    float arrowPosX;
    void Awake()
    {
        arrowIcon = gameObject.transform.GetChild(1).GetComponent<RectTransform>();
        text = gameObject.transform.GetChild(0).GetComponent<Text>();
        arrowPosX = arrowIcon.anchoredPosition.x;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        arrowIcon.gameObject.SetActive(false);
        LeanTween.cancel(arrowIcon);
        text.color = new Color(255 / 255, 255 / 255, 255 / 255);
        arrowIcon.anchoredPosition = new Vector2(arrowPosX, arrowIcon.anchoredPosition.y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        arrowIcon.gameObject.SetActive(true);
        text.color = new Color(255 / 255, 255 / 255, 0 / 255);
        LeanTween.moveX(arrowIcon, arrowPosX + 8, 0.5f).setLoopPingPong(-1).setEase(LeanTweenType.easeInOutSine).setIgnoreTimeScale(true);
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Up)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }
        else if (eventData.moveDir == MoveDirection.Down)
        {
            GameManager.instance.SC.Effects[0].PlayOneShot(GameManager.instance.SC.uiEffectSounds[8]);
        }
    }
}
