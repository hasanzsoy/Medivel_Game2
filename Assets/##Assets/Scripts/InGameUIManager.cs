using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Ayarlar Paneli")]
    public GameObject settingsPanel;
    public GameObject damagePanel;
    public GameObject questPanel; // Eklendi

    private bool isMenuOpen = false;
    private bool isQuestOpen = false;
    private InputAction menuAction;
    private InputAction questAction;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider deathSlider;
    public Slider footstepSlider;
    public Slider swordSlider;
    public Slider masterSlider;

    void Start()
    {

        Debug.Log("[InGameUIManager] Start çaðrýldý.");
        // Slider baðlantýlarý ayný
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        deathSlider.onValueChanged.AddListener(AudioManager.Instance.SetDeathVolume);
        footstepSlider.onValueChanged.AddListener(AudioManager.Instance.SetFootstepVolume);
        swordSlider.onValueChanged.AddListener(AudioManager.Instance.SetSwordVolume);
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }

    void OnEnable()
    {
        Debug.Log("[InGameUIManager] OnEnable çaðrýldý.");
        menuAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        menuAction.performed += ctx => ToggleMenu();
        menuAction.Enable();
    }


    void OnDisable()
    {
        Debug.Log("[InGameUIManager] OnDisable çaðrýldý.");
        if (menuAction != null)
        {
            menuAction.performed -= ctx => ToggleMenu();
            menuAction.Disable();
        }
    }

    private void ToggleMenu()
    {
        Debug.Log($"[InGameUIManager] ToggleMenu çaðrýldý. isMenuOpen: {isMenuOpen}");
        if (isMenuOpen)
            CloseSettingsMenu();
        else
            OpenSettingsMenu();
    }

    public void OpenSettingsMenu()
    {
        Debug.Log("[InGameUIManager] OpenSettingsMenu çaðrýldý.");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isMenuOpen = true;
    }

    public void CloseSettingsMenu()
    {
        Debug.Log("[InGameUIManager] CloseSettingsMenu çaðrýldý.");
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMenuOpen = false;
    }

    public void OpenQuestPanel()
    {
        Debug.Log("[InGameUIManager] OpenQuestPanel çaðrýldý.");
        if (questPanel != null)
        {
            questPanel.SetActive(true);
            Debug.Log("[InGameUIManager] questPanel.SetActive(true) çaðrýldý.");
        }
        else
        {
            Debug.LogWarning("[InGameUIManager] questPanel referansý atanmadý!");
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isQuestOpen = true;
    }

    public void CloseQuestPanel()
    {
        Debug.Log("[InGameUIManager] CloseQuestPanel çaðrýldý.");
        if (questPanel != null)
            questPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isQuestOpen = false;
    }

    public void ShowDamagePanel()
    {
        Debug.Log("[InGameUIManager] ShowDamagePanel çaðrýldý.");
        if (damagePanel != null)
        {
            damagePanel.SetActive(true);
            Invoke("HideDamagePanel", 0.5f);
        }
    }
}