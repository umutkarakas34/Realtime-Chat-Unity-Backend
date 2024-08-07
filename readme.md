# Realtime Chat Unity-Backend

## Proje Açıklaması

Bu proje, Unity tabanlı bir gerçek zamanlı sohbet uygulaması için backend ve frontend bileşenlerini içerir. Backend, TypeScript ile yazılmıştır ve NATS, Redis ve WebSocket teknolojilerini kullanarak gerçek zamanlı mesajlaşma sağlamaktadır. Frontend ise Unity kullanılarak geliştirilmiştir ve WebSocket bağlantısı üzerinden gerçek zamanlı sohbet işlevselliğini destekler.

## İçindekiler

1. [Kurulum](#kurulum)
2. [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
3. [Backend Detayları](#backend-detayları)
4. [Unity C# SDK](#unity-c-sdk)
5. [AWS Üzerinde Dağıtım](#aws-üzerinde-dağıtım)
6. [Sık Karşılaşılan Sorunlar ve Çözümleri](#sık-karşılaşılan-sorunlar-ve-çözümleri)
7. [Demo Video](#demo-video)

## Kurulum

### Gereksinimler

- Node.js ve npm
- Redis
- NATS Server
- Unity

### Adımlar

#### Backend Kurulumu

1. **Depoyu klonlayın:**

   ```bash
   git clone https://github.com/umutkarakas34/Realtime-Chat-Unity-Backend.git
   cd Realtime-Chat-Unity-Backend/chat-backend
   ```

2. **Gerekli paketleri yükleyin:**

   ```bash
   npm install
   ```

3. **PM2 kullanarak backend'i başlatın:**

   ```bash
   pm2 start src/server.ts --name "chat-backend"
   ```

##### Redis Kurulumu

**Redis'in kurulumunu ve çalıştırılmasını gerçekleştirin:**

1. **Redis'i yükleyin:**

   ```bash
   sudo yum install redis -y
   ```

2. **Redis'i başlatın:**

   ```bash
   redis-server &
   ```

##### NATS Kurulumu

**NATS server'ın kurulumunu ve çalıştırılmasını gerçekleştirin:**

1. **NATS'i indirin ve yükleyin:**

   ```bash
   wget https://github.com/nats-io/nats-server/releases/download/v2.9.2/nats-server-v2.9.2-linux-amd64.tar.gz
   tar -xvzf nats-server-v2.9.2-linux-amd64.tar.gz
   sudo mv nats-server-v2.9.2-linux-amd64/nats-server /usr/local/bin/
   ```

2. **NATS server'ı başlatın:**

   ```bash
   nats-server -p 4222 &
   ```

#### Frontend Kurulumu

**Unity kullanarak frontend bileşenlerini kurun:**

1. **Unity projesini açın:**

   ```bash
   cd Realtime-Chat-Unity-Backend/unity
   ```

2. **Unity editoründe projeyi açın ve yapılandırın.**

3. **Build işlemini gerçekleştirin.**

## Kullanılan Teknolojiler

### Node.js ve TypeScript

Backend geliştirmesi için Node.js kullanılmıştır. TypeScript, statik tip kontrolü sağlar ve kodun daha güvenli ve okunabilir olmasını sağlar.

### Redis

Redis, veritabanı olarak kullanılmıştır. Kullanıcı oturum yönetimi ve mesajların kalıcılığı Redis ile sağlanmıştır. Redis'in hızlı ve verimli veri saklama özellikleri, gerçek zamanlı uygulamalarda avantaj sağlar.

### NATS

NATS, mesajlaşma sistemi olarak kullanılmıştır. NATS, yüksek performanslı, düşük gecikmeli mesajlaşma sağlar ve gerçek zamanlı mesajların iletiminde kullanılır. Bu proje için, NATS yayıncı/abone modeli kullanılarak mesajların dağıtımı sağlanmıştır.

### WebSocket

WebSocket, gerçek zamanlı iletişim için kullanılmıştır. WebSocket, çift yönlü iletişim sağlar ve bu sayede kullanıcılar anlık olarak mesajlaşabilirler.

### Unity

Frontend geliştirmesi için Unity kullanılmıştır. Unity, oyun ve etkileşimli içerik geliştirmek için kullanılan güçlü bir platformdur. Bu proje için, Unity kullanılarak kullanıcıların gerçek zamanlı olarak sohbet edebileceği bir arayüz oluşturulmuştur.

## Backend Detayları

### Yapılandırma ve Çalıştırma

Backend, Express.js tabanlı bir Node.js uygulamasıdır ve aşağıdaki özellikleri içerir:

- **Kullanıcı Oturum Yönetimi:** JWT kullanılarak güvenli oturum yönetimi sağlanır.
- **Gerçek Zamanlı Mesajlaşma:** WebSocket ve NATS kullanılarak gerçek zamanlı mesajlaşma sağlanır.
- **Mesajların Kalıcılığı:** Redis ile mesajların kalıcılığı sağlanır.

Backend'in çalıştırılması için PM2 kullanılır:

```bash
pm2 start src/server.ts --name "chat-backend"
```

### Endpoints

**Base URL**

    ```bash
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

### Kurulum Talimatları

1. **Unity Projenizi Açın:**
   Unity Editor'ü açın ve projenizi yükleyin.

2. **Proje Dosyalarını İçe Aktarın:**
   `Assets` klasörü altındaki `Scripts` klasörünü projenize ekleyin.

### API Referansı

Projede bulunan başlıca sınıflar ve metodlar:

- **AuthManager.cs**: Kullanıcı kimlik doğrulama işlemlerini yönetir.

  - **Public Değişkenler:**
    - `TMP_InputField registerUsernameInput`: Kayıt kullanıcı adı girişi.
    - `TMP_InputField registerPasswordInput`: Kayıt şifre girişi.
    - `TMP_InputField registerConfirmPasswordInput`: Kayıt şifre doğrulama girişi.
    - `Button registerButton`: Kayıt butonu.
    - `TMP_Text registerFeedbackText`: Kayıt geri bildirim metni.
    - `TMP_InputField usernameInput`: Giriş kullanıcı adı girişi.
    - `TMP_InputField passwordInput`: Giriş şifre girişi.
    - `Button loginButton`: Giriş butonu.
    - `TMP_Text feedbackText`: Giriş geri bildirim metni.
    - `GameObject loginPanel`: Giriş paneli.
    - `GameObject registerPanel`: Kayıt paneli.
    - `GameObject chatPanel`: Sohbet paneli.
    - `Button logoutButton`: Çıkış butonu.
  - **Public Metodlar:**
    - `Register()`: Kayıt işlemini başlatır.
    - `Login()`: Giriş işlemini başlatır.
    - `Logout()`: Çıkış işlemini yapar ve ilgili panelleri günceller.

- **WebSocketClient.cs**: WebSocket bağlantısını yönetir ve mesajların gönderilip alınmasını sağlar.

  - **Public Değişkenler:**
    - `TMP_InputField messageInputField`: Mesaj girişi.
    - `Button sendButton`: Mesaj gönder butonu.
    - `TMP_Text messagesText`: Mesajların görüntülendiği metin alanı.
    - `ScrollRect scrollRect`: Mesajların kaydırıldığı alan.
  - **Public Metodlar:**
    - `StartWebSocketConnection()`: WebSocket bağlantısını başlatır.
    - `CloseConnection()`: WebSocket bağlantısını kapatır.
    - `SendMessageToServer(string messageText)`: Mesajı sunucuya gönderir.

- **PanelSwitcher.cs**: Paneller arasında geçiş yapmayı sağlar.
  - **Public Değişkenler:**
    - `GameObject loginPanel`: Giriş paneli.
    - `GameObject registerPanel`: Kayıt paneli.
    - `GameObject chatPanel`: Sohbet paneli.
  - **Public Metodlar:**
    - `ShowLoginPanel()`: Giriş panelini gösterir.
    - `ShowRegisterPanel()`: Kayıt panelini gösterir.
    - `ShowChatPanel()`: Sohbet panelini gösterir.

### Detaylı Açıklamalar ve Kullanım Senaryoları

Bu SDK, kullanıcıların Unity tabanlı uygulamalarda gerçek zamanlı sohbet işlevselliği eklemelerine olanak sağlar. Kullanıcılar, bu SDK'yı kullanarak hızlı bir şekilde kayıt olabilir, giriş yapabilir ve mesajlaşabilirler.

#### Kullanım Senaryoları

1. **Gerçek Zamanlı Sohbet Uygulamaları:**

   - Kullanıcıların anlık mesajlaşma ihtiyaçlarını karşılamak için idealdir.

2. **Oyun İçi Sohbet Sistemleri:**

   - Çok oyunculu oyunlarda oyuncuların birbirleriyle iletişim kurmalarını sağlar.

## AWS Üzerinde Dağıtım

Bu proje, AWS EC2 üzerinde dağıtılabilir. Ancak, bu adımlar isteğe bağlıdır ve kullanıcılar projeyi yerel ortamlarında da çalıştırabilirler.

### AWS Üzerinde Dağıtım Adımları

#### EC2 Sunucusu Oluşturun:

AWS Management Console üzerinden yeni bir EC2 instance oluşturun.

#### Güvenlik Gruplarını Yapılandırın:

Gerekli portları (3000, 6379, 4222) güvenlik grubunuza ekleyin.

#### Sunucuya Bağlanın:

SSH kullanarak EC2 instance'a bağlanın.

#### Gerekli Yazılımları Yükleyin ve Yapılandırın:

Node.js, Redis ve NATS'i yükleyin.
Yukarıdaki adımları izleyerek backend'i çalıştırın.

## Sık Karşılaşılan Sorunlar ve Çözümleri

### Bağlantı Hataları:

- Redis ve NATS'in çalıştığından emin olun.
- Güvenlik grubu ayarlarını kontrol edin.

### Unity ile Bağlantı Sorunları:

- Backend URL'sinin doğru olduğundan emin olun.
- WebSocket bağlantılarının doğru çalıştığını kontrol edin.

### Sunucu Hataları:

- PM2 loglarını kontrol edin: `pm2 logs`
- Gerekli paketlerin yüklü olduğundan emin olun.

## Demo Video

[Demo Video](https://www.youtube.com/watch?v=OqqXZhoEqwM)
