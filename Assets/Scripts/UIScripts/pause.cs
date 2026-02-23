using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    public GameObject pauseClick;
    public GameObject resumeClick;
    public GameObject homeClick;

    public void pauseGame()
    {
        //pauses game activity and turns resume button on
        Debug.Log("pauseGame is running");
        Time.timeScale = 0f;
        pauseClick.SetActive(false);
        resumeClick.SetActive(true);
        homeClick.SetActive(true);
    }

    public void resumeGame()
    {
        //resumes game activity, turning resume button off
        Time.timeScale = 1f;
        pauseClick.SetActive(true);
        resumeClick.SetActive(false);
        homeClick.SetActive(false);

    }

    public void routeSelect() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RouteSelect");
    }

}
