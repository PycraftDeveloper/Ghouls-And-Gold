using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    private PlayerInput inputActions;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject resumeButton;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject retryButton;

    [Header("Win Screen")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject winButton;

    private void Start()
    {
        inputActions = new PlayerInput();

        inputActions.Player.Pause.performed += OpenPauseMenu;
        inputActions.Player.Pause.canceled += OpenPauseMenu;
    }

    void OpenPauseMenu(InputAction.CallbackContext context)
    {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(retryButton);
    }

    void RetryLevel()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(winButton);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
