using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILogic : MonoBehaviour
{
    public GameObject PanelMainMenu;
    public GameObject PanelOptions;

    public void NavigasiOptions()
    {
        PanelMainMenu.SetActive(false);
        PanelOptions.SetActive(true);
    }

    public void Navigasiscene_awal()
    {
        SceneManager.LoadScene("Loading");
    }
   
}
