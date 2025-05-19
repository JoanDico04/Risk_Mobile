using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Este script debe estar en un objeto con Image.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = Color.magenta; 
        }
    }
}
