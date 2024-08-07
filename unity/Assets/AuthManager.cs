using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public Button registerButton;
    public TMP_Text registerFeedbackText;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText;

    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject chatPanel;
    public Button logoutButton;

    private string baseUrl = "http://localhost:3000"; // Node.js sunucunuzun adresi

    void Start()
    {
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        logoutButton.onClick.AddListener(Logout);
        chatPanel.SetActive(false); // Ba�lang��ta chatPanel'i gizle
        loginPanel.SetActive(true); // Ba�lang��ta loginPanel'i aktif yap
        registerPanel.SetActive(false); // Ba�lang��ta registerPanel'i gizle
    }

    void Register()
    {
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;

        if (password != confirmPassword)
        {
            registerFeedbackText.text = "Passwords do not match!";
            return;
        }

        StartCoroutine(RegisterCoroutine());
    }

    void Login()
    {
        StartCoroutine(LoginCoroutine());
    }

    void Logout()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        ShowLoginUI();
        WebSocketClient.Instance.CloseConnection(); // WebSocket ba�lant�s�n� kapat
    }

    IEnumerator RegisterCoroutine()
    {
        string username = registerUsernameInput?.text ?? "";
        string password = registerPasswordInput?.text ?? "";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/register", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                registerFeedbackText.text = "Error: " + www.error;
            }
            else if (www.responseCode == 409) // Kullan�c� ad� zaten mevcut
            {
                registerFeedbackText.text = "Username already exists!";
            }
            else
            {
                registerFeedbackText.text = "Registration Successful!";
                ShowLoginUI();
            }
        }
    }

    IEnumerator LoginCoroutine()
    {
        string username = usernameInput?.text ?? "";
        string password = passwordInput?.text ?? "";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/login", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText.text = "Error: " + www.error;
            }
            else if (www.responseCode == 401) // Kullan�c� ad� veya �ifre hatal�
            {
                feedbackText.text = "Invalid username or password!";
            }
            else
            {
                var jsonResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                feedbackText.text = "Login Successful!";
                StoreToken(jsonResponse.token);
                StoreUsername(jsonResponse.username);
                WebSocketClient.Instance.StartWebSocketConnection(); // WebSocket ba�lant�s�n� ba�lat
                ShowChatUI();
            }
        }
    }

    void StoreToken(string token)
    {
        PlayerPrefs.SetString("userToken", token);
        PlayerPrefs.Save();
    }

    void StoreUsername(string username)
    {
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();
    }

    void ShowLoginUI()
    {
        registerPanel?.SetActive(false);
        loginPanel?.SetActive(true);
        chatPanel?.SetActive(false);
        ClearFeedbackText();
    }

    void ShowChatUI()
    {
        loginPanel?.SetActive(false);
        chatPanel?.SetActive(true);
    }

    void ShowRegisterUI()
    {
        loginPanel?.SetActive(false);
        registerPanel?.SetActive(true);
        ClearFeedbackText();
    }

    void ClearFeedbackText()
    {
        registerFeedbackText.text = "";
        feedbackText.text = "";
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string username;
    }
}
