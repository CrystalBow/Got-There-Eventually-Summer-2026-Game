using UnityEngine;

public class ExplorationPartyHud : MonoBehaviour
{
    [SerializeField] private PartyMember[] partyMembers;
    [SerializeField] private PartyHudEntry[] hudEntries;

    private void LateUpdate()
    {
        if (hudEntries == null)
        {
            return;
        }

        for (int i = 0; i < hudEntries.Length; i++)
        {
            PartyHudEntry entry = hudEntries[i];

            if (entry == null)
            {
                continue;
            }

            if (partyMembers == null ||
                i >= partyMembers.Length ||
                partyMembers[i] == null)
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            PartyMember member = partyMembers[i];

            if (DataCenter.Instance == null ||
                !DataCenter.Instance.Allies.TryGetValue(
                    member.MemberName,
                    out PlayableData staticData))
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            entry.gameObject.SetActive(true);

            entry.SetStats(
                member.MemberName,
                member.HP,
                staticData.Hp,
                member.MP,
                staticData.Mp);
        }
    }
}