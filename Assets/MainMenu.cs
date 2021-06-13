using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu, optionsMenu, creditsMenu;
    public GameObject menuFirstButton, optionsFirstButton, optionsClosedButton, creditsFirstButton, creditsBackButton;
    

    public void OpenOptions()
    {
        optionsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(optionsClosedButton);


    }

    public void OpenCredits()
    {
        creditsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(creditsFirstButton);
    }

    public void CloseCredits()
    {
        creditsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(creditsBackButton);
    }
}
