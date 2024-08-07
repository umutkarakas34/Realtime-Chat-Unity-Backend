module.exports = {
    apps: [
        {
            name: "chat-backend",
            script: "./src/server.js",
            instances: 1,
            exec_mode: "fork",
            watch: false,
            env: {
                NODE_ENV: "production",
            },
            env_development: {
                NODE_ENV: "development",
            },
        },
    ],
};
