using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance;
    public FloatingJoystick joystick;

    private void Awake()
    {
        Instance = this;
    }
}
