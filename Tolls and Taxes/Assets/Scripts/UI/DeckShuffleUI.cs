using System.Collections;
using UnityEngine;

public class DeckShuffleUI : MonoBehaviour
{
    public static DeckShuffleUI Instance { get; private set; }

    [SerializeField] private GameObject shuffleRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform[] cards;

    [Header("Animation Timing")]
    [SerializeField] private float splitDuration = 0.25f;
    [SerializeField] private float mergeDuration = 0.4f;
    [SerializeField] private float holdDuration = 0.15f;
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine activeAnimation;

    private void Awake()
    {
        Instance = this;

        if (shuffleRoot != null)
        {
            shuffleRoot.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static void PlayIfAvailable()
    {
        Instance?.Play();
    }

    public void Play()
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }

        activeAnimation = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        if (shuffleRoot == null ||
            canvasGroup == null ||
            cards == null ||
            cards.Length == 0)
        {
            activeAnimation = null;
            yield break;
        }

        shuffleRoot.SetActive(true);
        canvasGroup.alpha = 1f;

        int cardCount = cards.Length;
        int halfCount = Mathf.CeilToInt(cardCount / 2f);

        Vector2[] startPositions = new Vector2[cardCount];
        Vector2[] splitPositions = new Vector2[cardCount];
        Vector2[] mergedPositions = new Vector2[cardCount];

        for (int i = 0; i < cardCount; i++)
        {
            float centeredIndex =
                i - ((cardCount - 1) / 2f);

            // Begin as a small horizontal fan.
            startPositions[i] =
                new Vector2(centeredIndex * 28f, 0f);

            // Divide into left and right piles.
            bool leftPile = i < halfCount;

            splitPositions[i] =
                new Vector2(
                    leftPile ? -145f : 145f,
                    (i % halfCount) * 7f);

            // Interleave the cards back into one stack.
            mergedPositions[i] =
                new Vector2(
                    i % 2 == 0 ? -10f : 10f,
                    i * 4f);
        }

        SetCardPositions(startPositions);

        yield return MoveCards(
            startPositions,
            splitPositions,
            splitDuration);

        yield return MoveCards(
            splitPositions,
            mergedPositions,
            mergeDuration);

        yield return new WaitForSecondsRealtime(
            holdDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha =
                1f - Mathf.Clamp01(
                    elapsed / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        shuffleRoot.SetActive(false);
        activeAnimation = null;
    }

    private IEnumerator MoveCards(
        Vector2[] startingPositions,
        Vector2[] endingPositions,
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float amount = Mathf.Clamp01(
                elapsed / duration);

            // Smooth movement instead of linear movement.
            amount =
                amount * amount * (3f - 2f * amount);

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].anchoredPosition =
                    Vector2.Lerp(
                        startingPositions[i],
                        endingPositions[i],
                        amount);
            }

            yield return null;
        }

        SetCardPositions(endingPositions);
    }

    private void SetCardPositions(
        Vector2[] positions)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].anchoredPosition =
                positions[i];

            cards[i].localRotation =
                Quaternion.identity;
        }
    }
}