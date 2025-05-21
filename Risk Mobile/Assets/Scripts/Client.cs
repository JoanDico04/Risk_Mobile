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
    public string message;
    public string token;
    public int id;
}

[Serializable]
public class TokenWrapper
{
    public int status;
    public TokenResponse response;
}

[Serializable]
public class SalaData
{
    public int id;
    public string nom;
    public int max_players;
    public string date;
    public string token;
    public int admin_id;
    public int connected;
    public int estat_torn;
    public int? torn_player_id;
}

public class SalaWrapper
{
    public int status;
    public LobbyResponse response;
}

[Serializable]
public class LobbyResponse
{
    public List<SalaData> salas;
}

public class Client : MonoBehaviour
{
    public static Client Instance;

    private WebSocket ws;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    public string token;
    public static List<SalaData> SalasRecibidas { get; private set; } = new List<SalaData>();
    public bool EstaBuscandoPartida = false;

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

            if (EstaBuscandoPartida)
            {
                try
                {
                    var lobbyResponse = JsonUtility.FromJson<SalaWrapper>(e.Data);
                    if (lobbyResponse != null && lobbyResponse.response.salas != null)
                    {
                        mainThreadActions.Enqueue(() =>
                        {
                            SalasRecibidas = lobbyResponse.response.salas;
                            Debug.Log($"Se recibieron {SalasRecibidas.Count} salas activas.");
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Error al parsear salas: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    var loginWrapper = JsonUtility.FromJson<TokenWrapper>(e.Data);
                    if (loginWrapper != null && loginWrapper.response != null)
                    {
                        mainThreadActions.Enqueue(() =>
                        {
                            token = loginWrapper.response.token;
                            Debug.Log("Token guardado desde login: " + token);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Error parseando token login: " + ex.Message);
                }
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
