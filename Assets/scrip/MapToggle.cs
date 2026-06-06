using UnityEngine;

public class MapToggle : MonoBehaviour
{
    public GameObject mapPanel;
    public Camera mapCamera;
    public bool freezePlayerWhenOpen = true;

    private bool isOpen = false;

    void Start()
    {
        mapPanel.SetActive(false);
        mapCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isOpen = !isOpen;
            mapPanel.SetActive(isOpen);
            mapCamera.enabled = isOpen;

            if (freezePlayerWhenOpen)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                var cc  = player.GetComponent<CharacterController>();
                var mov = player.GetComponent<PlayerController>();

                if (cc)  cc.enabled  = !isOpen;
                if (mov) mov.enabled = !isOpen;
            }
        }
    }
}
