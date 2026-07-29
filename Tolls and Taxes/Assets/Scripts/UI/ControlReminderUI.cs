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
    Hidden
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
                "WASD — MOVE     SPACE — CARDS     V — DECK     ESC — PAUSE",

            ControlReminderContext.ExplorationCards =>
                "A / D — SELECT CARD     ENTER — USE     C — CANCEL",

            ControlReminderContext.CombatCards =>
                "A / D — SELECT CARD     ENTER — CONFIRM     C — CANCEL",

            ControlReminderContext.TargetSelection =>
                "A / D — SELECT TARGET     ENTER — CONFIRM     C — BACK",

            ControlReminderContext.Dialogue =>
                "ENTER / SPACE — CONTINUE",

            ControlReminderContext.Menu =>
                "WASD / ARROWS — NAVIGATE     ENTER — SELECT     ESC — BACK",

            _ => string.Empty
        };
    }
}