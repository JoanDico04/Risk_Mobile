using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

[Serializable]
public class StartMatchResponse
{
    public string action;
    public int start;
    public SalaData sala;
}

[Serializable]
public class ServerStartMatchWrapper
{
    public StartMatchResponse response;
}

public class IniciarPartida : MonoBehaviour
{
    public Button comenzarButton;

    private int salaID;

    private void Start()
    {
        if (Client.Instance == null || Client.Instance.salaActual == null)
        {
            Debug.LogWarning("Client o salaActual no están listos al iniciar IniciarPartida.");
            salaID = 0;
        }
        else
        {
            salaID = Client.Instance.salaActual.id;
        }

        Client.Instance.OnServerMessage += OnServerMessage;
        InvokeRepeating(nameof(ActualizarEstadoBoton), 1f, 1f);
    }

    private void OnDestroy()
    {
        if (Client.Instance != null)
            Client.Instance.OnServerMessage -= OnServerMessage;
    }

    public void Comenzar()
    {
        if (Client.Instance == null || string.IsNullOrEmpty(Client.Instance.token))
        {
            Debug.LogWarning("No hay token disponible");
            return;
        }

        var sala = Client.SalasRecibidas.Find(s => s.id == salaID);
        if (sala == null || sala.connected < 2)
        {
            Debug.LogWarning("No hay suficientes jugadores para iniciar la partida.");
            return;
        }

        string json = $"{{\"action\":\"iniciar_partida\", \"token\":\"{Client.Instance.token}\", \"info\":{{\"sala_id\":{salaID}}}}}";
        Debug.Log("Enviando mensaje de iniciar partida: " + json);
        Client.Instance.SendMessageToServer(json);
    }

    private void OnServerMessage(string data)
    {
        try
        {
            var wrapper = JsonUtility.FromJson<ServerStartMatchWrapper>(data);
            if (wrapper != null && wrapper.response != null && wrapper.response.action == "start_match" && wrapper.response.start == 1)
            {
                Debug.Log("Partida iniciada correctamente, cambiando a escena 'Jugar'");
                SceneManager.LoadScene(2);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error procesando respuesta de inicio de partida: " + e.Message);
        }
    }

    private void ActualizarEstadoBoton()
    {
        if (Client.Instance == null || Client.SalasRecibidas == null) return;

        if (Client.Instance.salaActual != null)
            salaID = Client.Instance.salaActual.id;

        var sala = Client.SalasRecibidas.Find(s => s.id == salaID);
        if (comenzarButton != null)
        {
            comenzarButton.interactable = sala != null && sala.connected >= 2;
        }
    }
}
