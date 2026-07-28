using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatHud : MonoBehaviour
{
    [SerializeField] private EnemyHudEntry enemyEntryPrefab;
    [SerializeField] private Transform entryContainer;

    private readonly Dictionary<Combatant, EnemyHudEntry>
        activeEntries = new();

    private float nextScanTime;

    private void Awake()
    {
        if (entryContainer == null)
        {
            return;
        }

        // Removes the editor preview rows when the game starts.
        for (int i = entryContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(entryContainer.GetChild(i).gameObject);
        }
    }
    private void LateUpdate()
    {
        if (Time.unscaledTime >= nextScanTime)
        {
            ScanForEnemies();
            nextScanTime = Time.unscaledTime + 0.25f;
        }

        RefreshEntries();
    }

    private void ScanForEnemies()
    {
        Combatant[] allCombatants =
            FindObjectsByType<Combatant>(
                FindObjectsSortMode.None);

        List<Combatant> enemies = new();

        foreach (Combatant combatant in allCombatants)
        {
            if (combatant == null)
            {
                continue;
            }

            // PlayerCombatant inherits from Combatant,
            // so exclude player characters.
            if (combatant is PlayerCombatant)
            {
                continue;
            }

            // Wait until the combat system initializes this enemy.
            if (combatant.StaticData == null)
            {
                continue;
            }

            if (combatant.isDead())
            {
                continue;
            }

            enemies.Add(combatant);
        }

        // EnemyLoader places enemies from left to right.
        enemies.Sort(
            (first, second) =>
                first.transform.position.x.CompareTo(
                    second.transform.position.x));

        HashSet<Combatant> currentEnemies = new(enemies);

        for (int index = 0; index < enemies.Count; index++)
        {
            Combatant enemy = enemies[index];

            if (!activeEntries.TryGetValue(
                    enemy,
                    out EnemyHudEntry entry))
            {
                entry = Instantiate(
                    enemyEntryPrefab,
                    entryContainer);

                activeEntries.Add(enemy, entry);
            }

            entry.transform.SetSiblingIndex(index);
        }

        List<Combatant> enemiesToRemove = new();

        foreach (
            KeyValuePair<Combatant, EnemyHudEntry> pair
            in activeEntries)
        {
            if (pair.Key == null ||
                !currentEnemies.Contains(pair.Key) ||
                pair.Key.isDead())
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value.gameObject);
                }

                enemiesToRemove.Add(pair.Key);
            }
        }

        foreach (Combatant enemy in enemiesToRemove)
        {
            activeEntries.Remove(enemy);
        }
    }

    private void RefreshEntries()
    {
        foreach (
            KeyValuePair<Combatant, EnemyHudEntry> pair
            in activeEntries)
        {
            Combatant enemy = pair.Key;
            EnemyHudEntry entry = pair.Value;

            if (enemy == null ||
                entry == null ||
                enemy.StaticData == null)
            {
                continue;
            }

            SpriteRenderer spriteRenderer =
                enemy.GetComponent<SpriteRenderer>();

            Sprite portrait = spriteRenderer != null
                ? spriteRenderer.sprite
                : null;

            entry.SetStats(
                enemy.CombatantName,
                enemy.currentHP,
                enemy.StaticData.Hp,
                portrait);
        }
    }
}