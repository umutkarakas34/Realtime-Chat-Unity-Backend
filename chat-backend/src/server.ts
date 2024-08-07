import express, { Request, Response } from 'express';
import { createServer } from 'http';
import WebSocket, { WebSocketServer } from 'ws';
import { connect, StringCodec, NatsConnection, SubscriptionOptions, Msg } from 'nats';
import { createClient } from 'redis';
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import bodyParser from 'body-parser';

interface IMessage {
    username: string;
    text: string;
}

interface IUser {
    username: string;
    password: string;
}

const app = express();
const port = 3000;
const wsUrl = "ws://localhost:3000";
const jwtSecret = process.env.JWT_SECRET || 'defaultSecret'; // Güvenlik için çevresel değişken kullanın

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const redisClient = createClient({
    socket: {
        host: 'localhost',
        port: 6379
    }
});

redisClient.on('error', (err) => {
    console.error('Redis error: ', err);
    if (err.code === 'ECONNREFUSED') {
        setTimeout(() => {
            console.log('Attempting to reconnect to Redis...');
            redisClient.connect();
        }, 5000);
    }
});

let natsConnection: NatsConnection;
let sc = StringCodec();

const server = createServer(app);
const wss = new WebSocketServer({ server });

(async () => {
    try {
        natsConnection = await connect({ servers: "nats://localhost:4222" });
        await redisClient.connect();
        console.log("Connected to Redis and NATS");

        const subOptions: SubscriptionOptions = {
            callback: (err, msg: Msg) => {
                if (err) {
                    console.error('Error:', err);
                    return;
                }
                const message = sc.decode(msg.data) as string;
                console.log(`Received message from NATS: ${message}`);
                
                wss.clients.forEach(client => {
                    if (client.readyState === WebSocket.OPEN) {
                        client.send(message);
                    }
                });
            },
            queue: 'message_queue' // Mesaj sırasının korunması için bir kuyruk kullanın
        };

        await natsConnection.subscribe("chat.messages", subOptions);

        server.listen(port, () => {
            console.log(`Server is running on http://localhost:${port}`);
        });
    } catch (error) {
        console.error("Connection error:", error);
    }
})();

wss.on('connection', (ws: WebSocket) => {
    console.log('Client connected');

    ws.on('message', (data: WebSocket.RawData) => {
        const message = JSON.parse(data.toString()) as IMessage;
        console.log(`Received message: ${message.username}: ${message.text}`);
        
        const messageToSave = JSON.stringify(message);
        redisClient.rPush('messages', messageToSave);

        if (natsConnection && sc) {
            natsConnection.publish("chat.messages", sc.encode(messageToSave));
        }
    });

    ws.on('close', () => {
        console.log('Client disconnected');
    });

    ws.on('error', (error) => {
        console.log('WebSocket error: ', error);
        setTimeout(() => {
            connectWebSocket();
        }, 5000);
    });
});

// Mevcut mesajları çeken endpoint
app.get('/messages', async (req: Request, res: Response) => {
    try {
        const messages = await redisClient.lRange('messages', 0, -1);
        res.status(200).json(messages.map(message => JSON.parse(message)));
    } catch (err) {
        console.error('Error getting messages:', err);
        res.status(500).send('Error getting messages');
    }
});

// Yeniden bağlanma işlevi
function connectWebSocket() {
    const ws = new WebSocket(wsUrl);

    ws.on('open', () => {
        console.log('WebSocket connection reopened');
    });

    ws.on('message', (data: WebSocket.RawData) => {
        const message = JSON.parse(data.toString()) as IMessage;
        console.log(`Received message: ${message.username}: ${message.text}`);
        
        const messageToSave = JSON.stringify(message);
        redisClient.rPush('messages', messageToSave);

        if (natsConnection && sc) {
            natsConnection.publish("chat.messages", sc.encode(messageToSave));
        }
    });

    ws.on('close', () => {
        console.log('Client disconnected');
        setTimeout(() => {
            connectWebSocket();
        }, 5000);
    });

    ws.on('error', (error) => {
        console.log('WebSocket error: ', error);
        setTimeout(() => {
            connectWebSocket();
        }, 5000);
    });
}

// Kullanıcı Kayıt İşlemi
app.post('/register', async (req: Request, res: Response) => {
    const { username, password }: IUser = req.body;

    try {
        const existingUser = await redisClient.hGet('users', username);
        if (existingUser) {
            return res.status(409).send('Username already exists');
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        await redisClient.hSet('users', username, hashedPassword);
        res.status(200).send('User registered successfully');
    } catch (err) {
        console.error('Error registering user:', err);
        res.status(500).send('Error registering user');
    }
});

// Kullanıcı Giriş İşlemi
app.post('/login', async (req: Request, res: Response) => {
    const { username, password }: IUser = req.body;

    try {
        const hashedPassword = await redisClient.hGet('users', username);
        if (!hashedPassword || !(await bcrypt.compare(password, hashedPassword))) {
            return res.status(401).send('Invalid credentials');
        }

        const token = jwt.sign({ username }, jwtSecret, { expiresIn: '1h' });
        res.json({ token, username });
    } catch (err) {
        console.error('Error logging in:', err);
        res.status(500).send('Error logging in');
    }
});

// Kullanıcı adını döndüren endpoint
app.get('/username', async (req: Request, res: Response) => {
    const token = req.headers.authorization?.split(' ')[1];

    if (!token) {
        return res.status(401).send('Unauthorized');
    }

    try {
        const decoded = jwt.verify(token, jwtSecret) as { username: string };
        res.json({ username: decoded.username });
    } catch (err) {
        console.error('Error verifying token:', err);
        res.status(401).send('Unauthorized');
    }
});

// Tüm kullanıcıları çeken route
app.get('/get-all-users', async (req: Request, res: Response) => {
    try {
        const users = await redisClient.hKeys('users');
        console.log('Retrieved users:', users);

        const userDetails = await Promise.all(users.map(async (username) => {
            const password = await redisClient.hGet('users', username);
            return { username, password };
        }));

        res.status(200).json(userDetails);
    } catch (err) {
        console.error('Error getting all users:', err);
        res.status(500).send('Error getting all users');
    }
});
