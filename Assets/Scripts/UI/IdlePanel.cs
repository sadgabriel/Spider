using TMPro;
using UnityEngine;

class IdlePanel : Panel<NoData>
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;

    public override void Show(NoData data = default)
    {
        gameObject.SetActive(true);
    }

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
