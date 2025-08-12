using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdlePanel : Panel<NoData>
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private RawImage selectedSpecialActionIcon;
    [SerializeField] private Image HpBar;
    [SerializeField] private Image StaminaBar;

    private float BarMaxWidth;
    private float BarLeftEdge;

    protected void Awake()
    {
        if (HpBar != null)
        {
            BarMaxWidth = HpBar.rectTransform.sizeDelta.x;
            BarLeftEdge = HpBar.rectTransform.anchoredPosition.x - (BarMaxWidth * 0.5f);
        }
    }

    private void Update()
    {
        Player player = Player.Instance;
        if (player == null) return;

        if (hpText != null)
        {
            hpText.text = $"HP: {player.Hp}/{player.MaxHp}";
        }

        if (HpBar != null)
        {
            float hpPercentage = (float)player.Hp / player.MaxHp;
            HpBar.rectTransform.sizeDelta = new Vector2(BarMaxWidth * hpPercentage, HpBar.rectTransform.sizeDelta.y);

            HpBar.rectTransform.anchoredPosition = new Vector2(BarLeftEdge + 0.5f * BarMaxWidth * hpPercentage, HpBar.rectTransform.anchoredPosition.y);
        }

        if (staminaText != null)
        {
            staminaText.text = $"Stamina: {player.Stamina}/{player.MaxStamina}";
        }

        if (StaminaBar != null)
        {
            float staminaPercentage = (float)player.Stamina / player.MaxStamina;
            StaminaBar.rectTransform.sizeDelta = new Vector2(BarMaxWidth * staminaPercentage, StaminaBar.rectTransform.sizeDelta.y);

            StaminaBar.rectTransform.anchoredPosition = new Vector2(BarLeftEdge + 0.5f * BarMaxWidth * staminaPercentage, StaminaBar.rectTransform.anchoredPosition.y);
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
