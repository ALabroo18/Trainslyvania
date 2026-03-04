using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public void LoadScene()
    {
        StartCoroutine(delayLoad());
    }

    IEnumerator delayLoad() {
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadRoute() {
        SceneManager.LoadScene("RouteSelect");
    }
}