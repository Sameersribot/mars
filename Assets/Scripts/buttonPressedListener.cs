using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonPressedListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button btn;
    public bool usingNox;
    public void OnPointerDown(PointerEventData eventData)
    {
        usingNox = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        usingNox = false;
    }
    public bool getNoxValue()
    {
        return usingNox;
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    // Start is called before the first frame update
}
