#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    //There's nothing for main menu right now but let's put a button for loading to next scene

    [SerializeField] private Button goToGameplayScene;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
    }

    private void GotoGameplayScene()
    {
        
    }
}