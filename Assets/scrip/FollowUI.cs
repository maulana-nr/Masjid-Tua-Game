using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public Transform target;    
    public Vector3 offset;      
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);

        if (screenPos.z < 0)        
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = screenPos;
    }
}
