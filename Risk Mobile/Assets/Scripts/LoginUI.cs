using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usuarioInput;
    public TMP_InputField contrasenyaInput;

    public void OnLoginButton()
    {
        if (Client.Instance != null)
        {
            Client.Instance.EnviarMensaje(usuarioInput.text, contrasenyaInput.text);
        }
        else
        {
            Debug.LogWarning("No se encontró el cliente.");
        }
    }
}
