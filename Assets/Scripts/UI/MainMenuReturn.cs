using System;
using CoreModule.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuReturn : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            InputActionProvider.ClearEvents();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
