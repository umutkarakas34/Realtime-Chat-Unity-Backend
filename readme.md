# Realtime Chat Unity-Backend

## Proje Açıklaması

Bu proje, Unity tabanlı bir gerçek zamanlı sohbet uygulaması için backend ve frontend bileşenlerini içerir. Backend, TypeScript ile yazılmıştır ve NATS, Redis ve WebSocket teknolojilerini kullanarak gerçek zamanlı mesajlaşma sağlamaktadır. Frontend ise Unity kullanılarak geliştirilmiştir ve WebSocket bağlantısı üzerinden gerçek zamanlı sohbet işlevselliğini destekler.

## İçindekiler

1. [Kurulum](#kurulum)
2. [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
3. [Backend Detayları](#backend-detayları)
4. [Frontend Detayları](#frontend-detayları)
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

### Örnek API Endpoints

- **Kullanıcı Kaydı:** `POST /register`
- **Kullanıcı Girişi:** `POST /login`
- **Mesajları Getirme:** `GET /messages`

## Frontend Detayları

Unity kullanılarak geliştirilmiş olan frontend, kullanıcıların sohbet edebileceği bir arayüz sağlar. Kullanıcılar, kullanıcı adı ve şifre ile giriş yaparak sohbet odasına katılabilirler. Frontend, WebSocket bağlantısı üzerinden gerçek zamanlı mesaj alışverişi yapar.

### Unity Ayarları

1. **Proje Ayarları:**

   - Unity projesinde, `Assets/Scripts` klasörü altındaki scriptler kullanılarak oturum yönetimi ve mesajlaşma işlemleri gerçekleştirilir.

2. **WebSocket Bağlantısı:**
   - `WebSocketClient.cs` dosyasında, WebSocket bağlantısı üzerinden mesajların gönderilmesi ve alınması işlemleri gerçekleştirilir.

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

[Demo Video](<iframe width="884" height="708" src="https://www.youtube.com/embed/OqqXZhoEqwM" title="demo" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>)
