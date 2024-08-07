# Realtime Chat Unity-Backend

## Project Description

This project includes both backend and frontend components for a real-time chat application based on Unity. The backend is written in TypeScript and utilizes NATS, Redis, and WebSocket technologies to enable real-time messaging. The frontend is developed using Unity and supports real-time chat functionality through a WebSocket connection.

## Table of Contents

1. [Technologies Used](#technologies-used)
2. [Installation](#installation)
3. [Backend Details](#backend-details)
4. [Unity C# SDK](#unity-c-sdk)
5. [Deployment](#deployment)
6. [Common Issues and Solutions](#common-issues-and-solutions)
7. [Demo Video](#demo-video)

## Technologies Used

### TypeScript ve Node.js

TypeScript has been used for backend development in this project. TypeScript provides static type checking, making the code more secure and readable. Node.js provides the runtime environment for the application.

### Redis

Redis is used as the database. User session management and message persistence are handled by Redis. Its fast and efficient data storage capabilities offer advantages in real-time applications.

### NATS

NATS is used as the messaging system. NATS provides high-performance, low-latency messaging and is used for the transmission of real-time messages. For this project, NATS is utilized with a publish/subscribe model to distribute messages.

### WebSocket

WebSocket is used for real-time communication. WebSocket provides bidirectional communication, allowing users to exchange messages instantly.

### Unity

Unity is used for frontend development. Unity is a powerful platform for developing games and interactive content. In this project, Unity is used to create an interface where users can chat in real-time.

## Installation

### Requirements

- Node.js ve npm
- Redis
- NATS Server
- Unity

### Steps

#### Backend Setup

**Note: The following instructions are for Linux systems.**

1. **Clone the repository:**

   ```bash
   git clone https://github.com/umutkarakas34/Realtime-Chat-Unity-Backend.git
   cd Realtime-Chat-Unity-Backend/chat-backend
   ```

2. **Install the required packages:**

   ```bash
   npm install
   ```

3. **Start the backend using PM2:**

   ```bash
   pm2 start src/server.ts --name "chat-backend"
   ```

##### Redis Installation

**Install and run Redis:**

1. **Install Redis:**

   ```bash
   sudo yum install redis -y
   ```

2. **Start Redis:**

   ```bash
   redis-server &
   ```

##### NATS Installation

**Install and run the NATS server:**

1. **Download and install NATS:**

   ```bash
   wget https://github.com/nats-io/nats-server/releases/download/v2.9.2/nats-server-v2.9.2-linux-amd64.tar.gz
   tar -xvzf nats-server-v2.9.2-linux-amd64.tar.gz
   sudo mv nats-server-v2.9.2-linux-amd64/nats-server /usr/local/bin/
   ```

2. **Start the NATS server:**

   ```bash
   nats-server -p 4222 &
   ```

#### Frontend

**Set up the frontend components using Unity:**

1. **Open the Unity project:**

   ```bash
   cd Realtime-Chat-Unity-Backend/unity
   ```

2. **Open and configure the project in the Unity editor.**

3. **Perform the build process.**

4. **For more information and integration with the Unity SDK, refer to the [Unity C# SDK](#unity-c-sdk) section.**

## Backend Details

### Configuration and Running

The backend is a Node.js application based on Express.js developed with TypeScript and includes the following features:

- **User Session Management:** Secure session management is provided using JWT.
- **Real-Time Messaging:** Real-time messaging is enabled using WebSocket and NATS.
- **Message Persistence:** Message persistence is handled with Redis.

To run the backend, use PM2:

```bash
pm2 start src/server.ts --name "chat-backend"
```

### Architectural Decisions and High Concurrency Management

The project employs a modular architecture to ensure scalability and maintainability. Each component (WebSocket, NATS, Redis) is structured as an independent module, enhancing readability and ease of maintenance.

- **Modular Architecture:** The application is divided into independent modules for WebSocket, NATS, and Redis. This approach ensures better code organization and scalability.

- **High Concurrency Management:** WebSocket connections and messaging via NATS and Redis ensure consistency and reliability under high concurrency. PM2 is used for process management, providing high performance and fault tolerance.

- **Security:** JWT is used for secure session management, and SSL/TLS is employed for WebSocket connections to ensure secure data transmission.

### Endpoints

**Base URL**

    ```
    http://localhost:3000
    ```

#### 1. **Get Messages**

Retrieve all messages from the chat.

- **Endpoint:** `GET /messages`
- **Success Response:**
  - **Code:** 200 OK
  - **Content:**
    ```json
    [
      { "username": "user1", "text": "Hello, world!" },
      { "username": "user2", "text": "Hi there!" }
    ]
    ```
- **Error Response:**
  - **Code:** 500 Internal Server Error
  - **Content:**
    ```json
    "Error getting messages"
    ```

#### 2. **Register User**

Register a new user.

- **Endpoint:** `POST /register`
- **Request Body:**
  ```json
  {
    "username": "user1",
    "password": "password123"
  }
  ```
- **Success Response:**
  - **Code:** 200 OK
  - **Content:**
    ```json
    "User registered successfully"
    ```
- **Error Response:**

  - **Code:** 409 Conflict
  - **Content:**
    ```json
    "Username already exists"
    ```

- **Code: 500 Internal Server Error**
  - **Content:**
    ```json
    "Error registering user"
    ```

#### 3. **Login User**

Login User

- **Endpoint:** `POST /login`
- **Request Body:**
  ```json
  {
    "username": "user1",
    "password": "password123"
  }
  ```
- **Success Response:**
  - **Code:** 200 OK
  - **Content:**
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "username": "user1"
    }
    ```
- **Error Response:**

  - **Code:** 401 Unauthorized
  - **Content:**
    ```json
    "Invalid credentials"
    ```

- **Code: 500 Internal Server Error**

  - **Content:**
    ```json
    "Error logging in"
    ```

## Unity C# SDK

### Installation Instructions

1. **Open Your Unity Project:**
   Open Unity Editor and load your project.

2. **Import Project Files:**
   Add the `Scripts` folder under the `Assets` directory to your project.

### API Reference

Main classes and methods in the project:

- **AuthManager.cs**: Manages user authentication processes.

  - **Public Değişkenler:**
    - `TMP_InputField registerUsernameInput`: Username input for registration.
    - `TMP_InputField registerPasswordInput`: Password input for registration.
    - `TMP_InputField registerConfirmPasswordInput`: Password confirmation input for registration.
    - `Button registerButton`: Registration button.
    - `TMP_Text registerFeedbackText`: Feedback text for registration.
    - `TMP_InputField usernameInput`: Username input for login.
    - `TMP_InputField passwordInput`: Password input for login.
    - `Button loginButton`: Login button.
    - `TMP_Text feedbackText`: Feedback text for login.
    - `GameObject loginPanel`: Login panel.
    - `GameObject registerPanel`: Registration panel.
    - `GameObject chatPanel`: Chat panel.
    - `Button logoutButton`: Logout button.
  - **Public Metodlar:**
    - `Register()`: Initiates the registration process.
    - `Login()`: Initiates the login process.
    - `Logout()`: Logs out and updates the relevant panels.

- **WebSocketClient.cs**: Manages the WebSocket connection and handles message sending and receiving.

  - **Public Değişkenler:**
    - `TMP_InputField messageInputField`: Message input field.
    - `Button sendButton`: Send message button.
    - `TMP_Text messagesText`: Text area where messages are displayed.
    - `ScrollRect scrollRect`: Area where messages are scrolled.
  - **Public Metodlar:**
    - `StartWebSocketConnection()`: Starts the WebSocket connection.
    - `CloseConnection()`: Closes the WebSocket connection.
    - `SendMessageToServer(string messageText)`: Sends a message to the server.

- **PanelSwitcher.cs**: Allows switching between panels.
  - **Public Değişkenler:**
    - `GameObject loginPanel`: Login panel.
    - `GameObject registerPanel`: Registration panel.
    - `GameObject chatPanel`: Chat panel.
  - **Public Metodlar:**
    - `ShowLoginPanel()`: Shows the login panel.
    - `ShowRegisterPanel()`: Shows the registration panel.
    - `ShowChatPanel()`: Shows the chat panel.

### Detailed Explanations and Use Cases

This SDK enables users to add real-time chat functionality to Unity-based applications. Users can quickly register, log in, and chat using this SDK.

#### Use Cases

1. **Real-Time Chat Applications:**

   - Ideal for meeting the needs of instant messaging.

2. **In-Game Chat Systems:**

   - Facilitates communication among players in multiplayer games.

## Deployment

This project can be deployed on AWS EC2. However, these steps are optional, and users can also run the project on their local environments.

### AWS Deployment Steps

#### Create an EC2 Instance:

Create a new EC2 instance through the AWS Management Console.

#### Configure Security Groups:

Add the necessary ports (3000, 6379, 4222) to your security group.

#### Connect to the Server:

Connect to your EC2 instance using SSH.

#### Install and Configure Required Software:

Install Node.js, Redis, and NATS.
Follow the steps above to run the backend.

## Common Issues and Solutions

### Connection Errors:

- Ensure that Redis and NATS are running.
- Check the security group settings.

### Unity Connection Issues:

- Verify that the backend URL is correct.
- Check that WebSocket connections are working properly.

### Server Errors:

- Check PM2 logs: `pm2 logs`
- Ensure that all required packages are installed.

## Demo Video

[Demo Video](https://www.youtube.com/watch?v=OqqXZhoEqwM)

### Executable Build

The latest build of the project can be found in the `Build/Windows` directory. This directory contains the executable (`RealTimeChat.exe`) file for the project, which is the latest compiled version ready for use.
