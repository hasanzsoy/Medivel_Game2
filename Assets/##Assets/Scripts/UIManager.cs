using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button continueButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button backButton;
    public Button exitButton;

    [Header("Panel ve Ögeler")]
    public TextMeshProUGUI titleText;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject exitPanel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider deathSlider;
    public Slider footstepSlider;
    public Slider swordSlider;
    public Slider masterSlider;


    void Start()
    {
        // Inspector’dan atanan slider referanslarý
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        deathSlider.onValueChanged.AddListener(AudioManager.Instance.SetDeathVolume);
        footstepSlider.onValueChanged.AddListener(AudioManager.Instance.SetFootstepVolume);
        swordSlider.onValueChanged.AddListener(AudioManager.Instance.SetSwordVolume);
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Harita");
    }

    public void ContinueGame()
    {
        // Logic to continue the game
        Debug.Log("Continue Game");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);
        titleText.GameObject().SetActive(false);
    }
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        exitPanel.SetActive(false);
        titleText.GameObject().SetActive(false);
    }
    public void OpenExitPanel()
    {
        exitPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        titleText.GameObject().SetActive(true);
    }
    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);
        titleText.GameObject().SetActive(true);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Exit Game");  
        Application.Quit();  
#endif
    }



}
