using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdlePanel : Panel<NoData>
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private RawImage selectedSpecialActionIcon;

    private void Update()
    {
        Player player = Player.Instance;
        if (player == null) return;

        if (hpText != null)
        {
            hpText.text = $"HP: {player.Hp}";
        }

        if (staminaText != null)
        {
            staminaText.text = $"Stamina: {player.Stamina}";
        }

        if (selectedSpecialActionIcon != null && SpecialActionManager.Instance.SelectedSpecialAction != null)
        {
            selectedSpecialActionIcon.texture = SpecialActionManager.Instance.SelectedSpecialAction.Icon;
            selectedSpecialActionIcon.color = new Color(0f, 0f, 1f, 1f);
        }
        else
        {
            selectedSpecialActionIcon.texture = null;
            selectedSpecialActionIcon.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    public override void Show(NoData data = default)
    {
        gameObject.SetActive(true);
    }   
}
