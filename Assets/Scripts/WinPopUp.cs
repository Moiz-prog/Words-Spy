using UnityEngine;

public class WinPopUp : MonoBehaviour
{
    public GameObject winPopup;

    void Start()
    {
        winPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnBoardCompleted += ShowWinPopup;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardCompleted -= ShowWinPopup;
    }

    private void ShowWinPopup()
    {
        winPopup.SetActive(true);
    }

    public void LoadNextLevel()
    {
        GameEvents.LoadNextLevelMethod();
    }
}
