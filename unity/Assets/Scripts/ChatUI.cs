using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUI : MonoBehaviour
{
    // Input field for typing messages
    public TMP_InputField inputField;

    // Button to send messages
    public Button sendButton;

    // Reference to the WebSocket client
    public WebSocketClient webSocketClient;

    void Start()
    {
        // Add listener to the send button to call SendMessage when clicked
        sendButton.onClick.AddListener(SendMessage);
    }

    void SendMessage()
    {
        // Get the message from the input field
        string message = inputField.text;

        // Check if the message is not empty
        if (!string.IsNullOrEmpty(message))
        {
            // Send the message to the server via WebSocket client
            webSocketClient.SendMessageToServer(message);

            // Clear the input field after sending the message
            inputField.text = "";
        }
    }
}
