using UnityEngine;

public class GamePauseManual : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }
}
