using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;

    void Start()
    {
        ShowLoginPanel(); // Baþlangýçta LoginPanel'i göster
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
