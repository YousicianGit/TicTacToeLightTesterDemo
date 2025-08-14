using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using AltWebSocketSharp.Server;
using AltWebSocketSharp;

public class LightServer : MonoBehaviour
{
    private WebSocketServer server;
    private static string logFilePath;

    public static AppBehavior app;
    public static DriverBehavior driver;

    void Start()
    {
        logFilePath = Environment.GetEnvironmentVariable("LIGHT_SERVER_LOG_FILE");

        string host = Environment.GetEnvironmentVariable("LIGHT_SERVER_HOST") ?? "127.0.0.1";
        string portStr = Environment.GetEnvironmentVariable("LIGHT_SERVER_PORT") ?? "13000";
        int port = int.Parse(portStr);

        server = new WebSocketServer(System.Net.IPAddress.Any, port);

        server.AddWebSocketService<AppBehavior>("/altws/app");
        server.AddWebSocketService<DriverBehavior>("/altws");

        server.Start();

        Log($"WebSocket server listening on ws://{host}:{port}");
    }

    void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
            Log("WebSocket server stopped.");
        }
    }

    public static void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]");
        string line = $"{timestamp} {message}";
        Debug.Log(line);

        if (!string.IsNullOrEmpty(logFilePath))
        {
            try
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to write to log file: " + ex.Message);
            }
        }
    }

    public class AppBehavior : WebSocketBehavior
    {
        public void SendToApp(string message)
        {
            Send(message);
        }

        protected override void OnOpen()
        {
            Log($"App {Context.UserEndPoint} tries to connect.");
            lock (typeof(LightServer))
            {
                if (app != null)
                {
                    Log($"{Context.UserEndPoint} was rejected. App already connected.");
                    Context.WebSocket.Close((ushort)4002, "App already connected");
                    return;
                }
                app = this;
                Log("App connected.");
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
                parsed.TryGetValue("messageId", out var messageIdObj);
                string messageId = messageIdObj?.ToString();
                
                Log($"App sent a message to server: {e.Data}");

                if (driver != null)
                {
                    driver.SendToDriver(e.Data);
                    Log($"Server forwarded a message to driver: {messageId}");
                }
            }
            catch (Exception ex)
            {
                Log("Error handling message from app: " + ex);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            lock (typeof(LightServer))
            {
                if (app == this)
                {
                    app = null;
                    Log("App disconnected.");
                }
            }
        }
    }

    public class DriverBehavior : WebSocketBehavior
    {
        public void SendToDriver(string message)
        {
            Send(message);
        }

        protected override void OnOpen()
        {
            Log($"Driver {Context.UserEndPoint} tries to connect.");
            lock (typeof(LightServer))
            {
                if (driver != null)
                {
                    Log($"{Context.UserEndPoint} was rejected. Driver already connected.");
                    Context.WebSocket.Close((ushort)4005, "Driver already connected");
                    return;
                }
                driver = this;
                Log("Driver connected.");
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
                parsed.TryGetValue("commandName", out var commandNameObj);
                parsed.TryGetValue("messageId", out var messageIdObj);
                string commandName = commandNameObj?.ToString();
                string messageId = messageIdObj?.ToString();

                Log($"Driver sent a message to server: {e.Data}");

                if (commandName == "getServerVersion")
                {
                    var response = new Dictionary<string, object>
                    {
                        { "commandName", commandName },
                        { "messageId", messageId },
                        { "data", JsonConvert.SerializeObject("2.2") }
                    };
                    string responseJson = JsonConvert.SerializeObject(response);
                    Send(responseJson);
                    Log($"Server sent a message to driver: {responseJson}");
                }
                else
                {
                    if (app != null)
                    {
                        app.SendToApp(e.Data);
                        Log($"Server forwarded a message to app: {messageId}");
                    }
                    else
                    {
                        Log("Server failed to send a message to app, it is not connected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error handling message from driver: " + ex);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            lock (typeof(LightServer))
            {
                if (driver == this)
                {
                    driver = null;
                    Log("Driver disconnected.");
                }
            }
        }
    }
}
