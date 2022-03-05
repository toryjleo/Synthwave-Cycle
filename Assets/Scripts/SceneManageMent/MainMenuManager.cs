using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>Bullet</c> A Unity Component which will turn on/off the major menus in this scene.</summary>
public class MainMenuManager : MonoBehaviour
{
    public GameObject scoreScreen;
    public GameObject mainMenu;

    void Awake()
    {
        if (PlayerDataObject.wasJustInGame) 
        {
            TurnOnScoreScreen();
        }
        else 
        {
            TurnOnMainMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreScreen.activeInHierarchy && PlayerInputsToMainMenu()) 
        {
            TurnOnMainMenu();
        }
    }

    /// <summary>Sets only the main menu to active.</summary>
    private void TurnOnMainMenu() 
    {
        mainMenu.SetActive(true);
        scoreScreen.SetActive(false);
    }

    /// <summary>Sets only the score screen to active.</summary>
    private void TurnOnScoreScreen() 
    {
        mainMenu.SetActive(false);
        scoreScreen.SetActive(true);
    }

    /// <summary>Will return true if the player presses a key that will return to the main nmenu.</summary>
    /// <returns>True or false.</returns>
    private bool PlayerInputsToMainMenu() 
    {
        return Input.GetKeyDown(KeyCode.Space) ||
               Input.GetKeyDown(KeyCode.Mouse0);
    }
}
