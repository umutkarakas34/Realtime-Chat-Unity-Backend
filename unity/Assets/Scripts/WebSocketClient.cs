using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class WebSocketClient : MonoBehaviour
{
    private static WebSocketClient instance;
    private WebSocket ws;
    public TMP_InputField messageInputField;
    public Button sendButton;
    public TMP_Text messagesText;
    public ScrollRect scrollRect;

    private string wsUrl = "ws://107.23.110.166:3000";
    private string username;
    private Dictionary<string, Color> userColors = new Dictionary<string, Color>();
    private HashSet<string> receivedMessages = new HashSet<string>(); // Eklenen mesajlarý takip etmek için
    private bool isFirstConnection = true; // Ýlk baðlantý kontrolü

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static WebSocketClient Instance
    {
        get { return instance; }
    }

    public void StartWebSocketConnection()
    {
        StartCoroutine(GetUsernameFromServer());
    }

    IEnumerator GetUsernameFromServer()
    {
        string token = PlayerPrefs.GetString("userToken", "");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("User token is missing");
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get("http://107.23.110.166:3000/username");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error getting username: " + request.error);
        }
        else
        {
            var response = JsonConvert.DeserializeObject<UsernameResponse>(request.downloadHandler.text);
            username = response.username;
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.Save();

            Connect();
        }
    }

    void Connect()
    {
        ws = new WebSocket(wsUrl);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened");

            if (isFirstConnection)
            {
                StartCoroutine(LoadExistingMessages());
                isFirstConnection = false; // Ýlk baðlantýdan sonra false yap
            }
        };

        ws.OnMessage += (sender, e) =>
        {
            try
            {
                Debug.Log("Message received from server: " + e.Data);
                UnityMainThreadDispatcher.Enqueue(() => AddMessageToUI(e.Data));
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception during OnMessage event: " + ex.Message);
            }
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed");
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        ws.Connect();
    }

    IEnumerator LoadExistingMessages()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://107.23.110.166:3000/messages");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading existing messages: " + request.error);
        }
        else
        {
            var messages = JsonConvert.DeserializeObject<List<Message>>(request.downloadHandler.text);
            foreach (var message in messages)
            {
                string messageJson = JsonConvert.SerializeObject(message);
                if (!receivedMessages.Contains(messageJson)) // Mesajýn zaten eklenmiþ olup olmadýðýný kontrol et
                {
                    AddMessageToUI(messageJson);
                    receivedMessages.Add(messageJson); // Mesajý eklenenler listesine ekle
                }
            }
        }
    }

    public void CloseConnection()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null; // WebSocket baðlantýsýný serbest býrak
        }
    }

    public void SendMessageToServer(string messageText)
    {
        Debug.Log("Sending message to server...");
        if (ws != null && ws.IsAlive)
        {
            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError("Username is null or empty");
                return;
            }

            var message = new
            {
                username,
                text = messageText
            };
            string messageJson = JsonConvert.SerializeObject(message);
            ws.Send(messageJson);

            if (messageInputField != null)
            {
                messageInputField.text = "";
            }
            else
            {
                Debug.LogError("messageInputField is null");
            }

            Debug.Log("Message sent to server: " + messageJson);
        }
        else
        {
            Debug.LogError("WebSocket is not connected");
        }
    }

    void AddMessageToUI(string messageJson)
    {
        var message = JsonConvert.DeserializeObject<Message>(messageJson);
        if (message == null)
        {
            Debug.LogError("Failed to deserialize message: " + messageJson);
            return;
        }

        if (!userColors.ContainsKey(message.username))
        {
            userColors[message.username] = GetRandomColor();
        }

        string colorHex = ColorUtility.ToHtmlStringRGB(userColors[message.username]);

        if (messagesText != null)
        {
            messagesText.text += $"<color=#{colorHex}>{message.username}: {message.text}</color>\n";
            Debug.Log(message.username);
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f; // En alta kaydýr
            }
        }
        else
        {
            Debug.LogError("messagesText is not assigned.");
        }
    }

    private Color GetRandomColor()
    {
        Color[] colors = {
            Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan, Color.white, Color.gray,
            new Color(1.0f, 0.5f, 0.0f), // Orange
            new Color(0.5f, 0.0f, 0.5f), // Purple
            new Color(0.0f, 0.5f, 0.5f), // Teal
            new Color(0.5f, 0.5f, 0.0f), // Olive
            new Color(0.75f, 0.75f, 0.75f), // Silver
            new Color(1.0f, 0.0f, 0.5f), // Rose
            new Color(0.5f, 0.0f, 0.0f), // Maroon
            new Color(0.0f, 0.0f, 0.5f), // Navy
            new Color(0.5f, 0.5f, 1.0f), // LightBlue
            new Color(0.5f, 1.0f, 0.5f), // LightGreen
            new Color(1.0f, 0.5f, 0.5f), // LightRed
            new Color(1.0f, 1.0f, 0.5f), // LightYellow
            new Color(1.0f, 0.5f, 1.0f), // LightMagenta
            new Color(0.5f, 1.0f, 1.0f), // LightCyan
            new Color(0.5f, 0.5f, 0.5f), // DarkGray
            new Color(0.25f, 0.25f, 0.25f), // VeryDarkGray
            new Color(0.8f, 0.2f, 0.2f), // LightMaroon
            new Color(0.2f, 0.8f, 0.2f), // LightTeal
            new Color(0.2f, 0.2f, 0.8f), // LightPurple
            new Color(0.8f, 0.8f, 0.2f), // LightOlive
            new Color(0.8f, 0.5f, 0.2f), // LightOrange
        };
        int randomIndex = Random.Range(0, colors.Length);
        return colors[randomIndex];
    }

    void OnDestroy()
    {
        CloseConnection();
        ws = null; // WebSocket baðlantýsýný serbest býrak
    }

    [System.Serializable]
    public class Message
    {
        public string username;
        public string text;
    }

    [System.Serializable]
    public class UsernameResponse
    {
        public string username;
    }
}
