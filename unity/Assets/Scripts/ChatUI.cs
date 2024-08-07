using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public WebSocketClient webSocketClient;

    void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
    }

    void SendMessage()
    {
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            webSocketClient.SendMessageToServer(message);
            inputField.text = ""; // Mesaj gönderildikten sonra input alanýný temizle
        }
    }
}
