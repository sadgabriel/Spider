using TMPro;
using UnityEngine;

public class IdlePanel : Panel<NoData>
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;

    private void Update()
    {
        Player player = Player.Instance;
        if (player == null) return;

        if (hpText != null)
        {
            hpText.text = $"HP: {player.HP}";
        }
        if (staminaText != null)
        {
            staminaText.text = $"Stamina: {player.Stamina}";
        }
    }

    public override void Show(NoData data = default)
    {
        gameObject.SetActive(true);
    }   
}
