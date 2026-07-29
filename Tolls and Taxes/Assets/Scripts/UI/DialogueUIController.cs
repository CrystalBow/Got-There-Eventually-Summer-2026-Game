using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public struct DialogueLine
{
    public string speakerName;

    [TextArea(2, 6)]
    public string dialogueText;

    public Sprite speakerPortrait;
}

public class DialogueUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continuePrompt;

    [Header("Input")]

    [Header("Typing")]
    [SerializeField] private float secondsPerCharacter = 0.025f;
    [SerializeField] private bool pauseGameplay = true;

    private DialogueLine[] activeLines;
    private int currentLineIndex;
    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool isDialogueOpen;
    private string fullCurrentText;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        dialogueRoot.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueOpen)
        {
            return;
        }

        bool keyboardPressed =
            Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame ||
             Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
             Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.eKey.wasPressedThisFrame);

        bool gamepadPressed =
            Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (keyboardPressed || gamepadPressed)
        {
            Advance();
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        // Replaces the normal gameplay reminder with the dialogue controls.
        ControlReminderUI.Instance?.Show(
            ControlReminderContext.Dialogue);

        activeLines = lines;
        currentLineIndex = 0;
        isDialogueOpen = true;
        dialogueRoot.SetActive(true);

        if (pauseGameplay)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        DisplayCurrentLine();
    }

    public void Advance()
    {
        if (!isDialogueOpen)
        {
            return;
        }

        if (isTyping)
        {
            FinishTypingImmediately();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= activeLines.Length)
        {
            CloseDialogue();
            return;
        }

        DisplayCurrentLine();
    }

    public void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        isDialogueOpen = false;
        dialogueRoot.SetActive(false);

        if (pauseGameplay)
        {
            Time.timeScale = previousTimeScale;
        }

        // Restores the normal exploration reminder after dialogue closes.
        ControlReminderUI.Instance?.Show(
            ControlReminderContext.Exploration);
    }

    private void DisplayCurrentLine()
    {
        DialogueLine line = activeLines[currentLineIndex];

        speakerNameText.text = string.IsNullOrWhiteSpace(line.speakerName)
            ? "UNKNOWN"
            : line.speakerName.ToUpperInvariant();

        speakerPortrait.sprite = line.speakerPortrait;
        speakerPortrait.enabled = line.speakerPortrait != null;

        fullCurrentText = line.dialogueText ?? string.Empty;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeCurrentLine());
    }

    private IEnumerator TypeCurrentLine()
    {
        isTyping = true;
        continuePrompt.gameObject.SetActive(false);
        dialogueText.text = string.Empty;

        foreach (char character in fullCurrentText)
        {
            dialogueText.text += character;

            float elapsed = 0f;

            while (elapsed < secondsPerCharacter)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        isTyping = false;
        typingCoroutine = null;
        continuePrompt.gameObject.SetActive(true);
    }

    private void FinishTypingImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = fullCurrentText;
        isTyping = false;
        continuePrompt.gameObject.SetActive(true);
    }
}