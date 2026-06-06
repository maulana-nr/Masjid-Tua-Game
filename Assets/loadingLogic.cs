using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadingLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneAfterDeley());
        
    }

    IEnumerator LoadSceneAfterDeley()
    {
        float delay = Random.Range(3f, 5f);
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("scene_awal 1");
    }
}
