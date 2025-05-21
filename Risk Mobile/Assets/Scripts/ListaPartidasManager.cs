using UnityEngine;
using TMPro;
using System.Collections;

public class ListaPartidasManager : MonoBehaviour
{
    public Transform contenedor;
    public GameObject partidaPrefab;


    void Start()
    {
        StartCoroutine(EsperarYMostrarPartidas());
    }

    IEnumerator EsperarYMostrarPartidas()
    {
        float tiempoMaximo = 5f;
        float tiempoEsperado = 0f;

        while ((Client.Instance == null || Client.SalasRecibidas == null || Client.SalasRecibidas.Count == 0)
               && tiempoEsperado < tiempoMaximo)
        {
            yield return new WaitForSeconds(0.2f);
            tiempoEsperado += 0.2f;
        }

        MostrarPartidas();
    }

    void MostrarPartidas()
    {
        if (Client.Instance == null || Client.SalasRecibidas == null)
        {
            Debug.LogWarning("Client o lista de salas nula");
            return;
        }

        // Limpia partidas anteriores
        foreach (Transform hijo in contenedor)
        {
            Destroy(hijo.gameObject);
        }

        foreach (var sala in Client.SalasRecibidas)
        {
            GameObject nueva = Instantiate(partidaPrefab, contenedor);
            nueva.transform.localPosition = Vector3.zero;
            nueva.transform.localScale = Vector3.one;

            TMP_Text nombreText = nueva.transform.Find("NombreSala")?.GetComponent<TMP_Text>();
            TMP_Text jugadoresText = nueva.transform.Find("JugadoresSala")?.GetComponent<TMP_Text>();

            if (nombreText != null)
            {
                nombreText.text = "Nombre: " + sala.nom;
                Debug.Log("NombreText actualizado con: " + nombreText.text);
            }
            else
            {
                Debug.LogWarning("No se encontró NombreText en el prefab");
            }

            if (jugadoresText != null)
            {
                jugadoresText.text = $"Jugadores: {sala.connected}/{sala.max_players}";
                Debug.Log("JugadoresText actualizado con: " + jugadoresText.text);
            }
            else
            {
                Debug.LogWarning("No se encontró JugadoresText en el prefab");
            }

            Debug.Log($"Instanciado '{sala.nom}' en posición {nueva.transform.localPosition} con escala {nueva.transform.localScale}");
        }


        Debug.Log("Partidas mostradas correctamente.");
    }
}
