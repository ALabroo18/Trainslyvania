using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public string nextScene;
    
    public GameObject ShopUI;
    public void LoadScene()
    {
        StartCoroutine(delayLoad());
    }

    

    public void LoadShop()
    {
        ShopUI.SetActive(true);
    }

    IEnumerator delayLoad() {
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadRoute() {
        Time.timeScale = 1;
        SceneManager.LoadScene("RouteSelect");
    }

    public void SetNextScene1()
    {
        sceneName = "Level 1";
    }

    public void SetNextScene2()
    {
        sceneName = "Level2";
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}