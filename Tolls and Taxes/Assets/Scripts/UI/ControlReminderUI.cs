using System;
using TMPro;
using UnityEngine;

public enum ControlReminderContext
{
    Exploration,
    ExplorationCards,
    CombatCards,
    TargetSelection,
    Dialogue,
    Menu,
    Hidden,
    Rest
}

public class ControlReminderUI : MonoBehaviour
{
    public static ControlReminderUI Instance { get; private set; }

    [SerializeField] private GameObject reminderPanel;
    [SerializeField] private TMP_Text reminderText;

    private void Awake()
    {
        Instance = this;
        Show(ControlReminderContext.Exploration);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        Instance = this;
    }

    public void Show(ControlReminderContext context)
    {
        if (context == ControlReminderContext.Hidden)
        {
            reminderPanel.SetActive(false);
            return;
        }

        reminderPanel.SetActive(true);

        reminderText.text = context switch
        {
            ControlReminderContext.Exploration =>
                "WASD — MOVE     SPACE — CARDS     V — DECK     Enter — Shuffle",

            ControlReminderContext.ExplorationCards =>
                "A / D — SELECT Character   1 / 2 - SELECT Card   Space — USE     C — CANCEL    Enter - Discard",

            ControlReminderContext.CombatCards =>
                "A / D — SELECT CARD     ENTER — CONFIRM     C — DISCARD   Enter - SHUFFLE",

            ControlReminderContext.TargetSelection =>
                "A / D — SELECT TARGET     ENTER — CONFIRM     C — BACK",

            ControlReminderContext.Dialogue =>
                "ENTER / SPACE — CONTINUE",

            ControlReminderContext.Menu =>
                "WASD - SELECT Character     1 / 2 - VIEW Card     ESC — Back",
            ControlReminderContext.Rest => 
                "C - CANCEL Shuffle",

            _ => string.Empty
        };
    }

    private void OnDisable()
    {
        Show(ControlReminderContext.Hidden);
    }
}