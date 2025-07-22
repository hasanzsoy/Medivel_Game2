using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIManager : MonoBehaviour
{
    [Header("Ayarlar Paneli")]
    public GameObject settingsPanel;

    private bool isMenuOpen = false;
    private InputAction menuAction;

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
}