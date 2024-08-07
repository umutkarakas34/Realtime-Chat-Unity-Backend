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

    private string baseUrl = "http://107.23.110.166:3000"; // Node.js sunucunuzun adresi

    void Start()
    {

        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        logoutButton.onClick.AddListener(Logout);
        chatPanel.SetActive(false); // Baþlangýçta chatPanel'i gizle
        loginPanel.SetActive(true); // Baþlangýçta loginPanel'i aktif yap
        registerPanel.SetActive(false); // Baþlangýçta registerPanel'i gizle
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
        WebSocketClient.Instance.CloseConnection(); // WebSocket baðlantýsýný kapat
    }

    IEnumerator RegisterCoroutine()
    {
        string username = registerUsernameInput?.text ?? "";
        string password = registerPasswordInput?.text ?? "";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

       if(registerConfirmPasswordInput.text == registerPasswordInput.text)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/register", form))
            {
                yield return www.SendWebRequest();

                if (www.responseCode == 500)
                {
                    registerFeedbackText.text = "Server Error!";
                }
                else if (www.responseCode == 409) // Kullanýcý adý zaten mevcut
                {
                    registerFeedbackText.text = "Username already exists!";
                }
                else
                {
                    feedbackText.text = "Registration successful! Please proceed to login.";
                    ShowLoginUI();
                }
            }
        }
        else
        {
            registerFeedbackText.text = "Passwords do not match.";
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

            if (www.responseCode == 500)
            {
                feedbackText.text = "Server Error!";
            }
            else if (www.responseCode == 401) // Kullanýcý adý veya þifre hatalý
            {
                feedbackText.text = "Invalid username or password!";
            }
            else
            {
                var jsonResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                feedbackText.text = "Login Successful!";
                StoreToken(jsonResponse.token);
                StoreUsername(jsonResponse.username);
                WebSocketClient.Instance.StartWebSocketConnection(); // WebSocket baðlantýsýný baþlat
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
        usernameInput.text = "";
        passwordInput.text = "";
        registerUsernameInput.text = "";
        registerPasswordInput.text = "";
        registerConfirmPasswordInput.text = "";
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string username;
    }
}
