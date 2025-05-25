using UnityEngine;
using TMPro; 

public class CountryButton : MonoBehaviour
{
    public string countryCode;
    public SpriteRenderer spriteRenderer; 
    public TextMeshPro troopsText; 

    private void OnMouseDown()
    {
        DeployManager.Instance.OnCountryClicked(countryCode);
    }

    public void SetOwner(int playerId)
    {
        Color color = DeployManager.Instance.GetColorForPlayer(playerId);
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    public void SetTroops(int count)
    {
        if (troopsText != null)
            troopsText.text = count.ToString();
    }
}
