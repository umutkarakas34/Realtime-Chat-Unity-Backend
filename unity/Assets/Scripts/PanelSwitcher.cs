using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    // UI panels for login and registration
    public GameObject loginPanel;
    public GameObject registerPanel;

    void Start()
    {
        // Show the LoginPanel initially
        ShowLoginPanel();
    }

    // Show the LoginPanel and hide the RegisterPanel
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    // Show the RegisterPanel and hide the LoginPanel
    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
