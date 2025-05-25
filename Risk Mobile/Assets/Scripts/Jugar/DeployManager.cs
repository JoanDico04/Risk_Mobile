using System;
using System.Collections.Generic;
using UnityEngine;

public class DeployManager : MonoBehaviour
{
    public Color GetColorForPlayer(int playerId)
    {
        switch (playerId)
        {
            case 1: return Color.red;
            case 2: return Color.magenta;
            case 3: return new Color(1f, 0.5f, 0.5f);
            case 4: return Color.cyan;
            case 5: return Color.green;
            default: return Color.gray;
        }
    }


    public static DeployManager Instance;

    private int activePlayerID;
    private string currentPhase;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Client.Instance.OnServerMessage += OnServerMessage;
    }

    private void OnDestroy()
    {
        Client.Instance.OnServerMessage -= OnServerMessage;
    }

    private void OnServerMessage(string data)
    {
        if (data.Contains("\"fase\":\"deploy\""))
        {
            try
            {
                ServerDeployResponse wrapper = JsonUtility.FromJson<ServerDeployResponse>(data);
                if (wrapper != null && wrapper.response != null)
                {
                    currentPhase = wrapper.response.fase;
                    activePlayerID = wrapper.response.active_player;
                    Debug.Log("FASE DEPLOY ACTIVADA. Jugador activo: " + activePlayerID);

                    string countryCode = wrapper.response.info.country;

                    GameObject countryGO = GameObject.Find("Paisos/" + countryCode);
                    if (countryGO != null)
                    {
                        var countryButton = countryGO.GetComponent<CountryButton>();
                        if (countryButton != null)
                        {
                            countryButton.SetOwner(wrapper.response.info.id);
                            countryButton.SetTroops(wrapper.response.info.troops);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Debug.Log("Error leyendo deploy: " + e.Message);
            }
        }

    }

    public void OnCountryClicked(string countryCode)
    {
        if (currentPhase != "deploy")
        {
            Debug.Log("No estamos en fase deploy");
            return;
        }

        if (Client.Instance == null || string.IsNullOrEmpty(Client.Instance.token))
        {
            Debug.LogWarning("No hay token disponible");
            return;
        }

        int salaID = Client.Instance.salaActual.id;

        string json = $"{{\"action\":\"deploy\",\"token\":\"{Client.Instance.token}\",\"info\":{{\"sala\":{salaID},\"country\":\"{countryCode}\"}}}}";
        Debug.Log("Enviando deploy: " + json);
        Client.Instance.SendMessageToServer(json);
    }
}

[Serializable]
public class ServerDeployResponse
{
    public DeployResponse response;
}

[Serializable]
public class DeployResponse
{
    public string fase;
    public int active_player;
    public DeployInfo info;
}

[Serializable]
public class DeployInfo
{
    public int id;
    public string country;
    public int troops;
}
