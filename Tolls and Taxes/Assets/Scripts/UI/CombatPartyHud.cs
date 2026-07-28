using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPartyHud : MonoBehaviour
{
    [SerializeField]
    private PartyHudEntry[] hudEntries =
        new PartyHudEntry[3];

    private readonly List<PlayerCombatant> partyMembers =
        new List<PlayerCombatant>();

    private IEnumerator Start()
    {
        HideAllEntries();

        // Combatants are spawned when combat begins, so wait for them.
        while (partyMembers.Count == 0)
        {
            FindPartyMembers();
            yield return null;
        }
    }

    private void LateUpdate()
    {
        if (partyMembers.Count == 0)
        {
            FindPartyMembers();
            return;
        }

        for (int i = 0; i < hudEntries.Length; i++)
        {
            PartyHudEntry entry = hudEntries[i];

            if (entry == null)
            {
                continue;
            }

            if (i >= partyMembers.Count ||
                partyMembers[i] == null)
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            PlayerCombatant member = partyMembers[i];

            if (DataCenter.Instance == null)
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            PlayableData staticData = member.StaticPlayableData;

            if (staticData == null &&
                !DataCenter.Instance.Allies.TryGetValue(
                    member.CombatantName,
                    out staticData))
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            int level = Mathf.Max(1, member.Level);

            int maxHp =
                DataCenter.Instance.maxHealthCalculation(
                    staticData,
                    level);

            int maxMp =
                DataCenter.Instance.maxManaCalculation(
                    staticData,
                    level);

            entry.gameObject.SetActive(true);

            entry.SetStats(
                member.CombatantName,
                member.currentHP,
                maxHp,
                member.currentMP,
                maxMp);
        }
    }

    private void FindPartyMembers()
    {
        PlayerCombatant[] foundMembers =
            FindObjectsByType<PlayerCombatant>(
                FindObjectsSortMode.None);

        if (foundMembers.Length == 0)
        {
            return;
        }

        partyMembers.Clear();

        // Preserve the same party order used during exploration.
        if (TransferCenter.Instance != null)
        {
            foreach (string memberName
                     in TransferCenter.Instance.PartyOrder)
            {
                foreach (PlayerCombatant candidate
                         in foundMembers)
                {
                    if (candidate != null &&
                        candidate.CombatantName == memberName)
                    {
                        partyMembers.Add(candidate);
                        break;
                    }
                }
            }
        }

        // Include any combatants not already added.
        foreach (PlayerCombatant candidate in foundMembers)
        {
            if (candidate != null &&
                !partyMembers.Contains(candidate))
            {
                partyMembers.Add(candidate);
            }
        }
    }

    private void HideAllEntries()
    {
        foreach (PartyHudEntry entry in hudEntries)
        {
            if (entry != null)
            {
                entry.gameObject.SetActive(false);
            }
        }
    }
}