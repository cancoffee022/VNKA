using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

public class WeaponEnchant : MonoBehaviour,IMoveHandler,IDeselectHandler
{
    public bool isClicked;
    public void OnMove(AxisEventData axisEventData)
    {
        if (isClicked)
        {
            if (axisEventData.moveDir == MoveDirection.Down)
            {
                GetComponent<ScrollSnap>().NextScreen();
            }
            else if(axisEventData.moveDir == MoveDirection.Up)
            {
                GetComponent<ScrollSnap>().PreviousScreen();
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isClicked = false;
    }
}
