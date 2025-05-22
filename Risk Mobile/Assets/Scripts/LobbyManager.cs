using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text jugadoresText;
    private int salaID;

    void Start()
    {
        StartCoroutine(EsperarSalaYActualizar());
    }

    IEnumerator EsperarSalaYActualizar()
    {
        while (Client.Instance == null || Client.Instance.salaActual == null || Client.Instance.salaActual.id == 0)
        {
            yield return null;
        }

        salaID = Client.Instance.salaActual.id;
        Debug.Log($"Sala válida recibida en LobbyManager: {salaID}");

        StartCoroutine(ActualizarJugadoresLoop());
    }

    IEnumerator ActualizarJugadoresLoop()
    {
        while (true)
        {
            if (Client.Instance != null && Client.Instance.IsConnected())
            {
                var salas = Client.SalasRecibidas;

                if (salas != null)
                {
                    var sala = salas.Find(s => s.id == salaID);
                    if (sala != null && jugadoresText != null)
                    {
                        jugadoresText.text = $"Jugadores: {sala.connected}/{sala.max_players}";
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
