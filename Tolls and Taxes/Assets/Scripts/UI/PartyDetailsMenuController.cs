using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyDetailsMenuController : MonoBehaviour
{
    [Serializable]
    private struct PortraitMapping
    {
        public string memberName;
        public Sprite portrait;
    }

    [Header("Party Members From Current Scene")]
    [SerializeField]
    private PartyMember[] partyMembers =
        new PartyMember[3];

    [Header("Member Selection Buttons")]
    [SerializeField]
    private GameObject[] memberButtonRoots =
        new GameObject[3];

    [SerializeField]
    private TMP_Text[] memberButtonNameTexts =
        new TMP_Text[3];

    [SerializeField]
    private Image[] memberButtonPortraits =
        new Image[3];

    [SerializeField]
    private Image[] memberButtonBackgrounds =
        new Image[3];

    [Header("Selected Member Details")]
    [SerializeField] private Image largePortrait;
    [SerializeField] private TMP_Text memberNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text mpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text xpText;

    [Header("Character Portraits")]
    [SerializeField] private PortraitMapping[] portraitMappings;

    [Header("Selection Colors")]
    [SerializeField]
    private Color normalButtonColor =
        new Color32(128, 85, 63, 255);

    [SerializeField]
    private Color selectedButtonColor =
        new Color32(160, 109, 82, 255);

    private int selectedIndex;

    private void OnEnable()
    {
        RefreshMemberButtons();
        SelectFirstAvailableMember();
    }

    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshSelectedMember();
        }
    }

    public void SelectMember(int index)
    {
        if (partyMembers == null ||
            index < 0 ||
            index >= partyMembers.Length ||
            partyMembers[index] == null)
        {
            return;
        }

        selectedIndex = index;
        RefreshMemberButtons();
        RefreshSelectedMember();
    }

    private void SelectFirstAvailableMember()
    {
        if (partyMembers != null &&
            selectedIndex >= 0 &&
            selectedIndex < partyMembers.Length &&
            partyMembers[selectedIndex] != null)
        {
            RefreshSelectedMember();
            return;
        }

        if (partyMembers == null)
        {
            ClearDetails();
            return;
        }

        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (partyMembers[i] != null)
            {
                selectedIndex = i;
                RefreshMemberButtons();
                RefreshSelectedMember();
                return;
            }
        }

        ClearDetails();
    }

    private void RefreshMemberButtons()
    {
        int buttonCount = memberButtonRoots?.Length ?? 0;

        for (int i = 0; i < buttonCount; i++)
        {
            bool hasMember =
                partyMembers != null &&
                i < partyMembers.Length &&
                partyMembers[i] != null;

            if (memberButtonRoots[i] != null)
            {
                memberButtonRoots[i].SetActive(hasMember);
            }

            if (!hasMember)
            {
                continue;
            }

            PartyMember member = partyMembers[i];

            if (memberButtonNameTexts != null &&
                i < memberButtonNameTexts.Length &&
                memberButtonNameTexts[i] != null)
            {
                memberButtonNameTexts[i].text =
                    member.MemberName.ToUpperInvariant();
            }

            if (memberButtonPortraits != null &&
                i < memberButtonPortraits.Length &&
                memberButtonPortraits[i] != null)
            {
                memberButtonPortraits[i].sprite =
                    FindPortrait(member.MemberName);
            }

            if (memberButtonBackgrounds != null &&
                i < memberButtonBackgrounds.Length &&
                memberButtonBackgrounds[i] != null)
            {
                memberButtonBackgrounds[i].color =
                    i == selectedIndex
                        ? selectedButtonColor
                        : normalButtonColor;
            }
        }
    }

    private void RefreshSelectedMember()
    {
        if (partyMembers == null ||
            selectedIndex < 0 ||
            selectedIndex >= partyMembers.Length ||
            partyMembers[selectedIndex] == null)
        {
            ClearDetails();
            return;
        }

        PartyMember member = partyMembers[selectedIndex];
        Sprite portrait = FindPortrait(member.MemberName);

        if (largePortrait != null)
        {
            largePortrait.sprite = portrait;
        }

        if (memberNameText != null)
        {
            memberNameText.text =
                member.MemberName.ToUpperInvariant();
        }

        int level = Mathf.Max(1, member.Level);

        if (levelText != null)
        {
            levelText.text = $"LEVEL {level}";
        }

        if (DataCenter.Instance == null ||
            !DataCenter.Instance.Allies.TryGetValue(
                member.MemberName,
                out PlayableData staticData))
        {
            ShowMissingData();
            return;
        }

        int maxHp =
            DataCenter.Instance.maxHealthCalculation(
                staticData,
                level);

        int maxMp =
            DataCenter.Instance.maxManaCalculation(
                staticData,
                level);

        int attack =
            DataCenter.Instance.AttackCalculation(
                staticData,
                level);

        int defense =
            DataCenter.Instance.DefenseCalculation(
                staticData,
                level);

        int speed =
            DataCenter.Instance.SpeedCalculation(
                staticData,
                level);

        hpText.text =
            $"HP   {Mathf.Max(0, member.HP)} / {maxHp}";

        mpText.text =
            $"MP   {Mathf.Max(0, member.MP)} / {maxMp}";

        attackText.text =
            $"ATTACK   {attack}";

        defenseText.text =
            $"DEFENSE   {defense}";

        speedText.text =
            $"SPEED   {speed}";

        xpText.text =
            $"XP   {Mathf.Max(0, member.XP)}";
    }

    private Sprite FindPortrait(string memberName)
    {
        if (portraitMappings == null)
        {
            return null;
        }

        foreach (PortraitMapping mapping in portraitMappings)
        {
            if (mapping.memberName == memberName)
            {
                return mapping.portrait;
            }
        }

        return null;
    }

    private void ShowMissingData()
    {
        hpText.text = "HP   DATA NOT FOUND";
        mpText.text = "MP   DATA NOT FOUND";
        attackText.text = "ATTACK   --";
        defenseText.text = "DEFENSE   --";
        speedText.text = "SPEED   --";
        xpText.text = "XP   --";
    }

    private void ClearDetails()
    {
        if (largePortrait != null)
        {
            largePortrait.sprite = null;
        }

        if (memberNameText != null)
        {
            memberNameText.text = "NO PARTY MEMBER";
        }

        if (levelText != null)
        {
            levelText.text = string.Empty;
        }

        if (hpText != null)
        {
            hpText.text = string.Empty;
        }

        if (mpText != null)
        {
            mpText.text = string.Empty;
        }

        if (attackText != null)
        {
            attackText.text = string.Empty;
        }

        if (defenseText != null)
        {
            defenseText.text = string.Empty;
        }

        if (speedText != null)
        {
            speedText.text = string.Empty;
        }

        if (xpText != null)
        {
            xpText.text = string.Empty;
        }
    }
}