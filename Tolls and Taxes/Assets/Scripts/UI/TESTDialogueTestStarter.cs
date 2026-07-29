using UnityEngine;

public class DialogueTestStarter : MonoBehaviour
{
    [SerializeField] private DialogueUIController dialogueUI;

    [SerializeField] private Sprite firstPortrait;
    [SerializeField] private Sprite secondPortrait;

    private void Start()
    {
        DialogueLine[] lines =
        {
            new DialogueLine
            {
                speakerName = "Samantha Pel",
                dialogueText = "We should find out why the road ahead is blocked.",
                speakerPortrait = firstPortrait
            },
            new DialogueLine
            {
                speakerName = "John Goblinus",
                dialogueText = "Maybe attacking that object will clear the way.",
                speakerPortrait = secondPortrait
            }
        };

        dialogueUI.StartDialogue(lines);
    }
}