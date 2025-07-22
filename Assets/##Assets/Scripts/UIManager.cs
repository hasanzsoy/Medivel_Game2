using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button continueButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button backButton;
    public Button exitButton;

    public TextMeshProUGUI titleText;

    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public GameObject exitPanel;

    public Slider volumeSlider;

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
