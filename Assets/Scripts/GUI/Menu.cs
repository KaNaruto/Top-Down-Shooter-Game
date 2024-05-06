using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [FormerlySerializedAs("inCredits")] public bool inCreditsMenu;
    [SerializeField] GameObject mainMenuHolder;
    [SerializeField] GameObject optionsMenuHolder;
    [SerializeField] private GameObject creditsMenuHolder;

    [SerializeField] Slider[] sliders;
    [SerializeField] int[] screenWidths;
    private int _activeScreenResIndex;
    [SerializeField] TMP_Dropdown resolutionDropdown;

    [SerializeField] Toggle fullscreenToggle;
    

    public void Play()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void Start()
    {
        _activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1);


        sliders[0].value = AudioManager.Instance.MasterVolumePercent;
        sliders[1].value = AudioManager.Instance.MusicVolumePercent;
        sliders[2].value = AudioManager.Instance.SfxVolumePercent;

        resolutionDropdown.value = _activeScreenResIndex;

        fullscreenToggle.isOn = isFullscreen;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        optionsMenuHolder.SetActive(false);
        creditsMenuHolder.SetActive(false);
        mainMenuHolder.SetActive(true);
        inCreditsMenu = false;
    }

    public void CreditsMenu()
    {
        mainMenuHolder.SetActive(false);
        creditsMenuHolder.SetActive(true);
        inCreditsMenu = true;
    }

    public void SetScreenResolutions()
    {
        float aspectRatio = 16 / 9;
        _activeScreenResIndex = resolutionDropdown.value;
        Screen.SetResolution(screenWidths[_activeScreenResIndex],
            (int)(screenWidths[_activeScreenResIndex] / aspectRatio), false);
        PlayerPrefs.SetInt("screen res index", _activeScreenResIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        resolutionDropdown.interactable = !isFullscreen;

        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolutions = allResolutions[^1];
            Screen.SetResolution(maxResolutions.width, maxResolutions.height, true);
        }
        else
        {
            SetScreenResolutions();
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }


    public void SetSfxValue(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void SetMusicValue(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    
}