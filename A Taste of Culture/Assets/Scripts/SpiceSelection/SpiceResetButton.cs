using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiceResetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.visible = true;
        Debug.Log("Cursor Visible");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.visible = false;
        Debug.Log("Cursor not visible");
    }
}
