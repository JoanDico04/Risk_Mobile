using UnityEngine;
using UnityEngine.SceneManagement;

public class BuscarPartidaButton : MonoBehaviour
{
    public void BuscarPartida()
    {
        if (Client.Instance != null && Client.Instance.IsConnected())
        {
            if (!string.IsNullOrEmpty(Client.Instance.token))
            {
                string json = "{\"action\":\"lobby\", \"token\":\"" + Client.Instance.token + "\"}";
                Debug.Log("Enviando petición de lobby: " + json);
                Client.Instance.SendMessageToServer(json);

                Client.Instance.EstaBuscandoPartida = true;

                SceneManager.LoadScene(7); 
            }
            else
            {
                Debug.LogWarning("Token no disponible.");
            }
        }
        else
        {
            Debug.LogWarning("Cliente no conectado.");
        }
    }
}
