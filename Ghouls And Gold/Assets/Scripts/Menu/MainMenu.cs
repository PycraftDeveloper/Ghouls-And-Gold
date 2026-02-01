using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject settingsMenu;

    [Header("Buttons")]
    [SerializeField] private GameObject playButton;

    [SerializeField] private GameObject backButton;

    [Header("Audio Sliders")]
    [SerializeField] private Slider music;

    [SerializeField] private Slider sfx;

    private void Start()
    {
        if (mainMenu == null)
            mainMenu = GameObject.Find("Main Menu");

        if (settingsMenu == null)
            settingsMenu = GameObject.Find("Settings Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Game Scene");
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton);
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

    private void FixedUpdate()
    {
        if (settingsMenu.activeInHierarchy)
        {
            Registry.Music_Volume = music.value;
            Registry.SFX_Volume = sfx.value;
        }
    }
}