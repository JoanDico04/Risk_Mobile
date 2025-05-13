using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using System;
using System.Collections.Generic;
using TMPro;

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
    public string action;
    public string token;
}

public class Client : MonoBehaviour
{
    public TMP_InputField usuarioInput;
    public TMP_InputField contrasenyaInput;

    private WebSocket ws;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    public string token;

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

                if (respuesta.action == "login" && !string.IsNullOrEmpty(respuesta.token))
                {
                    token = respuesta.token;
                    Debug.Log("Token guardado correctamente: " + token);

                    mainThreadActions.Enqueue(() => SceneManager.LoadScene(1));
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error al procesar la respuesta del servidor: " + ex.Message);
            }
        };

        ws.Connect();
        DontDestroyOnLoad(gameObject);
    }

    public void EnviarMensaje()
    {
        if (ws != null && ws.IsAlive)
        {
            string username = usuarioInput.text;
            string password = contrasenyaInput.text;

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

        }
        else
        {
            Debug.LogWarning("No hay conexión WebSocket activa.");
        }
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

    public void SendMessageToServer(string json)
    {
        if (IsConnected())
        {
            ws.Send(json);
        }
    }
}
