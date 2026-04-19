using UnityEngine;

public enum GameMode
{
    Normal,
    Infinite
}

public class ModeSelector : MonoBehaviour
{
    public static GameMode SelectedMode = GameMode.Normal;

    public void SelectNormalMode()
    {
        SelectedMode = GameMode.Normal;
        Debug.Log("Mode set to Normal");
    }

    public void SelectInfiniteMode()
    {
        SelectedMode = GameMode.Infinite;
        Debug.Log("Mode set to Infinite");
    }
}
