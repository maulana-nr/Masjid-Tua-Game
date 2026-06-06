using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitLogic : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelQuit;

    
    public void OpenQuitPanel()
    {
        panelMainMenu.SetActive(false);
        panelQuit.SetActive(true);
    }

    public void CancelQuit()
    {
        panelQuit.SetActive(false);
        panelMainMenu.SetActive(true);
    }

   
    public void ConfirmQuit()
    {
        Application.Quit();
    }
}
