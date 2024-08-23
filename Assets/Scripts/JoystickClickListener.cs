using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JoystickClickListener : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onJoystickClick;  // Event triggered on joystick click or touch

    // This method is called when the user touches or clicks on the joystick
    public void OnPointerDown(PointerEventData eventData)
    {
        // Invoke the click event
        onJoystickClick.Invoke();
    }
}


