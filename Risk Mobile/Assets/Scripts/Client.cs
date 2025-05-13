using UnityEngine;
using WebSocketSharp;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[Serializable]
public class LoginData
{
    public string user;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string action;
    public LoginData login;
}

[Serializable]
public class TokenResponse
{
    public string token;
}

public class Client : MonoBehaviour
{
    public static Client Instance;

    private WebSocket ws;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    public string token;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ws = new WebSocket("ws://10.200.1.4:8080");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Conectado al servidor WebSocket");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Mensaje recibido del servidor: " + e.Data);
            try
            {
                var respuesta = JsonUtility.FromJson<TokenResponse>(e.Data);
                if (!string.IsNullOrEmpty(respuesta.token))
                {
                    mainThreadActions.Enqueue(() =>
                    {
                        token = respuesta.token;
                        Debug.Log("Token guardado: " + token);
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("No se pudo interpretar el token: " + ex.Message);
            }
        };

        ws.Connect();
    }

    void Update()
    {
        while (mainThreadActions.Count > 0)
        {
            var action = mainThreadActions.Dequeue();
            action?.Invoke();
        }
    }

    public bool IsConnected()
    {
        return ws != null && ws.IsAlive;
    }

    public void EnviarMensaje(string username, string password)
    {
        if (IsConnected())
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogWarning("Usuario o contraseña vacíos");
                return;
            }

            LoginData login = new LoginData
            {
                user = username,
                password = password
            };

            LoginRequest request = new LoginRequest
            {
                action = "login",
                login = login
            };

            string json = JsonUtility.ToJson(request);
            Debug.Log("JSON enviado al servidor: " + json);
            ws.Send(json);

            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogWarning("No hay conexión WebSocket activa. No se cambia de escena.");
        }
    }

    public void SendMessageToServer(string json)
    {
        if (IsConnected())
        {
            ws.Send(json);
        }
    }
}
