using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TriggerMasjid : MonoBehaviour
{
    public EndingCutscene endingCutscene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endingCutscene.StartEnding();
            gameObject.SetActive(false); // biar trigger cuma sekali
        }
    }
}
