using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    // UI elements for registration
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public Button registerButton;
    public TMP_Text registerFeedbackText;

    // UI elements for login
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText;

    // Panels for UI management
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject chatPanel;
    public Button logoutButton;

    // Base URL for Node.js server
    private string baseUrl = "http://107.23.110.166:3000";

    void Start()
    {
        // Add listeners to buttons
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        logoutButton.onClick.AddListener(Logout);

        // Initialize UI panels
        chatPanel.SetActive(false); // Hide chatPanel initially
        loginPanel.SetActive(true); // Show loginPanel initially
        registerPanel.SetActive(false); // Hide registerPanel initially
    }

    void Register()
    {
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;

        // Check if passwords match
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
        // Clear stored user data
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Show login UI
        ShowLoginUI();

        // Close WebSocket connection
        WebSocketClient.Instance.CloseConnection();
    }

    IEnumerator RegisterCoroutine()
    {
        string username = registerUsernameInput?.text ?? "";
        string password = registerPasswordInput?.text ?? "";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        if (registerConfirmPasswordInput.text == registerPasswordInput.text)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/register", form))
            {
                yield return www.SendWebRequest();

                if (www.responseCode == 500)
                {
                    registerFeedbackText.text = "Server Error!";
                }
                else if (www.responseCode == 409) // Username already exists
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
            else if (www.responseCode == 401) // Invalid username or password
            {
                feedbackText.text = "Invalid username or password!";
            }
            else
            {
                var jsonResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                feedbackText.text = "Login Successful!";
                StoreToken(jsonResponse.token);
                StoreUsername(jsonResponse.username);

                // Start WebSocket connection
                WebSocketClient.Instance.StartWebSocketConnection();
                ShowChatUI();
            }
        }
    }

    // Store JWT token
    void StoreToken(string token)
    {
        PlayerPrefs.SetString("userToken", token);
        PlayerPrefs.Save();
    }

    // Store username
    void StoreUsername(string username)
    {
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();
    }

    // Show login UI
    void ShowLoginUI()
    {
        registerPanel?.SetActive(false);
        loginPanel?.SetActive(true);
        chatPanel?.SetActive(false);
        ClearFeedbackText();
    }

    // Show chat UI
    void ShowChatUI()
    {
        loginPanel?.SetActive(false);
        chatPanel?.SetActive(true);
    }

    // Show register UI
    void ShowRegisterUI()
    {
        loginPanel?.SetActive(false);
        registerPanel?.SetActive(true);
        ClearFeedbackText();
    }

    // Clear feedback text
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
