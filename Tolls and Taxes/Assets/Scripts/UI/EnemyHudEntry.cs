using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHudEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;

    [SerializeField] private Image hpFill;
    [SerializeField] private Image portraitImage;

    public void SetStats(
        string enemyName,
        int currentHp,
        int maxHp,
        Sprite portrait)
    {
        int safeMaxHp = Mathf.Max(1, maxHp);
        int displayedHp = Mathf.Clamp(currentHp, 0, safeMaxHp);

        nameText.text = string.IsNullOrWhiteSpace(enemyName)
            ? "ENEMY"
            : enemyName.ToUpperInvariant();

        hpText.text = $"HP {displayedHp}/{safeMaxHp}";
        hpFill.fillAmount =
            Mathf.Clamp01((float)displayedHp / safeMaxHp);

        portraitImage.sprite = portrait;
        portraitImage.enabled = portrait != null;
    }
}