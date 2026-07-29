using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyHudEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text mpText;

    [SerializeField] private Image hpFill;
    [SerializeField] private Image mpFill;
    [SerializeField] public Image Icon;

    public void SetStats(
        string memberName,
        int currentHp,
        int maxHp,
        int currentMp,
        int maxMp)
    {
        int safeMaxHp = Mathf.Max(1, maxHp);
        int safeMaxMp = Mathf.Max(1, maxMp);

        int displayedHp = Mathf.Clamp(currentHp, 0, maxHp);
        int displayedMp = Mathf.Clamp(currentMp, 0, maxMp);

        nameText.text = memberName;
        hpText.text = $"HP {displayedHp}/{maxHp}";
        mpText.text = $"MP {displayedMp}/{maxMp}";

        hpFill.fillAmount =
            Mathf.Clamp01((float)displayedHp / safeMaxHp);

        mpFill.fillAmount =
            Mathf.Clamp01((float)displayedMp / safeMaxMp);
    }
}