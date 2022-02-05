using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void TurnOnMainMenu() 
    {
        mainMenu.SetActive(true);
        scoreScreen.SetActive(false);
    }

    private void TurnOnScoreScreen() 
    {
        mainMenu.SetActive(false);
        scoreScreen.SetActive(true);
    }

    private bool PlayerInputsToMainMenu() 
    {
        return Input.GetKeyDown(KeyCode.Space) ||
               Input.GetKeyDown(KeyCode.Mouse0);
    }
}
