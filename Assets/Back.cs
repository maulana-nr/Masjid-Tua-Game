using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelOptions;

    public void OpenOptions()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void BackToMainMenu()
    {
        panelOptions.SetActive(false);
        panelMainMenu.SetActive(true);
    }
}
