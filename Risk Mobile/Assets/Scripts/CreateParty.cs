using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InfoData
{
    public string nom;
    public int max_players;
}

[System.Serializable]
public class CreateGameRequest
{
    public string action;
    public string token;
    public InfoData info;
}

public class CreateParty : MonoBehaviour
{
    public TMP_InputField nombrePartidaInput;
    public TMP_Dropdown jugadoresDropdown;
    public GameObject panel;
    public GameObject panelDesactivar;

    void Start()
    {
        panel.SetActive(false);
        jugadoresDropdown.value = 0;
        jugadoresDropdown.RefreshShownValue();
    }

    public void AbrirPanel()
    {
        panel.SetActive(true);
        panelDesactivar.SetActive(false);
    }

    public void CerrarPanel()
    {
        panel.SetActive(false);
    }

    public void Confirmar()
    {
        if (Client.Instance == null || !Client.Instance.IsConnected())
        {
            Debug.LogWarning("Cliente no conectado al servidor.");
            return;
        }

        if (string.IsNullOrEmpty(Client.Instance.token))
        {
            Debug.LogWarning("Token no disponible aún. Espera a que se reciba del servidor.");
            return;
        }

        string nombrePartida = nombrePartidaInput.text.Trim();

        if (string.IsNullOrEmpty(nombrePartida))
        {
            Debug.LogWarning("El nombre de la partida no puede estar vacío.");
            return;
        }

        InfoData info = new InfoData
        {
            nom = nombrePartida,
            max_players = int.Parse(jugadoresDropdown.options[jugadoresDropdown.value].text)
        };

        CreateGameRequest request = new CreateGameRequest
        {
            action = "create",
            token = Client.Instance.token,
            info = info
        };

        string json = JsonUtility.ToJson(request);
        Debug.Log("JSON enviado al servidor (crear partida): " + json);
        Client.Instance.SendMessageToServer(json);

        CerrarPanel();
        SceneManager.LoadScene(2);
    }
}
