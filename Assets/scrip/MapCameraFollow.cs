using UnityEngine;

public class MapCameraFollow : MonoBehaviour
{
    public Transform target;   // Player
    public float height = 80f; // tinggi kamera di atas player
    public bool rotateWithPlayer = true;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 pos = target.position;
        pos.y += height;
        transform.position = pos;

        if (rotateWithPlayer)
        {
            // map ikut arah hadap player
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            // map selalu "north up"
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
