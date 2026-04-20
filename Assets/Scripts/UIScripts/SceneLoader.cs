using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public string nextScene;
    public string infiniteSceneName = "InfiniteLevel1";

    public GameObject ShopUI;
    public void LoadScene()
    {
        StartCoroutine(delayLoad(sceneName));
    }

    

    public void LoadShop()
    {
        ShopUI.SetActive(true);
    }

    IEnumerator delayLoad(string scene) {
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(scene);
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

    public void SetNextScene3()
    {
        sceneName = "Level 3";
    }

    public void SetNextScene4()
    {
        sceneName = "Level 4";
    }

    public void SetNextScene5()
    {
        sceneName = "Level 5";
    }

    public void LoadLevel1()
    {
        Time.timeScale = 1;

        switch (ModeSelector.SelectedMode)
        {
            case GameMode.Normal:
                StartCoroutine(delayLoad("Level 1"));
                break;

            case GameMode.Infinite:
                StartCoroutine(delayLoad("InfiniteLevel1"));
                break;

            default:
                StartCoroutine(delayLoad("Level 1"));
                break;
        }
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}