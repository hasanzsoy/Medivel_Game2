using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Ayarlar Paneli")]
    public GameObject settingsPanel;
    public GameObject damagePanel;

    private bool isMenuOpen = false;
    private InputAction menuAction;

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


    void OnEnable()
    {
        menuAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        menuAction.performed += ctx => ToggleMenu();
        menuAction.Enable();
    }

    void OnDisable()
    {
        if (menuAction != null)
        {
            menuAction.performed -= ctx => ToggleMenu();
            menuAction.Disable();
        }
    }

    private void ToggleMenu()
    {
        if (!isMenuOpen)
            OpenSettingsMenu();
        else
            CloseSettingsMenu();
    }

    public void OpenSettingsMenu()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isMenuOpen = true;
    }

    public void CloseSettingsMenu()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMenuOpen = false;
    }

    public void ShowDamagePanel()
    {
        if (damagePanel != null)
        {
            damagePanel.SetActive(true);
            Invoke("HideDamagePanel", 0.5f); // 0.5 saniye sonra paneli gizle
        }
    }
}