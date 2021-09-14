using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventExtensions
{
    static readonly Dictionary<KeyCode, bool> keyPressed = new Dictionary<KeyCode, bool>();

    static readonly Dictionary<int, bool> mouseButtonPressed = new Dictionary<int, bool>();

    public static Action onMouseButtonDown;

    public static bool GetKey(this Event currentEvent,KeyCode keyCode)
    {
        if (currentEvent.keyCode == keyCode)
        {
            keyPressed[keyCode] = currentEvent.type == EventType.KeyDown;
        }

        return keyPressed.ContainsKey(keyCode) && keyPressed[keyCode];
    }

    public static bool GetMouseButton(this Event currentEvent, int button = 0)
    {
        if (currentEvent.isMouse && currentEvent.button == button)
        {
            if (mouseButtonPressed.ContainsKey(button) && mouseButtonPressed[button] && currentEvent.type == EventType.MouseDown)
            {
                onMouseButtonDown?.Invoke();
            }
            mouseButtonPressed[button] = currentEvent.type != EventType.MouseUp;
        }

        return mouseButtonPressed.ContainsKey(button) && mouseButtonPressed[button];
    }

    public static bool GetMouseButtonDown(this Event currentEvent, int button = 0)
    {
        return currentEvent.type == EventType.MouseDown && currentEvent.button == button;
    }
}
