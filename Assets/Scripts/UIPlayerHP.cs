using TMPro;
using UnityEngine;

public class UIPlayerHP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;

    private void Update()
    {
        Player player = Player.Instance;
        if (player != null && hpText != null)
        {
            hpText.text = $"HP: {player.Life}";
        }
    }
}
