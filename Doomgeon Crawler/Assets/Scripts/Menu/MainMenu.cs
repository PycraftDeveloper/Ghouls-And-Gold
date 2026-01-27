using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject settingsButton;

    void Start()
    {
        if (mainMenu == null)
            mainMenu = GameObject.Find("Main Menu");

        if (settingsMenu == null)
            settingsMenu = GameObject.Find("Settings Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsButton);
    }

    public void CloseSettingsMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
