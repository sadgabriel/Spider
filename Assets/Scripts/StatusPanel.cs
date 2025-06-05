using TMPro;
using UnityEngine;

class StatusPanel : Panel
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;

    private void Update()
    {
        Player player = Player.Instance;
        if (player != null && hpText != null)
        {
            hpText.text = $"HP: {player.Life}";
        }
        if (player != null && staminaText != null)
        {
            staminaText.text = $"Stamina: {player.Stamina}";
        }
    }
}
